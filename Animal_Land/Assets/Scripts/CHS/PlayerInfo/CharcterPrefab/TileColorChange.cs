using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileColorChange : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("Tile Color 정보")]
    [SerializeField]
    private Color _moveColor;
    [SerializeField]
    private Color _areaColor;
    [SerializeField]
    private Color _borderColor;

    Vector3Int      _prevPos;
    Tilemap         _tilemap;
    PhotonView      _pv;

    Tilemap         _curTilemap;
    PlayInfo        _playInfo;
    Vector3Int      _startPosition;         // moveTileList를 시작한 위치

    int test;
    void Awake()
    {
        PhotonNetwork.SendRate = 30;            // 5번/초로 메시지를 보냄
        PhotonNetwork.SerializationRate = 60;   // 5번/초로 메시지를 수신 및 업데이트
    }

    // Start is called before the first frame update
    void Start()
    {
        // TODO : 멤버 변수 초기화
        _prevPos = new Vector3Int(10000, 10000, 0);
        _tilemap = new Tilemap();
        _pv = GetComponent<PhotonView>();
        _playInfo = GetComponent<PlayInfo>();

        Vector3Int cellPosition = GetTileMap().WorldToCell(transform.position);
        UpdateTileColorToArea(new int[3] { cellPosition.x, cellPosition.y, cellPosition.z});
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 초기화가 이뤄지지 않았다면 return
        if(IsInit())
        {
            return;
        }

        // 마스터 클라이언트에서만 확인을 진행한다.
        // 나머지 클라이언트는 마스터 클라이언트의 상황을 공유받아 플레이한다.
        if (PhotonNetwork.IsMasterClient == true)
        {
            CheckTile();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting && PhotonNetwork.IsMasterClient == true && _pv.IsMine == true)
        {

        }
        else
        {

        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void CheckTile()
    {

        Vector3Int cellPos = GetTileMap().WorldToCell(transform.position);
        TileBase tile = GetTileMap().GetTile(cellPos);

        if (tile == null)
        {
            Debug.Log("tile is Null");
            return;
        }

        // 동일한 위치라면 반환한다.
        if (_prevPos.x == cellPos.x && _prevPos.y == cellPos.y)
        {
            return;
        }

        // TODO : 테스트를 위해 지나가는 타일의 색상을 자신의 색상으로 변환한다.
        
        // 자신의 영역에 들어왔다면
        if(GetTileMap().GetColor(cellPos) == _areaColor)
        {
            // 이전 위치가 영역 안에 위치한다면
            if (isInMoveTileList(new Vector3Int(_prevPos.x, _prevPos.y, _prevPos.z)) == false)
            {
                UpdatePrevPos(cellPos);
                UpdateStartPos(cellPos);
                return;
            }
            // 이전 위치가 영역 안이 아니라면
            else
            {
                // 색을 칠해준다.
                ChangeMoveLIstTile(cellPos);
                FillArea(cellPos);
                _playInfo.ClearMoveTileList();

                UpdatePrevPos(cellPos);
                return;
            }
        }
        // MoveTile인 경우
        else if(GetTileMap().GetColor(cellPos) == _moveColor)
        {


            return;
        }
        // 그 외의 타일인 경우
        else
        {
            Color color = _tilemap.GetColor(cellPos);
            _playInfo.AddMoveTileList(cellPos, color);
            UpdateTileColorToMove(new int[3] {cellPos.x,cellPos.y,cellPos.z});

            // 현재 진행방향으로 한칸 더 갔을 때, 자신의 영역에 닿는 다면 채우기 시도
            Vector3Int nextPos = FindNextPos(cellPos);
            if(GetTileMap().GetColor(nextPos) == _areaColor)
            {
                ChangeMoveLIstTile(cellPos);
                FillArea(cellPos);
                _playInfo.ClearMoveTileList();
            }

            UpdatePrevPos(cellPos);
            return;
        }
    }


    private bool isInMoveTileList(Vector3Int cellPosition)
    {
        List<Vector2> list = _playInfo.GetMoveTileList();

        foreach (Vector2 pos in list)
        {
            // 영역 밖에서 이동 중인 경로를 다시 지나간 경우
            if (pos.x == cellPosition.x && pos.y == cellPosition.y)
            {
                return true;
            }
        }
        return false;
    }


    // RPC 함수들
    [PunRPC]
    private void UpdateTileColorToMoveRPC(int[] pos)
    {
        Vector3Int cellPos = new Vector3Int(pos[0], pos[1], pos[2]);
        GetTileMap().SetTileFlags(cellPos, TileFlags.None);
        GetTileMap().SetColor(cellPos, new Color(_moveColor[0], _moveColor[1], _moveColor[2], _moveColor[3]));
    }

    [PunRPC]
    private void UpdateTileColorToAreaRPC(int[] pos)
    {
        Vector3Int cellPos = new Vector3Int(pos[0], pos[1], pos[2]);
        GetTileMap().SetTileFlags(cellPos, TileFlags.None);
        GetTileMap().SetColor(cellPos, new Color(_areaColor[0], _areaColor[1], _areaColor[2], _areaColor[3]));
    }

    // Update Tile Color Functions
    private void UpdateTileColorToMove(int[] pos)
    {
        _pv.RPC("UpdateTileColorToMoveRPC", RpcTarget.All, pos);
    }

    private void UpdateTileColorToArea(int[] pos)
    {
        _pv.RPC("UpdateTileColorToAreaRPC", RpcTarget.All, pos);
    }

    [PunRPC]
    private void UpdateTileColorToThisColor(int[] pos, float[] colors)
    {
        Vector3Int cellPos = new Vector3Int(pos[0], pos[1], pos[2]);
        GetTileMap().SetTileFlags(cellPos, TileFlags.None);
        GetTileMap().SetColor(cellPos, new Color(colors[0], colors[1], colors[2], colors[3]));
    }

    [PunRPC]
    private void UpdateTileColor(int[] pos, string colorName)
    {
        // TODO : Color 종류별로 함수를 만들어서 실행 속도 올려야 할듯

        Color color = new Color();
        if(colorName == "Move")
        {
            color = _moveColor;
        }
        else if(colorName == "Area")
        {
            color = _areaColor;
        }
        else if(colorName == "Border")
        {
            color = _borderColor;
        }
        Vector3Int cellPos = new Vector3Int(pos[0], pos[1], pos[2]);
        GetTileMap().SetTileFlags(cellPos, TileFlags.None);
        GetTileMap().SetColor(cellPos, new Color(color[0], color[1], color[2], color[3]));
    }

    // Color 관련 함수들
    public void SetMoveColor(float[] colorArray)
    {
        Color color = new Color(colorArray[0], colorArray[1], colorArray[2]);
        color.a = colorArray[3];
        _moveColor = color;
    }
    public void SetAreaColor(float[] colorArray)
    {
        Color color = new Color(colorArray[0], colorArray[1], colorArray[2]);
        color.a = colorArray[3];
        _areaColor = color;
    }
    public void SetBorderColor(float[] colorArray)
    {
        Color color = new Color((byte)colorArray[0], (byte)colorArray[1], (byte)colorArray[2]);
        color.a = colorArray[3];

        _borderColor = color;
    }

    // Pos 관련 함수들
    private void UpdateStartPos(Vector3Int cellPos)
    {
        _startPosition = cellPos;
    }

    private void UpdatePrevPos(Vector3Int cellPos)
    {
        _prevPos = cellPos;
    }

    private Vector3Int FindNextPos(Vector3Int cellPosition)
    {
        int dx = 0;
        int dy = 0;

        Vector2 dir = _playInfo.GetCurDir();

        if (dir == Vector2.up)
        {
            dx = 0; dy = 1;
        }
        else if (dir == Vector2.down)
        {
            dx = 0; dy = -1;
        }
        else if (dir == Vector2.left)
        {
            dx = -1; dy = 0;
        }
        else if (dir == Vector2.right)
        {
            dx = 1; dy = 0;
        }

        Vector3Int nextPos = new Vector3Int(cellPosition.x + dx, cellPosition.y + dy, cellPosition.z);

        return nextPos;
    }
    private List<Vector3Int> GetFillAreaPos(Vector3Int cellPos)
    {
        List<Vector2> list = new List<Vector2>();
        list = _playInfo.GetMoveTileList();

        // 처음 시작 위치를 리스트에 추가
        Vector2 pos = new Vector2(cellPos.x,cellPos.y);
        list.Add(pos);

        List<Vector3Int> FillAreaPosList = new List<Vector3Int>();

        for (int i = list.Count - 2; i > 0; i--)
        {
            bool result;

            if (list[i + 1].y == list[i].y)
            {
                result = PerformTilemapSearch(new Vector3Int((int)list[i].x, (int)list[i].y + 1, cellPos.z));
                if (result == true)
                {
                    FillAreaPosList.Add(new Vector3Int((int)list[i].x, (int)list[i].y + 1, cellPos.z));
                }

                result = PerformTilemapSearch(new Vector3Int((int)list[i].x, (int)list[i].y - 1, cellPos.z));
                if (result == true)
                {
                    FillAreaPosList.Add(new Vector3Int((int)list[i].x, (int)list[i].y - 1, cellPos.z));
                }
            }
            else
            {
                result = PerformTilemapSearch(new Vector3Int((int)list[i].x + 1, (int)list[i].y, cellPos.z));
                if (result == true)
                {
                    FillAreaPosList.Add(new Vector3Int((int)list[i].x + 1, (int)list[i].y, cellPos.z));
                }

                result = PerformTilemapSearch(new Vector3Int((int)list[i].x - 1, (int)list[i].y, cellPos.z));
                if (result == true)
                {
                    FillAreaPosList.Add(new Vector3Int((int)list[i].x - 1, (int)list[i].y, cellPos.z));
                }
            }
        }

        return FillAreaPosList;
    }


    private bool IsInit()
    {
        // 초기화가 이뤄졌다면 _moveColor와 _borderColor가 같을리 없으므로 return
        return _moveColor == _borderColor;
    }

    // 타일 관련 함수들
    private void ChangeMoveLIstTile(Vector3Int celPos)
    {
        List<Vector2> moveList = _playInfo.GetMoveTileList();

        if (moveList.Count == 0)
        {
            return;
        }

        foreach (var move in moveList)
        {
            UpdateTileColorToArea(new int[3] { (int)move.x, (int)move.y, celPos.z });
        }

    }

    private void FillArea(Vector3Int cellPos)
    {
        List<Vector3Int> FillAreaList = new List<Vector3Int>();

        FillAreaList = GetFillAreaPos(cellPos);

        if (FillAreaList.Count > 0)
        {
            foreach (Vector3Int fillArea in FillAreaList)
            {
                FloodFillArea(fillArea);
            }
        }
        else
        {
            Debug.Log("FillAreaList Count is 0");
        }
    }

    private void FloodFillArea(Vector3Int cellPos)
    {
        // 해당 타일이 passedColor인지 확인
        Color color1 = _tilemap.GetColor(cellPos);

        if (color1 == _areaColor || color1 == _borderColor)
        {
            Debug.Log("return");
            return;
        }

        UpdateTileColorToArea(new int[3] {cellPos.x,cellPos.y,cellPos.z});
        int[] dx = new int[4] { 0, 0, -1, 1 };
        int[] dy = new int[4] { 1, -1, 0, 0 };

        Queue<Vector3Int> Q = new Queue<Vector3Int>();
        Dictionary<Vector3Int, bool> Dict = new Dictionary<Vector3Int, bool>();

        Q.Enqueue(cellPos);
        Dict.Add(cellPos, true);

        while (Q.Count > 0)
        {
            Vector3Int pos = Q.Dequeue();
            UpdateTileColorToArea(new int[3] { pos.x, pos.y, pos.z });

            for (int i = 0; i < 4; i++)
            {
                Vector3Int vPos = new Vector3Int(pos.x + dx[i], pos.y + dy[i], pos.z);
                Color color = _tilemap.GetColor(vPos);

                if (color != _areaColor && color != _borderColor)
                {
                    if (Dict.ContainsKey(vPos) == false)
                    {
                        Q.Enqueue(vPos);
                        Dict.Add(vPos, true);
                    }
                }
            }
        }

    }

    private bool PerformTilemapSearch(Vector3Int startTilePosition)
    {
        Color startColor = _tilemap.GetColor(startTilePosition);
        if (startColor == _borderColor || startColor == _areaColor)
        {
            return false;
        }

        Queue<Vector3Int> queue; //
        queue = new Queue<Vector3Int>();
        queue.Enqueue(startTilePosition);

        Dictionary<Vector3Int, bool> Dict = new Dictionary<Vector3Int, bool>();
        Dict.Add(startTilePosition, true);

        while (queue.Count > 0)
        {
            Vector3Int currentPosition = queue.Dequeue();
            Color currentTileColor = _tilemap.GetColor(currentPosition);

            if (currentTileColor == _areaColor)
            {
                // 플레이어 타일을 찾았을 때
                
            }
            else if (currentTileColor == _borderColor)
            {
                // 테두리 타일을 만났을 때
                return false;
            }
            else
            {
                // 배경 타일일 경우 주변 타일들을 큐에 추가
                Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

                foreach (Vector3Int direction in directions)
                {
                    Vector3Int adjacentPosition = currentPosition + direction;
                    if (Dict.ContainsKey(adjacentPosition) == false && !queue.Contains(adjacentPosition))
                    {
                        queue.Enqueue(adjacentPosition);
                        Dict.Add(adjacentPosition, true);
                    }
                }
            }
        }

        // 테두리 타일을 만나지 못한 경우
        return true;
    }

    private Tilemap GetTileMap()
    {
        if(_tilemap != null)
        {
            return _tilemap;
        }

        Tilemap tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        if(tilemap != null)
        {
            _tilemap = tilemap;
            return _tilemap;
        }
        else
        {
            Debug.Log("Tile Map is Null");
            return null;
        }
    }
}

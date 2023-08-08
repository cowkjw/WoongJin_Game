using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


// 게임 플레이 중 필요한 정보를 꺼내쓸 수 있는 스크립트
// 캐릭터 프리팹에 컴포넌트를 추가하여 게임 중의 정보를 저장한다.

public class SPlayInfo : MonoBehaviourPunCallbacks
{
    private Vector2 _curDir;                // 현재 플레이가 가는 방향
    private Vector2 _prevDir;               // 이전에 플레이어가 간 방향
    private Vector2 _prevprevDir;           // 이전의 이전에 플레이어가 간 방향

    public List<Vector2> _moveTileList;          // 플레이어가 자신의 영역 밖에서 이동하면서 색상을 바꾼 타일들의 리스트
    public List<Color> _moveTileColorList;     // moveTileList가 지나가며 바꿨던 색상들의 리스트 

    private void Awake()
    {
        _curDir = Vector2.zero;
        _prevDir = Vector2.zero;
        _prevprevDir = Vector2.zero;

        _moveTileColorList = new List<Color>();
        _moveTileList = new List<Vector2>();
    }

    void Start()
    {

    }

    [PunRPC]
    public void SetCurDir(Vector2 dir)
    {
        if (dir == _curDir)
        {
            return;
        }

        SetPrevDIR(_curDir);
        _curDir = dir;
    }

    public void SetPrevDIR(Vector2 dir)
    {
        SetPrevPrevDIR(_prevDir);
        _prevDir = dir;
    }

    private void SetPrevPrevDIR(Vector2 dir)
    {
        _prevprevDir = dir;
    }

    public Vector2 GetCurDir()
    {
        return _curDir;
    }

    public Vector2 GetPrevDir()
    {
        return _prevDir;
    }

    public Vector2 GetPrevPrevDir()
    {
        return _prevprevDir;
    }

    public List<Color> GetMoveTileColorList()
    {
        return _moveTileColorList;
    }

    public List<Vector2> GetMoveTileList()
    {
        return _moveTileList;
    }
    public void AddMoveTileColorList(Vector3Int Pos, Color color)
    {
        _moveTileColorList.Add(color);
    }

    public void AddMoveTileList(Vector3Int cellPosition, Color color)
    {
        Vector2 pos = new Vector2(cellPosition.x, cellPosition.y);

        if (_moveTileList.Count > 0)
        {
            Vector2 prevPos = _moveTileList[_moveTileList.Count - 1];
            if (prevPos.x == pos.x && prevPos.y == pos.y)
            {
                return;
            }
        }
        _moveTileList.Add(pos);
        AddMoveTileColorList(cellPosition, color);
    }

    public void ClearMoveTileList()
    {
        _moveTileList.Clear();
        _moveTileColorList.Clear();
    }
}

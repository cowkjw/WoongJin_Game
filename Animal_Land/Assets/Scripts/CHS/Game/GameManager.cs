using Photon.Pun;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.TextCore.Text;
using Unity.VisualScripting;

// 점수와 게임 오버 여부, 게임 UI를 관리하는 게임 매니저
public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    // 외부에서 싱글톤 오브젝트를 가져올때 사용할 프로퍼티
    public static GameManager   instance;
    private static GameManager  _instance; // 싱글톤이 할당될 static 변수
    public Tilemap              tileMap;

    public bool                 isGameover { get; private set; } // 게임 오버 상태

    [SerializeField]
    GameObject[]                _SpawnPos = new GameObject[4];

    GameObject _localUser;

    // 주기적으로 자동 실행되는, 동기화 메서드
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 로컬 오브젝트라면 쓰기 부분이 실행됨
        if (stream.IsWriting)
        {

        }
        else
        {

        }
    }

    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }

        // TODO : 캐릭터가 만들어지는 동안 입력을 받지 못하게 한다.


        // 게임 시작과 동시에 플레이어가 될 게임 오브젝트를 생성
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        GameObject user = SpawnUser(playerCount);
        _localUser = user;

        // 유저 정보를 업데이트
        UpdateUser(user, playerCount);
    }

    private void Start()
    {

    }

    private void Init()
    {
        
    }

    private GameObject SpawnUser(int playerCount)
    {
        // 생성할 랜덤 위치 지정
        Vector2 SpawnPos = GetSpawnPos(playerCount);

        // 플레이어 생성 순서에 따라 어떤 캐릭터의 정보를 들고올지 선택

        // 네트워크 상의 모든 클라이언트들에서 생성 실행
        // 단, 해당 게임 오브젝트의 주도권은, 생성 메서드를 직접 실행한 클라이언트에게 있음
        GameObject Object = PhotonNetwork.Instantiate("Prefabs/Character", SpawnPos, Quaternion.identity, 0);

        return Object;
    }

    private void UpdateUser(GameObject user, int playerCount)
    {
        // 각자 클라이언트에서만 업데이트되어 다른 클라이언트로 정보를 뿌린다.
        PhotonView pv = user.GetComponent<PhotonView>();
        Character character = user.GetComponent<Character>();
        if(pv.IsMine == true)
        {
            // 조이스틱 연결
            SetupJoyStick(user);

            // 캐릭터 정보 업데이트
            character.SetupCharacter(playerCount);
        }
    }

    private void SetupJoyStick(GameObject user)
    {
        GameObject JoyStick = GameObject.FindGameObjectWithTag("GameController");

        if (user.GetComponent<PhotonView>().IsMine == true)
        {
            JoyStick.GetComponent<JoyStick>().Character = user;
        }
    }

    private Vector2 GetSpawnPos(int playerCount)
    {
        Vector2 spawnPos = Vector2.zero;
        // TODO : Player의 Count에 따라 알맞는 위치에 생성

        spawnPos = _SpawnPos[playerCount - 1].transform.position;

        Debug.Log(spawnPos);

        return spawnPos;
    }

    // 점수를 추가하고 UI 갱신
    public void AddScore(int newScore)
    {
        // 게임 오버가 아닌 상태에서만 점수 증가 가능
        if (!isGameover)
        {

        }
    }

    // 게임 오버 처리
    public void EndGame()
    {
        // 게임 오버 상태를 참으로 변경
        isGameover = true;
    }

    // 키보드 입력을 감지하고 룸을 나가게 함
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    // 룸을 나갈때 자동 실행되는 메서드
    public override void OnLeftRoom()
    {
        // 룸을 나가면 로비 씬으로 돌아감
        SceneManager.LoadScene("Lobby");
    }

    private UserManager GetUserManager()
    {
        GameObject Object = GameObject.Find("UserManager");
        if(Object != null)
        {
            UserManager userManager = Object.GetComponent<UserManager>();
            if(userManager != null)
            {
                return userManager;
            }
        }

        Debug.Log("User Manager is Null");
        return null;
    }
}
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.TextCore.Text;
using Unity.VisualScripting;
using TMPro;

// 점수와 게임 오버 여부, 게임 UI를 관리하는 게임 매니저
public class SGameManager : MonoBehaviour
{
    // 외부에서 싱글톤 오브젝트를 가져올때 사용할 프로퍼티
    public static GameManager instance;
    private static GameManager _instance; // 싱글톤이 할당될 static 변수
    public Tilemap tileMap;

    public bool isGameover { get; private set; } // 게임 오버 상태

    [SerializeField]
    GameObject[] _SpawnPos = new GameObject[4];

    GameObject _localUser;

    [Header("버튼")]
    public TextMeshProUGUI _playButton;

    private void Awake()
    {
        // 게임 시작과 동시에 플레이어가 될 게임 오브젝트를 생성
        int playerCharacterNum = 1;
        GameObject user = SetUser(playerCharacterNum);
        _localUser = user;

        // 유저 정보를 업데이트
        UpdateUser(user, playerCharacterNum);
    }

    private void Start()
    {

    }

    private void Init()
    {

    }

    private GameObject SetUser(int playerCount)
    {
        // 생성할 랜덤 위치 지정
        Vector2 SpawnPos = GetSpawnPos(playerCount);

        // 플레이어 생성 순서에 따라 어떤 캐릭터의 정보를 들고올지 선택

        // 네트워크 상의 모든 클라이언트들에서 생성 실행
        // 단, 해당 게임 오브젝트의 주도권은, 생성 메서드를 직접 실행한 클라이언트에게 있음
        GameObject Object = GameObject.FindGameObjectWithTag("Player").gameObject;

        return Object;
    }

    private void UpdateUser(GameObject user, int playerCount)
    {
        // 각자 클라이언트에서만 업데이트되어 다른 클라이언트로 정보를 뿌린다.
        SCharacter character = user.GetComponent<SCharacter>();

        // 조이스틱 연결
        SetupJoyStick(user);

        // 캐릭터 정보 업데이트
        character.SetupCharacter(playerCount);
    }

    private void SetupJoyStick(GameObject user)
    {
        GameObject JoyStick = GameObject.FindGameObjectWithTag("GameController");

        JoyStick.GetComponent<SJoyStick>().Character = user;
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
            //TODO : 설정 창 키기

        }
    }

    private UserManager GetUserManager()
    {
        GameObject Object = GameObject.Find("UserManager");
        if (Object != null)
        {
            UserManager userManager = Object.GetComponent<UserManager>();
            if (userManager != null)
            {
                return userManager;
            }
        }

        Debug.Log("User Manager is Null");
        return null;
    }

    public void StartGameBtn()
    {
        StartGame();
    }

    public void DropOutAllPlayerBtn()
    {

    }

    void DropOutAllPlayer()
    {

    }

    void StartGame()
    {
        SJoyStick JoyStick = GameObject.FindGameObjectWithTag("GameController").GetComponent<SJoyStick>();
        if (JoyStick != null)
        {
            if (JoyStick.IsGameStart() == true)
            {
                return;
            }
            else
            {
                JoyStick.SetIsGameStart(true);
                _playButton.text = "Game is Already Start";
            }
        }
    }
}
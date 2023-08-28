using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.TextCore.Text;
using Unity.VisualScripting;
using TMPro;
using Mono.Cecil.Cil;

// 점수와 게임 오버 여부, 게임 UI를 관리하는 게임 매니저
public class SGameManager : MonoBehaviour
{
    // 외부에서 싱글톤 오브젝트를 가져올때 사용할 프로퍼티
    public static GameManager instance;
    private static GameManager _instance; // 싱글톤이 할당될 static 변수
    public Tilemap _tileMap;
    public TilemapCollider2D _tileCollider;

    public bool isGameover { get; private set; } // 게임 오버 상태

    [SerializeField]
    GameObject[] _SpawnPos = new GameObject[4];
    GameObject _localUser;

    [Header("UI")]
    public TextMeshProUGUI _playButton;
    [SerializeField] private SGameUIManager UIManager;

    [Header("스코어")]
    [SerializeField] private int _areaScore;
    [SerializeField] private int _monsterScore;
    [SerializeField] private int _timeScore;
    [SerializeField] private int _solveScore;

    [Header("라운드 재화 획득 량")]
    [SerializeField] private int _money = 0;

    [Header("시간")]
    [SerializeField] private float _gameTime;
    [SerializeField] private float _maxGameTime;
    [SerializeField] private Slider _timeSlider;

    [Header("게임 세팅")]
    [SerializeField] private bool   _gameOver = false;
    [SerializeField] private bool   _gameStop = false;
    [SerializeField] private float  _stageBonus = 1f;   // 스테이지 별 점수 배율

    [Header("아이템 사용 가능 여부")]
    [SerializeField] private bool _canUseHpItem = false;
    [SerializeField] private bool _canUseSpeedItem = false;
    [SerializeField] private bool _canUseGaugeItem = false;

    [Header("아이템 효과 수치")]
    [SerializeField] private float _hpItemValue;
    [SerializeField] private float _speedItemValue;
    [SerializeField] private float _speedtimeValue;  // 지속 시간
    [SerializeField] private float _gaugeItemValue;
    [SerializeField] private float _timeItemValue;

    [Header("문제 풀이 관련")]
    [SerializeField] private float _solveGaugeValue;

    DataManager _dataManager;

    [Header("플레이어 오브젝트")]
    [SerializeField] private GameObject _player;
    private string _character;
    

    private void Awake()
    {
        // 게임 시작과 동시에 플레이어가 될 게임 오브젝트를 생성
        // TODO : 가져올 캐릭터의 종류 및 정보를 받아온다.

        int playerCharacterNum = 1;
        _dataManager = GameObject.Find("@DataManager").GetComponent<DataManager>();
        if(_dataManager !=null)
        {
            playerCharacterNum = Init();
        }

        GameObject user = SetUser(playerCharacterNum);
        _localUser = user;

        // 유저 정보를 업데이트
        UpdateUser(user, playerCharacterNum, _character);

        // 몬스터에 정보를 업데이트
        UpdateMonsters();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //TODO : 설정 창 키기
            UIManager.OpenSettingScreen();
        }

        UpdateTime();
    }

    private void Start()
    {
        StartGame();
    }

    private int Init()
    {
        // 아이템 갱신
        if (_dataManager.PlayerStat.HP > 0)
        {
            _canUseHpItem = true;
        }
        if(_dataManager.PlayerStat.Speed >0)
        {
            _canUseSpeedItem = true;
        }
        if (_dataManager.PlayerStat.Energy >0)
        {
            _canUseGaugeItem = true;
        }

        // 플레이어 정보 갱신
        int playerNum = 0;
        _character = _dataManager.PlayerData.Character;
        if (_character == "Bird")
        {
            playerNum = 1;
        }
        else if (_character == "Dog")
        {
            playerNum = 2;
        }
        else if (_character == "Frog")
        {
            playerNum = 3;
        }
        else
        {
            playerNum = 4;
        }

        return playerNum;
    }

    void UpdateTime()
    {
        if(_gameStop || _gameOver)
        {
            return;
        }

        // TODO : 게임 제한시간 구현
        if(_gameTime < _maxGameTime)
        {
            _gameTime += Time.deltaTime;

            // TODO : 제한시간이 넘어가면 게임 종료
            if (_gameTime + Time.deltaTime >= _maxGameTime)
            {
                EndGame(false);
            }
        }

        // TODO : 제한시간 UI 업데이트
        if(_timeSlider != null)
        {
            float value = _gameTime / _maxGameTime;
            UIManager.UpdateTime(1f - value);
            // _timeSlider.value =  1f - value;
        }
    }

    private void UpdateMonsters()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monster in monsters)
        {
            monster.GetComponent<SMonsterMove>().Init();
        }
    }

    private GameObject SetUser(int playerCount)
    {
        // 생성할 랜덤 위치 지정
        Vector2 SpawnPos = GetSpawnPos(playerCount);

        GameObject Object = GameObject.FindGameObjectWithTag("Player").gameObject;
        return Object;
    }

    private void UpdateUser(GameObject user, int playerCount, string characterType)
    {
        // 각자 클라이언트에서만 업데이트되어 다른 클라이언트로 정보를 뿌린다.
        SCharacter character = user.GetComponent<SCharacter>();

        // 조이스틱 연결
        SetupJoyStick(user);

        // 캐릭터 정보 업데이트
        character.SetupCharacter(playerCount, characterType);
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
    public void AddMonsterScore(int score)
    {
        // 게임 오버가 아닌 상태에서만 점수 증가 가능
        if (!isGameover)
        {
            _monsterScore += score;
        }
    }

    public void SetAreaScore(int count)
    {
        // 점유한 땅 %
        float wholeArea = ((46 + 28) * (26 + 16));

        if (!isGameover)
        {
            _areaScore = (int)(count / wholeArea) * 100;
        }

        // 100% 다 채운 경우
        if(count == wholeArea)
        {
            EndGame(true);
        }
    }

    public void AddSolveScore(int count = 1)
    {
        _solveScore += count * 5;
    }

    public int GetTotalScore()
    {
        return (int)((_areaScore + _timeScore + _solveScore) * _stageBonus);
    }

    // 게임 오버 처리
    public void EndGame(bool gameClear)
    {
        // 게임 오버 상태를 참으로 변경
        isGameover = gameClear;

        // 게임 진행 정지
        StopGame();

        // TODO : 게임 종료 패널과 함께 결과 창 출력 (획득 점수, 재화 등을 넘겨주기)
        if (UIManager == null)
        {
            return;
        }

        // TODO : _timeScore 갱신
        _timeScore = (int)(_maxGameTime - _gameTime);

        // 획득 재화 계산
        _money = GetTotalScore() / 5;   
        
        UIManager.OpenResultPanel(isGameover, GetTotalScore(), _money, _gameTime);

        // DataManager에 값 저장 (재화, 스코어, 클리어 정보 등)
        int plusScore = GetTotalScore();
        if (DatabaseManager.Instance != null)
        {
            DatabaseManager.Instance.WriteDB(plusScore);
        }

        // 획득 재화 계산
        _money = GetTotalScore() / 5;
        int newGold = _dataManager.PlayerData.Gold + _money;
        _dataManager.PlayerData.Gold = newGold;
        DataManager.Instance.SavePlayerData(); // 플레이어 데이터 저장
    }

    public void TimeOver(float value)
    {
        if(value <= 0)
        {
            // TImeSlider의 FillArea를 비활성화


            EndGame(false);
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

    public void StartGame()
    {
        // TODO : 플레이어 이동
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.GetComponent<SCharacter>().StartPlayer();
        }

        // TODO : 몬스터 이동 
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monster in monsters)
        {
            if (monster != null && monster.GetComponent<SMonster>() != null)
            {
                monster.GetComponent<SMonster>().StartMonster();
            }
        }

        _tileCollider.enabled = true;

        // 시간초 재개
        _gameStop = false;
    }

    public void StopGame()
    {
        // 플레이어 정지 (Move)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player != null)
        {
            player.GetComponent<SCharacter>().StopPlayer();
        }

        // Monster 정지 (Move, Attack 관련)
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach(GameObject monster in monsters)
        {
            if(monster != null && monster.GetComponent<SMonster>() != null)
            {
                monster.GetComponent<SMonster>().StopMonster();
            }
        }

        _tileCollider.enabled = false;

        // 시간초 정지
        _gameStop = true;
    }

    public int CalAreaScore()
    {
        int result = 0;
        int count = 0;

        // 플레이어 AreaColor
        Color playerAreaColor = _localUser.GetComponent<STileColorChange>().GetAreaColor();

        // -45, 15 / 28 , -26
        // (46 + 28) * (26 + 16)

        for(int i = -45; i <= 28; i++)
        {
            for(int j = -26; j <= 15; j++)
            {
                if (_tileMap.GetColor(new Vector3Int(i, j, 0)) == playerAreaColor)
                    count++;
            }
        }

        // AreaScore 갱신
        SetAreaScore(count);

        // 반환
        return result;
    }

    public void UseItemHp()
    {
        // 사용 가능 여부 확인
        if(_canUseHpItem == false)
        {
            return;
        }
        _canUseHpItem = false;

        // Hp 회복
        _player.GetComponent<SCharacterHp>().Heal(_hpItemValue);

        // 효과음 출력
        UIManager.GetComponent<SoundManager>().PlayEffect(Effect.Button);

        // TODO : 아이템 비활성화
        
    }

    public void UseItemSpeed()
    {
        // 사용 가능 여부 확인
        if (_canUseSpeedItem == false)
        {
            return;
        }
        _canUseSpeedItem = false;

        SJoyStick JoyStick = GameObject.FindGameObjectWithTag("GameController").GetComponent<SJoyStick>();
        // 스피드 상승
        JoyStick.SpeedUp(_speedItemValue);

        // 일정 시간 이후에 줄어든다.
        Invoke("SpeedDown", _speedtimeValue);

        // 효과음 출력
        UIManager.GetComponent<SoundManager>().PlayEffect(Effect.Button);

        // TODO : 아이템 비활성화


    }

    public void SpeedDown()
    {
        SJoyStick JoyStick = GameObject.FindGameObjectWithTag("GameController").GetComponent<SJoyStick>();
        JoyStick.SpeedDown();
    }

    public void UseItemGauge()
    {
        // 사용 가능 여부 확인
        if (_canUseGaugeItem == false)
        {
            return;
        }
        _canUseGaugeItem = false;

        // 게이지 회복
        float sliderValue = _player.GetComponent<SCharacter>().AddMoveGauge(_gaugeItemValue);

        // UI 갱신
        UIManager.AddMoveGauge(sliderValue);

        // 효과음 출력
        UIManager.GetComponent<SoundManager>().PlayEffect(Effect.Button);

        // TODO : 아이템 비활성화

    }

    public void Solve()
    {
        // 게이지 회복
        float sliderValue = _player.GetComponent<SCharacter>().AddMoveGauge(_solveGaugeValue);

        // UI 갱신
        UIManager.AddMoveGauge(sliderValue);
    }

    public void DropItemHp()
    {
        _player.GetComponent<SCharacterHp>().Heal(_hpItemValue);
    }

    public void DropItemTime()
    {
        _gameTime -= _timeItemValue;
        if(_gameTime < 0)
        {
            _gameTime = 0;
        }
        // TODO : 제한시간 UI 업데이트
        if (_timeSlider != null)
        {
            float value = _gameTime / _maxGameTime;
            UIManager.UpdateTime(1f - value);
            // _timeSlider.value =  1f - value;
        }

    }
    public void DropItemGauge()
    {
        // 게이지 회복
        float sliderValue = _player.GetComponent<SCharacter>().AddMoveGauge(_gaugeItemValue);

        // UI 갱신
        UIManager.AddMoveGauge(sliderValue);
    }
    public void DropItemSpeed()
    {
        SJoyStick JoyStick = GameObject.FindGameObjectWithTag("GameController").GetComponent<SJoyStick>();
        // 스피드 상승
        JoyStick.SpeedUp(_speedItemValue);

        // 일정 시간 이후에 줄어든다.
        Invoke("SpeedDown", _speedtimeValue);
    }
}
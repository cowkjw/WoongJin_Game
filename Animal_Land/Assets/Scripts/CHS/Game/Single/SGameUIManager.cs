using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SGameUIManager : MonoBehaviour
{
    [Header("화면")]
    [SerializeField] private GameObject _resultScreen;
    [SerializeField] private GameObject _solveScreen;
    [SerializeField] private GameObject _settingScreen;

    [Header("슬라이더")]
    [SerializeField] private Slider _timeSlider;
    [SerializeField] private Slider _moveGaugeSlider;
    [SerializeField] private Slider _hpSlider;

    [Header("버튼")]
    [SerializeField] private GameObject _hpItems;
    [SerializeField] private GameObject _speedItems;
    [SerializeField] private GameObject _gaugeItems;

    [Header("테스트 값")]
    [SerializeField] private float _addGaugeValue;

    private DrawLine _line;
    private SGameManager _gameManager;
    private SoundManager _soundManager;
    private void Awake()
    {
        _line = GetComponent<DrawLine>();
        _gameManager = GameObject.Find("GameManager").GetComponent<SGameManager>();
        _soundManager = GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenSolveScreen()
    {
        // Solve Screen 활성화
        _solveScreen.SetActive(true);

        _gameManager.StopObject();

        _soundManager.PlayEffect(Effect.Button);
    }

    public void CloseSolveScreen()
    {
        // Solve Screen 비활성화
        _solveScreen.SetActive(false);

        _gameManager.StartObject();

        _line.ClearLine();

        _soundManager.PlayEffect(Effect.Back);
    }

    public void OpenSettingScreen()
    {
        _settingScreen.SetActive(true);

        _gameManager.StopGame();

        _soundManager.PlayEffect(Effect.Button);
    }

    public void CloseSettingScreen()
    {
        _settingScreen.SetActive(false);

        _gameManager.StartGame();

        _soundManager.PlayEffect(Effect.Back);
    }

    public void OpenResultPanel(bool gameClear, int score, int money, float gameTime)
    {
        // Result Screen 활성화
        _resultScreen.SetActive(true);

        TextMeshProUGUI Score = _resultScreen.transform.GetChild(7).gameObject.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI Gold = _resultScreen.transform.GetChild(8).gameObject.GetComponent<TextMeshProUGUI>();

        Score.text = score.ToString();
        Gold.text = money.ToString();

        // 효과음 출력
        _soundManager.PlayEffect(Effect.Gameover);
    }
    
    public void UpdateHp(float value)
    {
        _hpSlider.value = value;
    }

    public void UpdateGauge(float value)
    {
        _moveGaugeSlider.value = value;
    }

    public void UpdateTime(float value)
    {
        _timeSlider.value = value;
    }

    public void TimeOver()
    {
        _timeSlider.gameObject.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void AddMoveGauge(float val)
    {
        float value = _moveGaugeSlider.value + val;

        if(value > 1f)
        {
            value = 1f;
        }

        _moveGaugeSlider.value = value;
    }

    public void BackToLobby()
    {
        // 로비로 이동
        SceneManager.LoadScene("Lobby 1");

        _soundManager.PlayEffect(Effect.Button);
    }

    public void Restart()
    {
        // 재시작
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);

        _soundManager.PlayEffect(Effect.Button);
    }

    public void Resume()
    {
        // 세팅 패널 비활성화
        CloseSettingScreen();

        _soundManager.PlayEffect(Effect.Back);
    }

    public void ActiveButton(string name, bool value)
    {
        if (name == "Hp")
        {
            if (value == false)
            {
                _hpItems.GetComponent<Image>().color = Color.gray;
            }
            else
            {
                _hpItems.GetComponent<Image>().color = Color.white;
            }
        }
        else if (name == "Gauge")
        {
            if (value == false)
            {
                _gaugeItems.GetComponent<Image>().color = Color.gray;
            }
            else
            {
                _gaugeItems.GetComponent<Image>().color = Color.white;
            }
        }
        else if (name == "Speed")
        {
            if (value == false)
            {
                _speedItems.GetComponent<Image>().color = Color.gray;
            }
            else
            {
                _speedItems.GetComponent<Image>().color = Color.white;
            }
        }
    }
}


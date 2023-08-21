using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SGameUIManager : MonoBehaviour
{
    [Header("화면")]
    [SerializeField] private GameObject _resultScreen;
    [SerializeField] private GameObject _solveScreen;

    [Header("슬라이더")]
    [SerializeField] private Slider _timeSlider;
    [SerializeField] private Slider _moveGaugeSlider;
    [SerializeField] private Slider _hpSlider;

    [Header("테스트 값")]
    [SerializeField] private float _addGaugeValue;

    private DrawLine _line;
    private SGameManager _gameManager;

    private void Awake()
    {
        _line = GetComponent<DrawLine>();
        _gameManager = GameObject.Find("GameManager").GetComponent<SGameManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenSolveScreen()
    {
        // Solve Screen 활성화
        _solveScreen.SetActive(true);

        _gameManager.StopGame();
    }

    public void CloseSolveScreen()
    {
        // Solve Screen 비활성화
        _solveScreen.SetActive(false);

        _gameManager.StartGame();
        _line.ClearLIne();
    }

    public void OpenResultPanel(bool gameClear, int score, int money, float gameTime)
    {
        // Result Screen 활성화
        _resultScreen.SetActive(true);
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

    public void AddMoveGauge()
    {
        float value = _moveGaugeSlider.value + _addGaugeValue;

        if(value > 1f)
        {
            value = 1f;
        }

        _moveGaugeSlider.value = value;
    }
}

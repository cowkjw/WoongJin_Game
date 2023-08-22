using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WjChallenge;

public enum CurrentStatus { WAITING, DIAGNOSIS, LEARNING }
public class WJ_Sample : MonoBehaviour
{
    [SerializeField] WJ_Connector wj_conn;
    [SerializeField] CurrentStatus currentStatus;
    public CurrentStatus CurrentStatus => currentStatus; // 프로퍼티

    [Header("Panels")]
    [SerializeField] GameObject panel_diag_chooseDiff;  //난이도 선택 패널
    [SerializeField] GameObject panel_question;         //문제 패널(진단,학습)

    [SerializeField] Text textDescription;        //문제 설명 텍스트
    [SerializeField] TEXDraw textEquation;           //문제 텍스트(TextDraw로 변경 했음)
    [SerializeField] Button[] btAnsr = new Button[4]; //정답 버튼들
    TEXDraw[] textAnsr;                  //정답 버튼들 텍스트(TextDraw로 변경 했음)

    [Header("Status")]
    int currentQuestionIndex;
    bool isSolvingQuestion;
    float questionSolveTime;

    [Header("For Debug")]
    [SerializeField] WJ_DisplayText wj_displayText;         //텍스트 표시용(필수X)
    [SerializeField] Button getLearningButton;      //문제 받아오기 버튼

    [Header("Test")]
    [SerializeField] Button StartButton;
    [SerializeField] Text QuestionTimer;
    [SerializeField] Text CorrectAnswerCount;
    [SerializeField] Slider TimerSlider;
    [SerializeField] GameObject Pannel;
    private const int SOLVE_TIME = 15; // 문제 풀이 시간
    private int _correctAnswerRemind; // 정답 인덱스 저장
    private int _diagnosisIndex; // 진단 인덱스
    private int _correctAnswers; // 맞은 정답 수 
    private IEnumerator _timerCoroutine;


    private void Awake()
    {
        textAnsr = new TEXDraw[btAnsr.Length]; // 버튼 숫자만큼 할당 TextDraw로 변경
        for (int i = 0; i < btAnsr.Length; ++i) // textAnsr에 text할당 

            textAnsr[i] = btAnsr[i].GetComponentInChildren<TEXDraw>();

        wj_displayText.SetState("대기중", "", "", "");
        _correctAnswerRemind = 0;
        _diagnosisIndex = 0;
        _correctAnswers = 0;
        _timerCoroutine = null;
    }


    private void OnEnable()
    {
        //Setup();
    }

    private void Setup()
    {
        Pannel?.SetActive(true);

        if (wj_conn != null && !wj_conn._needDiagnosis)
        {
            currentStatus = CurrentStatus.LEARNING;
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogError("Cannot find Connector");
#endif
        }
        switch (currentStatus)
        {
            case CurrentStatus.WAITING:
                panel_diag_chooseDiff.SetActive(true);
                if (wj_conn != null)
                {
                    wj_conn.onGetDiagnosis.AddListener(() => GetDiagnosis());
                    wj_conn.onGetLearning.AddListener(() => GetLearning(0));
                }
                break;
            case CurrentStatus.LEARNING:
                {
                    if (wj_conn != null)
                    {
                        SetLearning();
                        wj_conn.onGetLearning.AddListener(() => GetLearning(0));
                    }
                }
                break;
        }

      //  StartButton.gameObject.SetActive(false); // 게임시작 버튼 비활성화 (나중에 다시 주석 풀기)
    }

    private void Update()
    {
        if (isSolvingQuestion) // 문제 풀이 중일때 시간 계산
        {
            questionSolveTime += Time.deltaTime;
        }
    }

    /// <summary>
    /// 진단평가 문제 받아오기
    /// </summary>
    private void GetDiagnosis()
    {
        switch (wj_conn.cDiagnotics.data.prgsCd)
        {
            case "W":
                MakeQuestion(wj_conn.cDiagnotics.data.textCn,
                       wj_conn.cDiagnotics.data.qstCn,
                       wj_conn.cDiagnotics.data.qstCransr,
                       wj_conn.cDiagnotics.data.qstWransr);
                _diagnosisIndex++;
                if (_timerCoroutine == null) // 타이머 코루틴이 null이고 실행 중이 아닐 때 
                {
                    _timerCoroutine = SolveQuestionTimer();
#if UNITY_EDITOR
                    Debug.LogError("타이머 코루틴 시작");
#endif
                  //  if(!_timerCoroutine.MoveNext())
                    //{
                    //}
                       StartCoroutine(_timerCoroutine);
                }
                break;
            case "E":
                Debug.Log("진단평가 끝! 학습 단계로 넘어갑니다.");
                wj_displayText.SetState("진단평가 완료", "", "", "");
                currentStatus = CurrentStatus.LEARNING;
                panel_question.SetActive(false); // 새로운 문제를 받기 위해서 비활성화
                getLearningButton.gameObject.SetActive(true); // 진단이 끝나면 문제 풀이 버튼 활성화
                getLearningButton.interactable = true;
                _correctAnswers = 0;
                break;
        }
    }

    /// <summary>
    ///  n 번째 학습 문제 받아오기
    /// </summary>
    private void GetLearning(int _index)
    {
        if (_index == 0)
        {
            currentQuestionIndex = 0;
        }

        MakeQuestion(wj_conn.cLearnSet.data.qsts[_index].textCn,
                wj_conn.cLearnSet.data.qsts[_index].qstCn,
                wj_conn.cLearnSet.data.qsts[_index].qstCransr,
                wj_conn.cLearnSet.data.qsts[_index].qstWransr);

        
    }

    /// <summary>
    /// 받아온 데이터를 가지고 문제를 표시
    /// </summary>
    private void MakeQuestion(string textCn, string qstCn, string qstCransr, string qstWransr)
    {

        if (panel_diag_chooseDiff.activeSelf)
        {
            panel_diag_chooseDiff.SetActive(false);
        }

        panel_question.SetActive(true);

        // 첫번째 진단 문제이거나 첫번째 학습문제일때만
        bool isFirstQuestion = (_diagnosisIndex == 0 && currentStatus == CurrentStatus.DIAGNOSIS) ||
            (currentQuestionIndex == 0 && currentStatus == CurrentStatus.LEARNING);

        if (isFirstQuestion)
        {
            SetupQuestion(textCn, qstCn, qstCransr, qstWransr);
            _diagnosisIndex++;
        }
        else
        {
            StartCoroutine(ColoringCorrectAnswer(textCn, qstCn, qstCransr, qstWransr, 0.5f));
            _diagnosisIndex++;
        }

    }

    /// <summary>
    /// 답을 고르고 맞았는 지 체크
    /// </summary>
    public void SelectAnswer(int _idx = -1)
    {
        if (_idx == -1) // 시간초과 시
        {
            switch (currentStatus)
            {
                case CurrentStatus.DIAGNOSIS:

                    isSolvingQuestion = false;

                    wj_conn.Diagnosis_SelectAnswer("-1", "N", (int)(questionSolveTime * 1000));

                    wj_displayText.SetState("진단평가 중", "-1", "N", questionSolveTime + " 초");

                    questionSolveTime = 0;
                    break;

                case CurrentStatus.LEARNING:

                    isSolvingQuestion = false;
                    currentQuestionIndex++;

                    wj_conn.Learning_SelectAnswer(currentQuestionIndex, "-1", "N", (int)(questionSolveTime * 1000));

                    wj_displayText.SetState("문제풀이 중", "-1", "N", questionSolveTime + " 초");

                    if (currentQuestionIndex >= 2) // 문제 개수
                    {
                        panel_question.SetActive(false);
                        wj_displayText.SetState("문제풀이 완료", "", "", "");
                    }
                    else
                    {
                        GetLearning(currentQuestionIndex);
                    }
                    questionSolveTime = 0;
                    break;
            }
            return;
        }

        bool isCorrect = false;
        string ansrCwYn = "N";
       // StopAllCoroutines();

        switch (currentStatus)
        {
            case CurrentStatus.DIAGNOSIS:
                isCorrect = textAnsr[_idx].text.CompareTo(wj_conn.cDiagnotics.data.qstCransr) == 0 ? true : false;
                ansrCwYn = isCorrect ? "Y" : "N";

                isSolvingQuestion = false;

                wj_conn.Diagnosis_SelectAnswer(textAnsr[_idx].text, ansrCwYn, (int)(questionSolveTime * 1000));

                wj_displayText.SetState("진단평가 중", textAnsr[_idx].text, ansrCwYn, questionSolveTime + " 초");

                //panel_question.SetActive(false);  // 끄면 문제들 깜빡거림

                questionSolveTime = 0;
                break;

            case CurrentStatus.LEARNING:
                isCorrect = textAnsr[_idx].text.CompareTo(wj_conn.cLearnSet.data.qsts[currentQuestionIndex].qstCransr) == 0 ? true : false;
                ansrCwYn = isCorrect ? "Y" : "N";

                isSolvingQuestion = false;
                currentQuestionIndex++;

                wj_conn.Learning_SelectAnswer(currentQuestionIndex, textAnsr[_idx].text, ansrCwYn, (int)(questionSolveTime * 1000));

                wj_displayText.SetState("문제풀이 중", textAnsr[_idx].text, ansrCwYn, questionSolveTime + " 초");

                if (currentQuestionIndex >= 2) // 문제 개수
                {
                    panel_question.SetActive(false);
                    wj_displayText.SetState("문제풀이 완료", "", "", "");

                    if(_timerCoroutine != null)
                    {
                        StopCoroutine(_timerCoroutine);
                        _timerCoroutine = null;
                    }

                    getLearningButton.gameObject.SetActive(true); // 진단이 끝나면 문제 풀이 버튼 활성화
                    getLearningButton.interactable = true;
                }
                else
                {
                    GetLearning(currentQuestionIndex);
                }
                questionSolveTime = 0;
                break;
        }

        if (isCorrect)
        {
            _correctAnswers += 1;
        }
        CorrectAnswerCount.text = $"맞은 정답 수 : {_correctAnswers}";

    }

    public IEnumerator SolveQuestionTimer() // 문제풀이 타이머
    {

        float elapsedTime = SOLVE_TIME;
        while ((int)elapsedTime >= 0)
        {
            if (elapsedTime <= 0)
            {
                elapsedTime = 0;
            }
            QuestionTimer.text = $"남은 시간 : {(int)elapsedTime} 초";
            TimerSlider.value = elapsedTime;//슬라이더 값 설정
            elapsedTime -= Time.deltaTime;
#if UNITY_EDITOR
            Debug.LogError(elapsedTime);
#endif
            yield return null;// new WaitForSeconds(1f);
        }
        //SelectAnswer();
    }

    public void UpdateTimerSlider(float elapsedTime)
    {
        TimerSlider.value = elapsedTime;
    }

    void SetupQuestion(string textCn, string qstCn, string qstCransr, string qstWransr) // 문제 설정 함수
    {
        string correctAnswer;
        string[] wrongAnswers;

        textDescription.text = textCn;
        textEquation.text = qstCn;

        correctAnswer = qstCransr;
        wrongAnswers = qstWransr.Split(',');

        int ansrCount = Mathf.Clamp(wrongAnswers.Length, 0, 3) + 1;

        for (int i = 0; i < btAnsr.Length; i++)
        {
            if (i < ansrCount)
                btAnsr[i].gameObject.SetActive(true);
            else
                btAnsr[i].gameObject.SetActive(false);
        }

        int ansrIndex = Random.Range(0, ansrCount);
        _correctAnswerRemind = ansrIndex; // 정답 인덱스 저장해두기

        for (int i = 0, q = 0; i < ansrCount; ++i, ++q)
        {
            if (i == ansrIndex)
            {
                textAnsr[i].text = correctAnswer;
                --q;
            }
            else
                textAnsr[i].text = wrongAnswers[q];
        }
        isSolvingQuestion = true;
    }

    IEnumerator ColoringCorrectAnswer(string textCn, string qstCn, string qstCransr, string qstWransr, float delay) // 다음 문제 전 색상 표시
    {
        int prevIndex = _correctAnswerRemind; // 이전 인덱스 저장
        textAnsr[_correctAnswerRemind].color = new Color(1.0f, 0.0f, 0.0f); // 정답 인덱스 색상 변경

        yield return new WaitForSeconds(delay); // 딜레이

        textAnsr[prevIndex].color = new Color(0.0f, 0.0f, 0.0f); // 이전 정답 인덱스 다시 색상 되돌리기
        SetupQuestion(textCn, qstCn, qstCransr, qstWransr);
    }

    void SetLearning()
    {
        currentStatus = CurrentStatus.LEARNING;
        panel_question.SetActive(false); // 새로운 문제를 받기 위해서 비활성화
        getLearningButton.gameObject.SetActive(true); // 진단이 끝나면 문제 풀이 버튼 활성화
        getLearningButton.interactable = true;
        _correctAnswers = 0;
    }

    public void DisplayCurrentState(string state, string myAnswer, string isCorrect, string svTime)
    {
        if (wj_displayText == null) return;

        wj_displayText.SetState(state, myAnswer, isCorrect, svTime);
    }

    #region Unity ButtonEvent
    public void ButtonEvent_ChooseDifficulty(int a) // 진단평가 시 난이도 선택 버튼(On Click에서 설정해줘야함)
    {
        if (wj_conn._needDiagnosis)
        {
            currentStatus = CurrentStatus.DIAGNOSIS; // 현재 상태 진단으로 변경
            wj_conn.FirstRun_Diagnosis(a); // 난이도 전달 
        }
        else
        {
            SetLearning();
            wj_conn.Learning_GetQuestion();
            wj_displayText.SetState("문제풀이 중", "-", "-", "-");
            _correctAnswers = 0;
            CorrectAnswerCount.text = $"맞은 정답 수 : {_correctAnswers}";
        }
    }
    public void ButtonEvent_GetLearning() // 진단 평가 이후
    {
        SetLearning();

        wj_conn.Learning_GetQuestion();
        wj_displayText.SetState("문제풀이 중", "-", "-", "-");
        _correctAnswers = 0;
        CorrectAnswerCount.text = $"맞은 정답 수 : {_correctAnswers}";

        if (_timerCoroutine == null)
        {
            _timerCoroutine = SolveQuestionTimer();
        }
        StartCoroutine(_timerCoroutine);

         getLearningButton.gameObject.SetActive(false); // 진단이 끝나면 문제 풀이 버튼 활성화
         getLearningButton.interactable = false;
    }
    public void ButtonEvent_GameStart() // 추가
    {
        Setup(); // 게임 시작 시 셋팅
    }
    #endregion
}

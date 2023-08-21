using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LearningAPI : MonoBehaviour
{
    [SerializeField] WJ_Connector wj_conn;
    [SerializeField] CurrentStatus currentStatus;
    public CurrentStatus CurrentStatus => currentStatus; // 프로퍼티

    [Header("Panels")]
    [SerializeField] GameObject panel_question;         //문제 패널(진단,학습)

    [Header("Status")]
    int currentQuestionIndex;
    bool isSolvingQuestion;
    float questionSolveTime;

    [SerializeField] Text textDescription;        //문제 설명 텍스트
    [SerializeField] TEXDraw textEquation;           //문제 텍스트(TextDraw로 변경 했음)
    [SerializeField] Button[] btAnsr = new Button[4]; //정답 버튼들
    TEXDraw[] textAnsr;                  //정답 버튼들 텍스트(TextDraw로 변경 했음)

    [SerializeField] SGameUIManager uIManager;

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

     
        _correctAnswerRemind = 0;
        _diagnosisIndex = 0;
        _correctAnswers = 0;
        _timerCoroutine = null;
    }

    private void Update()
    {
        if (isSolvingQuestion) // 문제 풀이 중일때 시간 계산
        {
            questionSolveTime += Time.deltaTime;
        }
    }

    private void OnEnable()
    {
        Setup();
    }

    void Setup() // 문제 초기화
    {

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
    }

    void SetLearning()
    {
        currentStatus = CurrentStatus.LEARNING;
     //   panel_question.SetActive(false); // 새로운 문제를 받기 위해서 비활성화
        //getLearningButton.gameObject.SetActive(true); // 진단이 끝나면 문제 풀이 버튼 활성화
        //getLearningButton.interactable = true;
        _correctAnswers = 0;
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
                case CurrentStatus.LEARNING:

                    isSolvingQuestion = false;
                    currentQuestionIndex++;

                    wj_conn.Learning_SelectAnswer(currentQuestionIndex, "-1", "N", (int)(questionSolveTime * 1000));

                    if (currentQuestionIndex >= 2) // 문제 개수
                    {
                        //  panel_question.SetActive(false);

                        uIManager.CloseSolveScreen();
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
            case CurrentStatus.LEARNING:
                isCorrect = textAnsr[_idx].text.CompareTo(wj_conn.cLearnSet.data.qsts[currentQuestionIndex].qstCransr) == 0 ? true : false;
                ansrCwYn = isCorrect ? "Y" : "N";

                isSolvingQuestion = false;
                currentQuestionIndex++;

                wj_conn.Learning_SelectAnswer(currentQuestionIndex, textAnsr[_idx].text, ansrCwYn, (int)(questionSolveTime * 1000));

                if (currentQuestionIndex >= 2) // 문제 개수
                {
                   // panel_question.SetActive(false);
                    uIManager.CloseSolveScreen();

                    if (_timerCoroutine != null)
                    {
                        StopCoroutine(_timerCoroutine);
                        _timerCoroutine = null;
                    }

                    //getLearningButton.gameObject.SetActive(true); // 진단이 끝나면 문제 풀이 버튼 활성화
                    //getLearningButton.interactable = true;
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
    }


    void SetupQuestion(string textCn, string qstCn, string qstCransr, string qstWransr) // 문제 설정 함수
    {
        string correctAnswer;
        string[] wrongAnswers;

      //  textDescription.text = textCn;
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


    public void ButtonEvent_GetLearning()
    {
        SetLearning();

        wj_conn.Learning_GetQuestion();
        _correctAnswers = 0;

    }


}

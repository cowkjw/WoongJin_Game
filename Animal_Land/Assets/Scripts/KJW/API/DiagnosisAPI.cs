using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiagnosisAPI : MonoBehaviour
{
    [SerializeField] WJ_Connector wj_conn;
    [SerializeField] CurrentStatus currentStatus;
    public CurrentStatus CurrentStatus => currentStatus; // ������Ƽ

    [Header("Panels")]
    [SerializeField] GameObject panel_diag_chooseDiff;  //���̵� ���� �г�
    [SerializeField] GameObject panel_question;         //���� �г�(����,�н�)

    [Header("Status")]
    int currentQuestionIndex;
    bool isSolvingQuestion;
    float questionSolveTime;

    [SerializeField] Image ansCheckImage;
    [SerializeField] List<Sprite> checkList;
    [SerializeField] TEXDraw textEquation;           //���� �ؽ�Ʈ(TextDraw�� ���� ����)
    [SerializeField] TEXDraw textDescription;           //���� ����
    [SerializeField] Button[] btAnsr = new Button[4]; //���� ��ư��
    TEXDraw[] textAnsr;                  //���� ��ư�� �ؽ�Ʈ(TextDraw�� ���� ����)

    private const int SOLVE_TIME = 15; // ���� Ǯ�� �ð�
    private int _correctAnswerRemind; // ���� �ε��� ����
    private int _diagnosisIndex; // ���� �ε���
    private int _correctAnswers; // ���� ���� �� 

    private void Awake()
    {
        textAnsr = new TEXDraw[btAnsr.Length]; // ��ư ���ڸ�ŭ �Ҵ� TextDraw�� ����
        for (int i = 0; i < btAnsr.Length; ++i) // textAnsr�� text�Ҵ� 

            textAnsr[i] = btAnsr[i].GetComponentInChildren<TEXDraw>();

        _correctAnswerRemind = 0;
        _diagnosisIndex = 0;
        _correctAnswers = 0;
    }

    void Start()
    {
        Setup();
    }

    void Update()
    {
        if (isSolvingQuestion) // ���� Ǯ�� ���϶� �ð� ���
        {
            questionSolveTime += Time.deltaTime;
        }
    }

    void Setup()
    {
        wj_conn = FindObjectOfType<WJ_Connector>();
        if (wj_conn != null)
        {
            if(!wj_conn._needDiagnosis)
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
        }
    }

    /// <summary>
    /// ������ ���� �޾ƿ���
    /// </summary>
    void GetDiagnosis()
    {
        switch (wj_conn.cDiagnotics.data.prgsCd)
        {
            case "W":
                MakeQuestion(wj_conn.cDiagnotics.data.textCn,
                       wj_conn.cDiagnotics.data.qstCn,
                       wj_conn.cDiagnotics.data.qstCransr,
                       wj_conn.cDiagnotics.data.qstWransr);
                _diagnosisIndex++;
                break;
            case "E":
                if (!PlayerPrefs.HasKey("Diagnosis")) // ���� �Ǵ��� ������
                {
                    PlayerPrefs.SetInt("Diagnosis", System.Convert.ToInt16(0)); // ���� �ʿ����� �������� ����
                    wj_conn._needDiagnosis = false;
#if UNITY_EDITOR
                    Debug.LogWarning("���� �Ϸ�");
#endif
                }
                currentStatus = CurrentStatus.LEARNING;

                StartCoroutine(ColoringCorrectAnswer(1.5f));
                _correctAnswers = 0;
                break;
        }
    }

    /// <summary>
    ///  n ��° �н� ���� �޾ƿ���
    /// </summary>
    void GetLearning(int _index)
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
    /// �޾ƿ� �����͸� ������ ������ ǥ��
    /// </summary>
    void MakeQuestion(string textCn, string qstCn, string qstCransr, string qstWransr)
    {

        if (panel_diag_chooseDiff.activeSelf)
        {
            panel_diag_chooseDiff.SetActive(false);
        }

        panel_question.SetActive(true);

        // ù��° ���� �����̰ų� ù��° �н������϶���
        bool isFirstQuestion = (_diagnosisIndex == 0 && currentStatus == CurrentStatus.DIAGNOSIS) ||
            (currentQuestionIndex == 0 && currentStatus == CurrentStatus.LEARNING);



        if (textCn.Contains("�ִ�����") || textCn.Contains("�ּҰ����") ||
           textCn.Contains("greatest")|| textCn.Contains("minimum"))
        {
            ProblemConverter converter = new ProblemConverter();
            qstCn = converter.ProblemConvert(qstCn, ProblemType.A);
        }
        else if (textCn.Contains("������") || textCn.Contains("equation"))
        {
            ProblemConverter converter = new ProblemConverter();
            qstCn = converter.ProblemConvert(qstCn, ProblemType.B);
        }

        if (isFirstQuestion)
        {
            SetupQuestion(textCn, qstCn, qstCransr, qstWransr);
            _diagnosisIndex++;
        }
        else
        {
            StartCoroutine(ColoringCorrectAnswer(textCn, qstCn, qstCransr, qstWransr, 1.0f));
            _diagnosisIndex++;
        }

    }

    void SetupQuestion(string textCn, string qstCn, string qstCransr, string qstWransr) // ���� ���� �Լ�
    {
        string correctAnswer;
        string[] wrongAnswers;

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
        _correctAnswerRemind = ansrIndex; // ���� �ε��� �����صα�

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
        textEquation.text = qstCn;
        textDescription.text = textCn;
    }

    IEnumerator ColoringCorrectAnswer(string textCn, string qstCn, string qstCransr, string qstWransr, float delay) // ���� ���� �� ���� ǥ��
    {
        int prevIndex = _correctAnswerRemind; // ���� �ε��� ����
        textAnsr[_correctAnswerRemind].color = new Color(1.0f, 0.0f, 0.0f); // ���� �ε��� ���� ����
        ansCheckImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(delay); // ������
        textAnsr[prevIndex].color = new Color(0.0f, 0.0f, 0.0f); // ���� ���� �ε��� �ٽ� ���� �ǵ�����
        ansCheckImage.gameObject.SetActive(false);
        SetupQuestion(textCn, qstCn, qstCransr, qstWransr);
    }

    IEnumerator ColoringCorrectAnswer(float delay) // ������ ��
    {
        int prevIndex = _correctAnswerRemind; // �����̾��� �ε��� ���� (�ٽ� ���� ������ ������ ���ؼ�)
        textAnsr[_correctAnswerRemind].color = new Color(1.0f, 0.0f, 0.0f); // ���� �ε��� ���� ����
        ansCheckImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(delay); // ������

        textAnsr[prevIndex].color = new Color(0.0f, 0.0f, 0.0f); // ���� ���� �ε��� �ٽ� ���� �ǵ�����
        ansCheckImage.gameObject.SetActive(false);
        panel_question.SetActive(false);
    }

    /// <summary>
    /// ���� ���� �¾Ҵ� �� üũ
    /// </summary>
    public void SelectAnswer(int _idx = -1)
    {
        if (_idx == -1) // �ð��ʰ� ��
        {
            switch (currentStatus)
            {
                case CurrentStatus.DIAGNOSIS:

                    isSolvingQuestion = false;

                    wj_conn.Diagnosis_SelectAnswer("-1", "N", (int)(questionSolveTime * 1000));

                    questionSolveTime = 0;
                    break;
            }
            return;
        }

        GameObject UIManager = GameObject.Find("UIManager");
        if(UIManager != null)
        {
            if(UIManager.GetComponent<DrawLine>() != null)
            {
                UIManager.GetComponent<DrawLine>().ClearLineNS();
            }
        }

        bool isCorrect = false;
        string ansrCwYn = "N";

        switch (currentStatus)
        {
            case CurrentStatus.DIAGNOSIS:
                isCorrect = textAnsr[_idx].text.CompareTo(wj_conn.cDiagnotics.data.qstCransr) == 0 ? true : false;
                ansrCwYn = isCorrect ? "Y" : "N";

                isSolvingQuestion = false;
                if (ansrCwYn == "Y")
                {
                    if (ansCheckImage != null && checkList != null)
                    {
                        ansCheckImage.sprite = checkList[0];

                    }
                    Debug.Log("����");
                }
                else
                {
                    if (ansCheckImage != null && checkList != null)
                    {
                        ansCheckImage.sprite = checkList[1];
                    }
                    Debug.Log("����");
                }
                wj_conn.Diagnosis_SelectAnswer(textAnsr[_idx].text, ansrCwYn, (int)(questionSolveTime * 1000));

                questionSolveTime = 0;
                break;
        }

        if (isCorrect)
        {
            _correctAnswers += 1;
        }

        // TODO : ������ Ȯ���ϰ� �˸´� ȿ���� ���
        {
            SoundManager soundManager = GameObject.Find("UIManager").GetComponent<SoundManager>();
            if (soundManager == null)
            {
                return;
            }

            if (isCorrect)
            {
                soundManager.PlayEffect(Effect.Correct);
            }
            else
            {
                soundManager.PlayEffect(Effect.Wrong);
            }
        }
    }



    // ���̵� ���� ��ư
    public void ButtonEvent_ChooseDifficulty(int a) // ������ �� ���̵� ���� ��ư(On Click���� �����������)
    {
        if (wj_conn._needDiagnosis)
        {
            currentStatus = CurrentStatus.DIAGNOSIS; // ���� ���� �������� ����
            wj_conn.FirstRun_Diagnosis(a); // ���̵� ���� 
        }
    }
}

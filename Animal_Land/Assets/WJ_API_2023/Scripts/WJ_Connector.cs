using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using WjChallenge;

public class WJ_Connector : MonoBehaviour
{
    [Header("My Info")]
    public string strGameCD;        //�����ڵ�
    public string strGameKey;       //����Ű(Api Key)

    public string UserID => strMBR_ID;

    private string strAuthorization;

    private string strMBR_ID;       //��� ID
    private string strDeviceNm;     //����̽� �̸�
    private string strOsScnCd;      //OS
    private string strGameVer;      //���� ����

    public bool _needDiagnosis;

    #region StoredData
    [HideInInspector]
    public DN_Response cDiagnotics = null; //���� - ���� Ǯ���ִ� ������ ����
    [HideInInspector]
    public Response_Learning_Setting cLearnSet = null; //�н� - �н� ���� ��û �� �޾ƿ� �н� ������
    [HideInInspector]
    public Response_Learning_Progress cLearnProg = null; //�н� - �н� �Ϸ� �� �޾ƿ� ���
    [HideInInspector]
    public List<Learning_MyAnsr> cMyAnsrs = null;
    [HideInInspector]
    public Request_DN_Progress result = null;
    [HideInInspector]
    public string qstCransr = "";
    #endregion

    #region UnityEvents
    [HideInInspector]
    public UnityEvent onGetDiagnosis;
    [HideInInspector]
    public UnityEvent onGetLearning;
    #endregion

    private void Awake()
    {
        if (SystemInfo.deviceType == DeviceType.Desktop)
            strDeviceNm = "PC";
        else
            strDeviceNm = SystemInfo.deviceModel;

        strOsScnCd = SystemInfo.operatingSystem;
        strGameVer = Application.version;

        if (strOsScnCd.Length >= 15) strOsScnCd = strOsScnCd.Substring(0, 14);

        Make_MBR_ID();


        if (PlayerPrefs.HasKey("Diagnosis"))
        {
            _needDiagnosis = System.Convert.ToBoolean(PlayerPrefs.GetInt("Diagnosis")); // ������ �ʿ�����
        }
        else
        {
                _needDiagnosis = true;
        }

#if UNITY_EDITOR
        Debug.LogWarning(strMBR_ID);
#endif
    }


    //���� �ð��� �������� MBR ID ����
    private void Make_MBR_ID()
    {
        if (PlayerPrefs.HasKey("UserID")) // �̹� ���̵� �ִٸ�
        {
            strMBR_ID = PlayerPrefs.GetString("UserID").ToString();
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning("���̵� ����");
#endif
            DateTime dt = DateTime.Now;
            strMBR_ID = string.Format("{0}{1:0000}{2:00}{3:00}{4:00}{5:00}{6:00}{7:000}", strGameCD, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);

            PlayerPrefs.SetString("UserID", strMBR_ID);
        }
    }

    #region Function Progress

    /// <summary>
    /// ������ ù ���� �� ������ ���
    /// </summary>
    private IEnumerator Send_Diagnosis(int level)
    {
        Request_DN_Setting request = new Request_DN_Setting();

        request.gameCd = strGameCD;
        request.mbrId = strMBR_ID;
        request.deviceNm = strDeviceNm;
        request.gameVer = strGameVer;
        request.osScnCd = strOsScnCd;
        request.langCd = "EN";
        request.timeZone = TimeZoneInfo.Local.BaseUtcOffset.Hours;


        request.gameCd = strGameCD;
        request.mbrId = strMBR_ID;
        request.gameVer = strGameVer;
        request.osScnCd = strOsScnCd;
        request.deviceNm = strDeviceNm;
        request.langCd = "EN";
        request.timeZone = TimeZoneInfo.Local.BaseUtcOffset.Hours;

        switch (level)
        {
            case 0: request.bgnLvl = "A"; break; //���� �� �� �־��
            case 1: request.bgnLvl = "B"; break; // ���� ����
            case 2: request.bgnLvl = "C"; break; // ���� ������
            case 3: request.bgnLvl = "D"; break; // �м� �Ҽ�
            default: request.bgnLvl = "A"; break;
        }

        yield return StartCoroutine(UWR_Post<Request_DN_Setting, DN_Response>(request, "https://prd-brs-relay-model.mathpid.com/api/v1/contest/diagnosis/setting", false));

        onGetDiagnosis.Invoke();

        yield return null;
    }

    /// <summary>
    /// ������ �� ���� Ǯ�� �� ������ ���
    /// </summary>
    private IEnumerator SendProgress_Diagnosis(string _prgsCd, string _qstCd, string _qstCransr, string _ansrCwYn, long _sid, long _nQstDelayTime)
    {
        Request_DN_Progress request = new Request_DN_Progress();
        request.mbrId = strMBR_ID;
        request.gameCd = strGameCD;
        request.prgsCd = _prgsCd;// "W";    // W: ���� ����    E: ���� �Ϸ�    X: ��Ÿ ���?
        request.qstCd = _qstCd;             // ���� �ڵ�
        request.qstCransr = _qstCransr;     // �Է��� �䳻��
        request.ansrCwYn = _ansrCwYn;//"Y"; // ���� ����
        request.sid = _sid;                 // ���� ID
        request.slvTime = _nQstDelayTime;//5000;
        
        yield return StartCoroutine(UWR_Post<Request_DN_Progress, DN_Response>(request, "https://prd-brs-relay-model.mathpid.com/api/v1/contest/diagnosis/progress", true));

        onGetDiagnosis.Invoke();

        yield return null;
    }

    /// <summary>
    /// �н� ������ �޾ƿ��� ���� ������ ���
    /// </summary>
    private IEnumerator Send_Learning()
    {
        Request_Learning_Setting request = new Request_Learning_Setting();

        request.gameCd = strGameCD;
        request.mbrId = strMBR_ID;
        request.gameVer = strGameVer;
        request.osScnCd = strOsScnCd;
        request.deviceNm = strDeviceNm;
        request.langCd = "EN";
        request.timeZone = TimeZoneInfo.Local.BaseUtcOffset.Hours;

        request.mathpidId = "";


        yield return StartCoroutine(UWR_Post<Request_Learning_Setting, Response_Learning_Setting>(request, "https://prd-brs-relay-model.mathpid.com/api/v1/contest/learning/setting", true));

        onGetLearning.Invoke();

        cMyAnsrs = new List<Learning_MyAnsr>();

        yield return null;
    }

    /// <summary>
    /// �н� 8���� �Ϸ��Ҷ����� ������ ����Ͽ� �н� ����� �޾ƿ�
    /// </summary>
    private IEnumerator SendProgress_Learning()
    {
        Request_Learning_Progress request = new Request_Learning_Progress();

        request.gameCd = strGameCD;
        request.mbrId = strMBR_ID;
        request.prgsCd = "E";
        request.sid = cLearnSet.data.sid;
        request.bgnDt = cLearnSet.data.bgnDt;

        request.data = cMyAnsrs;

        yield return StartCoroutine(UWR_Post<Request_Learning_Progress, Response_Learning_Progress>(request, "https://prd-brs-relay-model.mathpid.com/api/v1/contest/learning/progress", true));

        yield return null;
    }

    /// <summary>
    /// UnityWebRequest�� ����Ͽ� ������ ������ ���
    /// </summary>
    /// <typeparam name="TRequest"> ������ ���ϴ� Ÿ�� </typeparam>
    /// <typeparam name="TResponse"> �޾ƿ� Ÿ�� </typeparam>
    /// <param name="request"> ������ �� </param>
    /// <param name="url"> URL </param>
    /// <param name="isSendAuth"> Authorization ��� ������ </param>
    /// <returns></returns>
    private IEnumerator UWR_Post<TRequest, TResponse>(TRequest request, string url, bool isSendAuth)
    where TRequest : class
    where TResponse : class
    {
        string strBody = JsonUtility.ToJson(request);

        using (UnityWebRequest uwr = UnityWebRequest.Post(url, string.Empty))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(strBody);
            uwr.uploadHandler = new UploadHandlerRaw(jsonToSend);
            uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            uwr.SetRequestHeader("Content-Type", "application/json");
            uwr.SetRequestHeader("x-api-key", strGameKey);

            if (isSendAuth) // ����Ű ����
            {
                if (PlayerPrefs.HasKey("Auth")) // Ű�� ������ �ִٸ� �ҷ�����
                {
                    strAuthorization = PlayerPrefs.GetString("Auth");
                }
                uwr.SetRequestHeader("Authorization", strAuthorization);
            }

            uwr.timeout = 5;

            yield return uwr.SendWebRequest();


            Debug.Log($"��Request => {strBody}");

            if (uwr.error == null)  //���� ��
            {
                TResponse output = default;
                try
                {
                    output = JsonUtility.FromJson<TResponse>(uwr.downloadHandler.text);
                }
                catch (Exception e) { Debug.LogError(e.Message); }

                cDiagnotics = null;
                cLearnSet = null;
                cLearnProg = null;

                switch (output)
                {
                    case DN_Response dnResponse:
                        cDiagnotics = dnResponse;
                        qstCransr = cDiagnotics.data.qstCransr;
                        break;

                    case Response_Learning_Setting ResponselearnSet:
                        cLearnSet = ResponselearnSet;

                        if (!PlayerPrefs.HasKey("Diagnosis")) // ���� �Ǵ��� ������
                        {
                            PlayerPrefs.SetInt("Diagnosis", System.Convert.ToInt16(0)); // ���� �ʿ����� �������� ����
#if UNITY_EDITOR
                            Debug.LogWarning("���� �Ϸ�");
#endif
                        }

                        break;
                    case Response_Learning_Progress ResponselearnProg:
                        cLearnProg = ResponselearnProg;
                        break;

                    default:
                        Debug.LogError("type error - output type : " + output.GetType().ToString());
                        break;
                }

                if (PlayerPrefs.HasKey("Auth")) // �̹� ����Ű�� �ִٸ�
                {
                    strAuthorization = PlayerPrefs.GetString("Auth").ToString();
                }
                else
                {
                    if (uwr.GetResponseHeaders().ContainsKey("Authorization")) // ���� Ű ����ְ�
                    {
                        strAuthorization = uwr.GetResponseHeader("Authorization");
                        PlayerPrefs.SetString("Auth", strAuthorization);
                    }
                }
            }
            else //���� ��
            {
                Debug.LogError("����� �޾ƿ��� �� �����߽��ϴ�.");
                Debug.LogError(uwr.error.ToString());
            }

            Debug.Log($"��Response => {uwr.downloadHandler.text}");
            uwr.Dispose();
        }
    }
    #endregion

    #region Public Method

    public void FirstRun_Diagnosis(int diff) // ó�� ������ 
    {
        StartCoroutine(Send_Diagnosis(diff)); // parameter ���̵� ����
    }

    public void Learning_GetQuestion()
    {
        StartCoroutine(Send_Learning());
    }

    /// <summary>
    /// ���� - �� ������ ���� �� ����
    /// </summary>
    public void Diagnosis_SelectAnswer(string _cransr, string _ansrYn, long _slvTime = 5000)
    {
        long sid = cDiagnotics.data.sid;
        string prgsCd = cDiagnotics.data.prgsCd;
        string qstCd = cDiagnotics.data.qstCd;

        string qstCransr = _cransr;
        string ansrCwYn = _ansrYn;
        long slvTime = _slvTime;

        StartCoroutine(SendProgress_Diagnosis(prgsCd, qstCd, qstCransr, ansrCwYn, sid, slvTime));
    }

    /// <summary>
    /// �н� - �� ������ ����, Ǭ ������ n���� �Ǹ� ����
    /// </summary>
    public void Learning_SelectAnswer(int _index, string _cransr, string _ansrYn, long _slvTime = 5000)
    {
        if (cMyAnsrs == null) cMyAnsrs = new List<Learning_MyAnsr>();

        cMyAnsrs.Add(new Learning_MyAnsr(cLearnSet.data.qsts[_index - 1].qstCd, _cransr, _ansrYn, 0));

        if (cMyAnsrs.Count >= 2) // ���⼭ ���� ��� �������� ���� ���߿� �����ѹ� �ٲٱ�
        {
            StartCoroutine(SendProgress_Learning());
        }
    }

    #endregion

    #region ForTest
    public void Diagnosis_SelectAnswer_Forced()
    {
        long sid = cDiagnotics.data.sid;
        string prgsCd = cDiagnotics.data.prgsCd;
        string qstCd = cDiagnotics.data.qstCd;

        string qstCransr = cDiagnotics.data.qstCransr;
        string ansrCwYn = "Y";
        long slvTime = 5000;

        StartCoroutine(SendProgress_Diagnosis(prgsCd, qstCd, qstCransr, ansrCwYn, sid, slvTime));
    }
    #endregion


}

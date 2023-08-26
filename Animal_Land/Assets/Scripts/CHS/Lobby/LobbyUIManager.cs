using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.Linq;
using UnityEngine.SocialPlatforms;

public class LobbyUIManager : MonoBehaviourPunCallbacks
{
    [Header("로비 텍스트 리스트")]
    [SerializeField]
    private string          ConnectingText       = "Connecting...";
    [SerializeField]
    private string          TryConnectText       = "Touch To Start";
    [SerializeField]
    private string          JoinToRoomText       = "Join To Room...";

    [Header("로비 UI 요소")]
    // UI 멤버 변수
    public Button           joinButton;                 // 룸 접속 버튼
    public TextMeshProUGUI  connectionInfoText;         // 네트워크 정보를 표시할 텍스트
    
    public Button           joinToMultiButton;                 // 룸 접속 버튼
    public Button           joinToSingleButton;                // 룸 접속 버튼

    [Header("싱글 게임 패널 관련")]
    public GameObject       singleGamePanel;

    [Header("설정 패널 관련")]
    public GameObject       settingPanel;

    [Header("매니저 클래스")]
    public LobbyManager     lobbyManager;

    // Start is called before the first frame update
    void Start()
    {
        SetConnectText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ConnectGame()
    {
    
    }

    public void OpenSingleGamePanel()
    {
        singleGamePanel.SetActive(true);
    }

    public void CloseSingleGamePanel()
    {
        singleGamePanel.SetActive (false);
    }

    public void OpenSettingPanel()
    {
        settingPanel.SetActive(true);
    }

    public void CloseSettingPanel()
    {
        settingPanel.SetActive(false);
    }

    public void ConnectMultiGame()
    {
        Debug.Log("ConnectGame");
        // 중복 접속 시도를 막기 위해 접속 버튼 잠시 비활성화
        joinButton.interactable = false;
        lobbyManager.Connect();
    }

    public void ConnectSingleGame()
    {
        SceneManager.LoadScene(SStageInfo.StageName);
    }

    public void SetJoinToRoomText()
    {
        ChangeText(JoinToRoomText);
    }

    public void SetConnectText()
    {
        Debug.Log("SetConnectText");
        ChangeText(ConnectingText);
    }
    public void SetTryConnectText()
    {
        joinButton.interactable = true;
        ChangeText(TryConnectText);
    }

    void ChangeText(string text)
    {
        connectionInfoText.text = text;
    }

    public void SetStageNumber(string stage)
    {
        // TODO : 스테이지 이름을 넘겨준다.
        SStageInfo.StageName = stage;
    }

}

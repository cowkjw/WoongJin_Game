using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class LobbyUIManager : MonoBehaviourPunCallbacks
{
    [Header("로비 텍스트 리스트")]
    [SerializeField]
    private string          ConnectingText       = "Connecting...";
    [SerializeField]
    private string          TryConnectText       = "Touch To Start";
    [SerializeField]
    private string          JoinToRoomText       = "Join To Room...";

    [Header("UI 요소")]
    // UI 멤버 변수
    public Button           joinToMultiButton;                 // 룸 접속 버튼
    public Button           joinToSingleButton;                // 룸 접속 버튼
    public TextMeshProUGUI  connectionInfoText;                // 네트워크 정보를 표시할 텍스트

    public GameObject       singleGamePanel;

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

    public void ConnectMultiGame()
    {
        Debug.Log("ConnectGame");
        // 중복 접속 시도를 막기 위해 접속 버튼 잠시 비활성화
        joinToMultiButton.interactable = false;
        lobbyManager.Connect();
    }

    public void OpenSingleGamePanel()
    {
        singleGamePanel.SetActive(true);
    }

    public void CloseSingleGamePanel()
    {
        singleGamePanel.SetActive(false);
    }

    public void ConnectSingleGame()
    {
        // TODO : 현재 단일 씬으로 이동하는 것을 스테이지 별로 이동할 수 있게 변경
        SceneManager.LoadScene("Single Game Room");
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
        joinToMultiButton.interactable = true;
        ChangeText(TryConnectText);
    }

    void ChangeText(string text)
    {
        connectionInfoText.text = text;
    }

}

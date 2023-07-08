using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR;
using UnityEngine;
using Photon.Pun;           // 유니티용 포톤 컴포넌트
using Photon.Realtime;      // 포톤 서비스 관련 라이브러리
using UnityEngine.UI;
using TMPro;

public class TitleManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1";

    public TextMeshProUGUI connectionInfoText;     // 네트워크 정보를 표시할 텍스트
    public Button joinButton;           // 룸 접속 버튼

    // 게임 실행과 동시에 마스터 섭 접속 시도
    private void Start()
    {
        // 원하는 동기화 주기 설정
        PhotonNetwork.SendRate = 30; // 5번/초로 메시지를 보냄
        PhotonNetwork.SerializationRate = 60; // 5번/초로 메시지를 수신 및 업데이트

        // 접속에 필요한 정보(게임 버전) 설정
        PhotonNetwork.GameVersion = gameVersion;
        // 설정한 정보로 마스터 서버 접속 시도 
        PhotonNetwork.ConnectUsingSettings();

        // 룸 접속 버튼 잠시 비활성화
        joinButton.interactable = false;
        // 접속 시도중임을 텍스트로 표시
        connectionInfoText.text = "Connecting...";
    }

    // 마스터 서버 접속 성공 시 자동 실행 -> ConnectUsingSettings() 성공 시
    public override void OnConnectedToMaster()
    {
        // 룸 접속 버튼 활성화
        joinButton.interactable = true;
        // 접속 정보 표시
        connectionInfoText.text = "Online : Master Server";
    }

    // 마스터 서버 접속 실패 시 자동 실행 -> ConnectUsingSettings() 실패 시 
    // 실패 원인 정보는 DisconnectCause 타입으로 들어온다.
    public override void OnDisconnected(DisconnectCause cause)
    {
        // 룸 접속 버튼 비활성화
        joinButton.interactable = false;
        // 접속 정보 표시
        connectionInfoText.text = "ReConnecting...";

        // 마스터 서버로의 재접속 시도
        PhotonNetwork.ConnectUsingSettings();
    }

    // 룸 접속 시도
    public void Connect()
    {
        // 중복 접속 시도를 막기 위해 접속 버튼 잠시 비활성화
        joinButton.interactable = false;

        // 마스터 서버에 접속 중이라면
        if (PhotonNetwork.IsConnected)
        {
            // 룸 접속 실행
            connectionInfoText.text = "Join To Room...";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // 마스터 서버에 접속 중이 아니라면 마스터 서버에 접속 시도
            connectionInfoText.text = "ReConnecting...";
            // 마스터 서버로의 재접속 시도
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // (빈 방이 없어) 랜덤 룸 참가에 실패한 경우 자동 실행
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // 접속 상태 표시
        connectionInfoText.text = "Create New Room";
        // 최대 4명을 수용 가능한 빈 방 생성
        PhotonNetwork.CreateRoom("GameRoom" + PhotonNetwork.CountOfRooms, new RoomOptions { MaxPlayers = 4 });
    }

    // 룸에 참가 완료된 경우 자동 실행
    public override void OnJoinedRoom()
    {
        // 접속 상태 표시
        connectionInfoText.text = "Success";

        // 모든 룸 참가자가 Main 씬을 로드하게 함
        PhotonNetwork.LoadLevel("Game Room");
    }
}

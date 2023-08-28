using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;           // 유니티용 포톤 컴포넌트
using Photon.Realtime;      // 포톤 서비스 관련 라이브러리
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    private string          _gameVersion = "1";
    public LobbyUIManager   lobbyUIManager;             

    private void Awake()
    {
    }

    private void Start()
    {
        // TODO : 멀티 기능 임시 차단
        return;

        // 원하는 동기화 주기 설정
        PhotonNetwork.SendRate = 30;            // 5번/초로 메시지를 보냄
        PhotonNetwork.SerializationRate = 60;   // 5번/초로 메시지를 수신 및 업데이트

        // 접속에 필요한 정보(게임 버전) 설정
        PhotonNetwork.GameVersion = _gameVersion;
        // 설정한 정보로 마스터 서버 접속 시도 
        PhotonNetwork.ConnectUsingSettings();
    }

    // 마스터 서버 접속 성공 시 자동 실행 -> ConnectUsingSettings() 성공 시
    public override void OnConnectedToMaster()
    {
        lobbyUIManager.SetTryConnectText();
    }

    // 마스터 서버 접속 실패 시 자동 실행 -> ConnectUsingSettings() 실패 시 
    public override void OnDisconnected(DisconnectCause cause)
    {
        // 마스터 서버로의 재접속 시도
        PhotonNetwork.ConnectUsingSettings();
    }

    // 룸 접속 시도
    public void Connect()
    {

    }

    // (빈 방이 없어) 랜덤 룸 참가에 실패한 경우 자동 실행
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // TODO : 방 설정하는 법에 대해 더 알아보기
        PhotonNetwork.CreateRoom("GameRoom" + PhotonNetwork.CountOfRooms, new RoomOptions { MaxPlayers = 4 });
    }

    // 룸에 참가 완료된 경우 자동 실행
    public override void OnJoinedRoom()
    {
        // 모든 룸 참가자가 Main 씬을 로드하게 함
        PhotonNetwork.LoadLevel("Game Room");
    }

    public string GetGameVersion()
    {
        return _gameVersion;
    }
}

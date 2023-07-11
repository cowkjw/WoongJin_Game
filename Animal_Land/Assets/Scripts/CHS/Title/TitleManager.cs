using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;           // 유니티용 포톤 컴포넌트
using Photon.Realtime;      // 포톤 서비스 관련 라이브러리
using UnityEngine.UI;
using TMPro;

public class TitleManager : MonoBehaviourPunCallbacks
{
    [Header("매니저 스크립트")]
    public TitleUIManager   titleUIManager; 
    public UserManager      userManager;


    // 게임 실행과 동시에 유저 정보 불러오기
    private void Start()
    {
        if (LoadData() == true)
        {
            // TODO : 불러온 데이터에서 진단평가를 했는지 판단하고 버튼을 고친다.
            bool check = IsAlreadyTest();
            titleUIManager.SetCanMoveToLobby(check);

        }
        else
        {
            Debug.LogError("유저 정보를 불러오지 못했습니다.");
        }
    }

    private bool LoadData()
    {
        bool result = false;
        // TODO : 클라이언트에 저장된 값을 불러온다.

        // 불러오는데에 성공했다면 버튼을 활성화 시킨다.        
        result = true;

        return result;
    }

    private bool IsAlreadyTest()
    {
        // TODO : 진단 평가를 했는지 파악하는 함수를 작성

        return true;
    }

    private bool CheckPlayerAlreadyTest()
    {
        bool result = false;
        // TODO : 데이터에서 값을 가져와서 판단한다.

        // 진단 평가를 진행한 이력이 있다면 로비로 이동한다.
        result = true;

        return result;
    }

    
}

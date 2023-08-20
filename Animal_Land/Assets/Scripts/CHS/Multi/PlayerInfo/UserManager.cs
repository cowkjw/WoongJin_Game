using Contents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserManager : MonoBehaviour
{
    private static UserManager instance = null;

    // 총 4가지 종류의 캐릭터의 정보를 가지고 있다
    private CharacterInfo[] userInfos = new CharacterInfo[4];
    public CharacterDefaultInfo[] CharacterDefaultInfos = new CharacterDefaultInfo[4];

    public PlayerData PlayerData { get; private set; } = null; // 재화,아이템 구매 리스트

    // TODO : 클라이언트에 저장되어 있는 값을 붑러온다.
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        LoadUserInfo();
    }

    void LoadUserInfo()
    {
        // 플레이어 정보 가져옴 

        PlayerData playerData = DataManager.Instance.PlayerData;
        if (playerData != null)
        {
            this.PlayerData = playerData;
        }

        //// 캐릭터 정보 로드
        //LoadCharcterInfo();

        //// 상점 정보 로드
        //LoadStoreInfo();

        //// 재화 로드
        //LoadCreditInfo();
    }

    void LoadCharcterInfo()
    {
        // 캐릭터의 정보를 가지고 온다.

    }

    void LoadStoreInfo()
    {
        // 상점 정보를 가지고 온다.

    }

    void LoadCreditInfo()
    {
        // 기타(재화 등)의 정보를 가지고 온다.

    }

    public CharacterInfo GetCharcterInfo(int playerCount)
    {
        return userInfos[playerCount - 1];
    }

    public CharacterDefaultInfo GetCharcterDefaultInfo(int playerCount)
    {
        CharacterDefaultInfo characterDefaultInfo = CharacterDefaultInfos[playerCount - 1];
        if (characterDefaultInfo != null)
        {
            return characterDefaultInfo;
        }
        else
        {
            Debug.Log("characterDefaultInfo is null");
            return null;
        }
    }

    void Start()
    {

    }

    void Update()
    {

    }
}

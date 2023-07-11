using Mono.Cecil.Cil;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Test
{
    Sprite CharcterSprite; 
}

public class Character : MonoBehaviour
{
    [SerializeField]
    CharacterDefaultInfo    _defaultInfo;
    [SerializeField]
    CharacterInfo           _characterInfo;

    private PhotonView _pv;

    private void Awake()
    {
        _defaultInfo = new CharacterDefaultInfo();
        _characterInfo = new CharacterInfo();
        _pv = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // 현재 클라이언트의 소유가 아닌 캐릭터들은 마스터 클라이언트로부터 받아온다.
        if(_pv.IsMine == false && PhotonNetwork.IsMasterClient == false)
        {
            _pv.RPC("SetupCharacterFromMasterClientRPC", RpcTarget.MasterClient);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 플레이어 카운트를 받아 해당 캐릭터로 업데이트한다. 
    // 클라이언트 접속 시, 실행되는 메소드
    public void SetupCharacter(int playerCount)
    {
        if (GetUserManager() != null)
        {
            // 입장 순서에 따라 디폴트 정보를 불러온다.
            _defaultInfo = GetUserManager().GetCharcterDefaultInfo(playerCount);

            // 자신의 정보를 가져온다.
            _characterInfo = GetUserManager().GetCharcterInfo(playerCount);

            // 가진 정보를 이용하여 캐릭터 커스텀을 불러온다.
            
        }

        // 불러온 정보를 이용하여 캐릭터 정보를 업데이트 한다.
        _pv.RPC("UpdateCharacterRPC", RpcTarget.All, 
            _defaultInfo.GetMoveColor(),
            _defaultInfo.GetAreaColor(),
            _defaultInfo.GetBorderColor(),
            _defaultInfo.GetCharcterSpriteName());
    }

    [PunRPC]
    public void SetupCharacterFromMasterClientRPC()
    {
        // 해당 함수를 호출받은 마스터 클라이언트는 자신의 정보를 모든 RPC에 전달한다
        _pv.RPC("UpdateCharacterRPC", RpcTarget.All,
            _defaultInfo.GetMoveColor(),
            _defaultInfo.GetAreaColor(),
            _defaultInfo.GetBorderColor(),
            _defaultInfo.GetCharcterSpriteName());
    }

    [PunRPC]
    private void UpdateCharacterRPC(
        /*여기에 업데이트할 정보를 넘겨준다.*/
        /*Default Info*/
        float[] moveColor,
        float[] areaColor,
        float[] borderColor,
        string sprieName

        /*Custom Info*/


        /**/

        )
    {
        // 정보를 저장한다.
        _defaultInfo.SetCharacterSpriteName(sprieName);

        // 정보를 업데이트한다.
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if(spriteRenderer != null)
        {
            Sprite sprite = Resources.Load<Sprite>("Sprites/Characters/" + _defaultInfo.GetCharcterSpriteName());
            spriteRenderer.sprite = sprite;
        }
    }

    private UserManager GetUserManager()
    {
        GameObject Object = GameObject.Find("UserManager");
        if (Object != null)
        {
            UserManager userManager = Object.GetComponent<UserManager>();
            if (userManager != null)
            {
                return userManager;
            }
        }

        Debug.Log("User Manager is Null");
        return null;
    }
}

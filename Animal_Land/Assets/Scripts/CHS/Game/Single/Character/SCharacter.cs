using Mono.Cecil.Cil;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 캐릭터의 정보를 저장하는 스크립트
// 캐릭터 프리팹에 컴포넌트로 추가하여 캐릭터의 정보를 저장한다.
public class SCharacter : MonoBehaviour
{
    [SerializeField]
    CharacterDefaultInfo    _defaultInfo;
    [SerializeField]
    CharacterInfo           _characterInfo;

    [Header("이동 게이지")]
    [SerializeField] private float _moveGauge = 10f;
    [SerializeField] private float _maxMoveGauge = 10f;
    [SerializeField] private float _moveGuageConsumption = 1f;

    [SerializeField] private SGameUIManager _gameUIManager;

    private void Awake()
    {
        _defaultInfo = new CharacterDefaultInfo();
        _characterInfo = new CharacterInfo();

        _gameUIManager = GameObject.Find("UIManager").GetComponent<SGameUIManager>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // 플레이어 카운트를 받아 해당 캐릭터로 업데이트한다. 
    // 클라이언트 접속 시, 실행되는 메소드
    public void SetupCharacter(int playerCount)
    {
        UserManager userManager = GetUserManager();
        if (userManager != null)
        {
            // 입장 순서에 따라 디폴트 정보를 불러온다.
            _defaultInfo = userManager.GetCharcterDefaultInfo(playerCount);

            // 자신의 정보를 가져온다.
            _characterInfo = userManager.GetCharcterInfo(playerCount);

            // 가진 정보를 이용하여 캐릭터 커스텀을 불러온다.

            // 불러온 정보를 이용하여 캐릭터 정보를 업데이트 한다.
            UpdateCharacter(
                _defaultInfo.GetMoveColor(),
                _defaultInfo.GetAreaColor(),
                _defaultInfo.GetBorderColor(),
                _defaultInfo.GetCharcterSpriteName());
        }
    }
    private void UpdateCharacter(
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
        // CharcterSprite 정보 저장&업데이트
        {
            // 정보를 저장한다.
            _defaultInfo.SetCharacterSpriteName(sprieName);

            // 정보를 업데이트한다.
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                Sprite sprite = Resources.Load<Sprite>("Sprites/Characters/" + _defaultInfo.GetCharcterSpriteName());
                spriteRenderer.sprite = sprite;
            }
        }
        // Color 정보 저장&업데이트 (Move, Area, Border)
        {
            STileColorChange tileColorChange = GetComponent<STileColorChange>();
            if (tileColorChange == null)
            {
                Debug.Log("TileColorChange Component is Null");
                return;
            }

            // MoveColor 정보 업데이트
            {
                // 정보를 저장한다.
                _defaultInfo.SetMoveColor(moveColor);

                // 정보를 업데이트한다.
                tileColorChange.SetMoveColor(_defaultInfo.GetMoveColor());
            }
            // Ara Color 정보 업데이트
            {
                // 정보를 저장한다.
                _defaultInfo.SetAreaColor(areaColor);

                // 정보를 업데이트한다.
                tileColorChange.SetAreaColor(_defaultInfo.GetAreaColor());
            }
            // Border Color 정보 업데이트
            {
                // 정보를 저장한다.
                _defaultInfo.SetBorderColor(borderColor);

                // 정보를 업데이트한다.
                tileColorChange.SetBorderColor(_defaultInfo.GetBorderColor());
            }
        }
        // 
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

    public void StopPlayer()
    {
        StopMove();
    }

    private void SetMove(bool value)
    {
        // 조이스틱 
        SJoyStick JoyStick = GameObject.FindGameObjectWithTag("GameController").GetComponent<SJoyStick>();
        if (JoyStick != null)
        {
            if (JoyStick.IsGameStart() == value)
            {
                return;
            }
            else
            {
                JoyStick.SetIsGameStart(value);
            }
        }

        this.GetComponent<Rigidbody2D>().velocity = Vector3.zero; 
    }

    public void StopMove()
    {
        SetMove(false);

    }

    public void StartPlayer()
    {
        SetMove(true);


    }
    public void ConsumeMoveGauge()
    {
        _moveGauge -= (_moveGuageConsumption * Time.deltaTime);

        if(_moveGauge <= 0f)
        {
            StopMove();
            _moveGauge = 0f;
            _gameUIManager.UpdateGauge(0f);

            return;
        }

        _gameUIManager.UpdateGauge( _moveGauge / _maxMoveGauge);
    }

    public float AddMoveGauge(float value)
    {
        float newGaugeValue = _moveGauge + value;
        if(newGaugeValue > _maxMoveGauge)
        {
            newGaugeValue = _maxMoveGauge;
        }

        _moveGauge = newGaugeValue;

        return _moveGauge / _maxMoveGauge;
    }

}

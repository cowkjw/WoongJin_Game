using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public enum Effect
{
    Gameover = 0,   // 게임 오버
    Button,         // 버튼 클릭 
    Back,           // 뒤로 가기
    Ground,         // 땅을 획득한 경우
    Item,           // 아이템 획득
    Countdown,      // 시간이 얼마 남지 않은 경우
    Correct,        // 정답을 맞춘 경우
    Wrong,          // 정답을 틀린 경우
    Erase,          // 메모장 지우기
    Punch           // 피격 시 
}

// 씬마다 해당 스크립트를 UIManager에 추가하고 배경음악과 설정을 연결한다.
public class SoundManager : MonoBehaviour
{
    [Header("사운드 트랙")]
    public AudioClip BackGroundSound;

    [Header("설정")]
    public Slider BGSoundSlider;
    public Slider EffectSoundSlider;

    [Header("이펙트")]
    [SerializeField] private AudioSource _effectSource;
    [SerializeField] private AudioSource _playerSource;
    [SerializeField] private AudioSource _monsterSource;
    [SerializeField] private AudioSource _itemSource;

    [Header("이펙트 리소스")]
    [SerializeField] private AudioClip _gameOver;
    [SerializeField] private AudioClip _button;
    [SerializeField] private AudioClip _back;
    [SerializeField] private AudioClip _ground;
    [SerializeField] private AudioClip _item;
    [SerializeField] private AudioClip _countdown;
    [SerializeField] private AudioClip _correct;
    [SerializeField] private AudioClip _wrong;
    [SerializeField] private AudioClip _erase;
    [SerializeField] private AudioClip _punch;

    private AudioSource audioSource;
    UserManager userManager;

    private void Awake()
    {
        Init();
    }

    private void ChangeAudioSource(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
    }

    private void Init()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = BackGroundSound;

#if UNITY_EDITOR
        Debug.Log("사운드 값 초기화");
#endif

        userManager = GameObject.Find("UserManager").GetComponent<UserManager>();
        if(userManager != null )
        {
            float bgVolume = userManager.GetSoundVolume() + 0.06f;
            float effectVolume = userManager.GetEffectVolume() + 0.06f;
            if (bgVolume > 0.5f)
            {
                bgVolume = 0.5f;
            }
            if (effectVolume > 0.5f)
            {
                effectVolume = 0.5f;
            }

            userManager.SetEffectVolume(effectVolume);
            userManager.SetSoundVolume(bgVolume);

            audioSource.volume = userManager.GetSoundVolume();
            if (BGSoundSlider != null)
            {
                BGSoundSlider.value = userManager.GetSoundVolume();
            }

            if (EffectSoundSlider != null)
            {
                EffectSoundSlider.value = userManager.GetEffectVolume();
            }

            if (_effectSource != null)
            {
                _effectSource.volume = userManager.GetEffectVolume();
            }

            if (_playerSource != null)
            {
               _playerSource.volume = userManager.GetEffectVolume();
            }

            if (_monsterSource != null)
            {
                _monsterSource.volume = userManager.GetEffectVolume();
            }

            if (_itemSource != null)
            {
                _itemSource.volume = userManager.GetEffectVolume();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetVolume(float volume)
    {
        if(volume < 0.06f)
        {
            volume = 0.06f;
            BGSoundSlider.value = volume;
        }

        // 최솟 값 보정
        volume -= 0.06f;

        if (audioSource != null)
        {
            audioSource.volume = volume;
            if(userManager != null)
            {
                userManager.SetSoundVolume(volume);
            }
        }
    }

    public void SetEffectVolume(float volume)
    {
        if (volume < 0.06f)
        {
            volume = 0.06f;
            EffectSoundSlider.value = volume;
        }

        // 최솟 값 보정
        volume -= 0.06f;

        if (userManager != null)
        {
            userManager.SetEffectVolume(volume);
        }

        if (_effectSource != null)
        {
            _effectSource.volume = volume;
        }

        if (_playerSource != null)
        {
            _playerSource.volume = volume;
        }

        if (_monsterSource != null)
        {
            _monsterSource.volume = volume;
        }

        if (_itemSource != null)
        {
            _itemSource.volume = volume;
        }
    }

    public void PlayEffect(Effect effect)
    {
        if(_effectSource == null)
        {
            return;
        }

        switch(effect)
        {
            case Effect.Back:
                _effectSource.clip = _back;
                _effectSource.Play();
                break;
            case Effect.Button:
                _effectSource.clip = _button;
                _effectSource.Play();
                break;
            case Effect.Erase:
                _effectSource.clip = _erase;
                _effectSource.Play();
                break;
            case Effect.Countdown:
                _effectSource.clip = _countdown;
                _effectSource.Play();
                break;
            case Effect.Item:
                _itemSource.clip = _item;
                _itemSource.Play();
                break;
            case Effect.Correct:
                _effectSource.clip = _correct;
                _effectSource.Play();
                break;
            case Effect.Ground:
                _playerSource.clip = _ground;
                _playerSource.Play();
                break;
            case Effect.Gameover:
                _effectSource.clip = _gameOver;
                _effectSource.Play();
                break;
            case Effect.Wrong:
                _effectSource.clip = _wrong;
                _effectSource.Play();
                break;
            case Effect.Punch:
                _monsterSource.clip = _punch;
                _monsterSource.Play();
                break;
        }
    }
}

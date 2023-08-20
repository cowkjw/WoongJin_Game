using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 씬마다 해당 스크립트를 UIManager에 추가하고 배경음악과 설정을 연결한다.
public class SoundManager : MonoBehaviour
{
    [Header("사운드 트랙")]
    public AudioClip BackGroundSound;

    [Header("설정")]
    public Slider BGSound;
    public Slider EffectSound;

    private AudioSource audioSource;

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
        if (audioSource != null)
        {
            audioSource.volume = volume * 0.2f;
        }
    }
}

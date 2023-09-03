using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어의 HP에 관여하는 스크립트
// 플레이어의 피격에 대한 처리를 담당한다.

public class SCharacterHp : MonoBehaviour
{
    [SerializeField] private float _hp = 100f;
    [SerializeField] private float _maxHp = 100f;

    private SGameUIManager gameUIManager;
    private bool _isDead = false;
    void Start()
    {
        // TODO : 게임 UI상의 캐릭터 HP바와 연동
        gameUIManager = GameObject.Find("UIManager").GetComponent<SGameUIManager>();
    }

    void Update()
    {

    }

    public float GetHp()
    {
        return _hp;
    }

    public void Heal(float val)
    {
        float newHp = _hp + val;
        if(newHp > _maxHp)
        {
            newHp = _maxHp;
        }

        _hp = newHp;

        // TODO : UI를 갱신한다.
        float value = _hp / _maxHp;
        gameUIManager.UpdateHp(value);
    }

    public void Damage(float damage)
    {
        if (_isDead)
            return;

        float newHp = _hp - damage;
        if (newHp <= 0)
        {
            newHp = 0;
            _isDead = true;
            // TODO : 플레이어가 죽었다는 것에 대한 작업 처리
            GameObject.Find("GameManager").GetComponent<SGameManager>().EndGame(false);
        }
        _hp = newHp;
        Debug.Log(_hp);

        // 효과음 출력
        GameObject.Find("UIManager").GetComponent<SoundManager>().PlayEffect(Effect.Punch);

        // TODO : UI를 갱신한다.
        float value = _hp / _maxHp;
        gameUIManager.UpdateHp(value);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어의 HP에 관여하는 스크립트
// 플레이어의 피격에 대한 처리를 담당한다.

public class SCharacterHp : MonoBehaviour
{
    private float _hp = 100f;

    void Start()
    {
        // TODO : 게임 UI상의 캐릭터 HP바와 연동

    }

    void Update()
    {

    }

    public void Damage(float damage)
    {
        float newHp = _hp - damage;
        if (newHp <= 0)
        {
            newHp = 0;

            // TODO : 플레이어가 죽었다는 것에 대한 작업 처리
        }
        _hp = newHp;
        Debug.Log(_hp);

        // TODO : UI를 갱신한다.
        
    }
}

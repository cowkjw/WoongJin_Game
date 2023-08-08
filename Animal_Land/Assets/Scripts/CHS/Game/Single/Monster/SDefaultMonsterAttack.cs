using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 몬스터가 기본적으로 취하는 공격 기술
// 해당 스크립트를 포함한 몬스터와 부딪힌 플레이어는 체력을 잃고 이전 위치로 돌아간다.

public class SDefaultMonsterAttack : MonoBehaviour
{
    private float _attackDamage = 10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            // TODO : 플레이어에게 데미지를 준다.
            other.GetComponent<SCharacterHp>().Damage(_attackDamage);

            // TODO : 플레이어를 기존 위치로 보낸다.
            other.GetComponent<STileColorChange>().ResetMoveTileList();
        }
    }
}

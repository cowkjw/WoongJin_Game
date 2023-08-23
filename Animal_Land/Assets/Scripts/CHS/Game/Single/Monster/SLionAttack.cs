using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SLionAttack : MonoBehaviour
{
    [Header("공격 패턴 관련")]
    [SerializeField] private float _attackCoolTime;
    [SerializeField] private float _attackMaxCoolTime;

    [SerializeField] private float _attackTime;         // 스킬 지속시간

    [Header("스킬")]
    [SerializeField] private GameObject _attackObject;

    SMonsterMove monsterMove;

    // Start is called before the first frame update
    void Start()
    {
        monsterMove = GetComponent<SMonsterMove>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckCoolTime())
        {
            Attack();
        }
    }

    void Attack()
    {
        // 스킬 활성화  &&  움직임 봉쇄
        _attackObject.SetActive(true);
        monsterMove.SetCanMove(false);

        // 일정 시간 이후 스킬 종료
        Invoke("SkillEnd", _attackTime);
    }

    void SkillEnd()
    {
        // 스킬 비활성화 && 움직임 활성화
        _attackObject.SetActive(false);
        monsterMove.SetCanMove(true);
    }

    bool CheckCoolTime()
    {
        if (_attackCoolTime <= _attackMaxCoolTime)
        {
            _attackCoolTime += Time.deltaTime;
            return false;
        }
        else
        {
            _attackCoolTime = 0f;
            return true;
        }
    }
}

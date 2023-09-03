using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 일정 시간 간격으로 투사체 공격을 날린다. (발바닥 모양)
// 

public class SBearAttack : MonoBehaviour
{
    [Header("공격 패턴 관련")]
    [SerializeField] private float _attackCoolTime;
    [SerializeField] private float _attackMaxCoolTime;

    [SerializeField] private float _attackDamage;
    [SerializeField] private float _attackRange;

    [Header("투사체")]
    [SerializeField] private GameObject _attackObject;

    // Start is called before the first frame update
    void Start()
    {
        _attackDamage = 30f;
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
        // 상.하.좌.우 중에 가장 플레이어와 가까운 방향으로 투사체를 발사한다.
        Vector3 dir = GetAttackDir();
        if(dir == null || dir == Vector3.zero)
        {
            return;
        }

        // 투사체 생성 후, 초기화
        GameObject attackObject = Instantiate(_attackObject);
        attackObject.transform.position = this.transform.position;

        SBearAttackSkill sBearAttackSkill = attackObject.GetComponent<SBearAttackSkill>();
        if (sBearAttackSkill != null)
        {
            sBearAttackSkill.SetDir(dir);
        }
    }

    Vector3 GetAttackDir()
    {
        // 나아갈 방향 계산
        Vector3 myPos = transform.position;
        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;

        int randNum = Random.Range(0, 4);
        switch(randNum)
        {
            case 0:
                return Vector3.up;
            case 1:
                return Vector3.down;
            case 2:
                return Vector3.right;
            case 3:
                return Vector3.left;
        }

        return Vector3.zero;
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

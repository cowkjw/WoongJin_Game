using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBearAttackSkill : MonoBehaviour
{
    [SerializeField] Vector3    _dir = Vector3.zero;
    [SerializeField] float      _speed;

    [SerializeField] float      _time;
    [SerializeField] float      _maxTime;

    [SerializeField] float      _damage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null)
        {
            return;
        }

        GameObject player = collision.gameObject; 

        if(player.tag == "Player")
        {
            player.GetComponent<SCharacterHp>().Damage(_damage);
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(_time < _maxTime)
        {
            Move();
            CheckBorder();
            _time += Time.deltaTime;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void CheckBorder()
    {
        // TODO : 게임 밖으로 나가면 사라진다.

    }

    private void Move()
    {
        this.transform.Translate(_dir * _speed * Time.deltaTime);
    }

    public void SetDir(Vector3 dir)
    {
        _dir = dir;
    }
}

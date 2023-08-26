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
            player.GetComponent<STileColorChange>().ResetMoveTileList();
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
        // Game Screen의 최대 크기
        Vector3 lt = new Vector3(-8.05f, 2.28f, 0);
        Vector3 rb = new Vector3(3.59f, -4.23f, 0);

        Vector3 newPos = transform.position;

        if (   transform.position.x < lt.x
            || transform.position.x > rb.x
            || transform.position.y > lt.y
            || transform.position.y < rb.y)
        {
            Destroy(this.gameObject);
        }
        else
        {
            return;
        }
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

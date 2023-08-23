using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SMonsterMove : MonoBehaviour
{
    [SerializeField]
    Color _playerAreaColor;
    Vector3 _dir = Vector3.zero;

    [SerializeField] float _speed;  
    float _maxDirChangeTime = 1f;
    float _dirChangeTime = 0f;
    bool _isInit = false;

    Tilemap _tilemap;

    bool _canMove = true;

    void Start()
    {
        _speed = 1f;
    }

    void Update()
    {
        if (_isInit == false
            || _canMove == false)
        {
            return;
        }

        UpdateDir();
        Move();
    }

    public void Init()
    {
        STileColorChange player = GameObject.FindGameObjectWithTag("Player").GetComponent<STileColorChange>();
        _tilemap = player.GetTileMap();
        _playerAreaColor = player.GetAreaColor();

        _isInit = true;
    }

    public Tilemap GetTilemap()
    {
        return _tilemap;
    }

    private void UpdateDir()
    {
        if(_maxDirChangeTime <= _dirChangeTime)
        {
            SetNewDir();
        }
        else
        {
            _dirChangeTime += Time.deltaTime;
        }
    }

    public void SetCanMove(bool canMove)
    {
        _canMove = canMove;
    }

    private void Move()
    {
        transform.Translate(_speed * _dir * Time.deltaTime);

        BorderCheck();

        PlayerAreaCheck();
    }

    private void BorderCheck()
    {
        // Game Screen의 최대 크기
        Vector3 lt = new Vector3(-8.05f, 2.28f, 0);
        Vector3 rb = new Vector3(3.59f, -4.23f, 0);

        Vector3 newPos = transform.position;

        if (transform.position.x < lt.x)
        {
            newPos = new Vector3(lt.x, transform.position.y, 0);
        }
        else if (transform.position.x > rb.x)
        {
            newPos = new Vector3(rb.x, transform.position.y, 0);
        }
        else if (transform.position.y > lt.y)
        {
            newPos = new Vector3(transform.position.x, lt.y, 0);
        }
        else if (transform.position.y < rb.y)
        {
            newPos = new Vector3(transform.position.x, rb.y, 0);
        }
        else
        {
            return;
        }
        transform.position = newPos;
    }

    private void PlayerAreaCheck()
    {   
        if(_tilemap == null || _playerAreaColor == null)
        {
            return;
        }
        else
        {
            while (true)
            {
                Vector3 nextPos = transform.position + _dir * Time.deltaTime;
                if (_tilemap.GetColor(_tilemap.WorldToCell(nextPos)) == _playerAreaColor)
                {
                    SetNewDir();
                }
                else
                {
                    break;
                }
            }
        }
    }

    private void SetNewDir()
    {
        _dir = GetNewDir();
        _dirChangeTime = 0f;
    }

    private Vector3 GetNewDir()
    {
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

    public Color GetPlayerAreaColor()
    {
        return _playerAreaColor;
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

enum Type
{
    Time = 0,
    Speed,
    Hp,
    Gauge
}

public class SItem : MonoBehaviour
{
    [Header("아이템 타입")]
    [SerializeField] private Type _type;

    private SGameManager _gameManager;

    public void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<SGameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gameObject = collision.gameObject;
        if(gameObject.tag == "Player")
        {
            switch(_type)
            {
                case Type.Time:
                    Time();  break;
                case Type.Speed:
                    Speed(); break;
                case Type.Hp:
                    Hp(); break;
                case Type.Gauge:
                    Gauge(); break;
            }

            // 효과음 출력
            GameObject.Find("UIManager").GetComponent<SoundManager>().PlayEffect(Effect.Item);

            Destroy(this.gameObject);
        }
    }

    public void Hp()
    {
        _gameManager.DropItemHp();
    }

    public void Time()
    {
        _gameManager.DropItemTime();
    }

    public void Gauge()
    {
        _gameManager.DropItemGauge();
    }

    public void Speed()
    {
        _gameManager.DropItemSpeed();
    }
}

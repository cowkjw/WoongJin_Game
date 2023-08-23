using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SLionAttackSkill : MonoBehaviour
{
    [SerializeField] float _damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null)
        {
            return;
        }

        GameObject player = collision.gameObject;

        if (player.tag == "Player")
        {
            player.GetComponent<SCharacterHp>().Damage(_damage);
            player.GetComponent<STileColorChange>().ResetMoveTileList();
        }
    }
}

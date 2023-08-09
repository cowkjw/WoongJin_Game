using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SMonsterDefaultAttack : MonoBehaviour
{
    float _attackDamage = 10f;
    Color _playerMoveColor = Color.black;
    Tilemap _tilemap;

    // Update is called once per frame
    void Update()
    {
        PlayerMoveTileCheck();
    }

    private void PlayerMoveTileCheck()
    {
        if(_playerMoveColor == Color.black || _tilemap == null)
        {
            SetPlayerMoveColor();
            return;
        }

        if (_tilemap != null)
        {
            Vector3Int pos = _tilemap.WorldToCell(this.transform.position);
            if (_tilemap.GetColor(pos) == _playerMoveColor)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if(player != null)
                {
                    player.GetComponent<SCharacterHp>().Damage(_attackDamage);

                    player.GetComponent<STileColorChange>().ResetMoveTileList();
                }
            }
        }
    }

    private void SetPlayerMoveColor()
    {
        STileColorChange player = GameObject.FindGameObjectWithTag("Player").GetComponent<STileColorChange>();
        _playerMoveColor = player.GetMoveColor();
        _tilemap = player.GetTileMap();
    }

    private void OnTriggerEnter2D(Collider2D Object)
    {
        if(Object.tag == "Player")
        {
            Object.GetComponent<SCharacterHp>().Damage(_attackDamage * 2);

            Object.GetComponent<STileColorChange>().ResetMoveTileList();
        }
    }
}

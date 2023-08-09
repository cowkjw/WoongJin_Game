using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SMonster : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AreaCheck()
    {
        Tilemap tilemap = GetComponent<SMonsterMove>().GetTilemap();
        if(tilemap != null)
        {
            Vector3Int pos = tilemap.WorldToCell(this.transform.position);
            if(tilemap.GetColor(pos) == GetComponent<SMonsterMove>().GetPlayerAreaColor())
            {
                Dead();
            }
        }
    }

    private void Dead()
    {
        GameObject.Destroy(this.gameObject);
    }
}

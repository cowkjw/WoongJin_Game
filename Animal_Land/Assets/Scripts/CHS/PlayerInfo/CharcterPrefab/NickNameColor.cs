using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class NickNameColor : MonoBehaviour
{
    public TextMeshProUGUI NickNameText;

    private void Start()
    {
        if(GetComponent<PhotonView>().IsMine == true)
        {
            NickNameText.color = Color.green;
        }
    }

    private void Update()
    {
    }

    [PunRPC]
    void UpdateColor(Vector3 pos)
    {
        Tilemap tilemap = GameObject.Find("GameManager").GetComponent<GameManager>().tileMap;
        Vector3Int cellPos = tilemap.WorldToCell(pos);

        Debug.Log("Set Color");
        tilemap.SetTileFlags(cellPos, TileFlags.None);
        tilemap.SetColor(cellPos, Color.red);
    }
}

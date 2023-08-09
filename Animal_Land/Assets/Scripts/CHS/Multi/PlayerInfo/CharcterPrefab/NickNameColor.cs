using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NickNameColor : MonoBehaviour
{
    public TextMeshProUGUI NickNameText;

    private void Start()
    {
        if(SceneManager.GetActiveScene().name == "Single Game Room")
        {
            NickNameText.color = Color.green;
        }
        else if(GetComponent<PhotonView>().IsMine == true)
        {
            NickNameText.color = Color.green;
        }
    }

    private void Update()
    {
    }
}

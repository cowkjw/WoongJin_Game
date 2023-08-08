using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SNickNameColor : MonoBehaviour
{
    public TextMeshProUGUI NickNameText;

    private void Start()
    {
        NickNameText.color = Color.green;
    }
}

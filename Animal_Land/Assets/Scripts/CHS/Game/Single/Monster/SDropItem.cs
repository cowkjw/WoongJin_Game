using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDropItem : MonoBehaviour
{
    [SerializeField] private GameObject[] Items = new GameObject[4];

    public void DropItem()
    {
        // 랜덤으로 아이템 선택
        int rand = Random.Range(0, Items.Length);

        // 아이템 생성 및 배치
        GameObject Item = Instantiate(Items[rand]);
        Item.transform.position = this.transform.position;
    }
}

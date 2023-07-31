using Contents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerDownHandler
{

    public ItemInfo SlotInfo { get; private set; }
    public int SlotIndex { get; private set; }
    bool isCanClick;

    void Start()
    {
        SlotIndex = transform.GetSiblingIndex();
        var element = DataManager.Instance.HatItemInfoList.ElementAtOrDefault(SlotIndex); // 리스트에 해당 인덱스 데이터가 있다면
        if (element != default)
        {
            SlotInfo = element;
            isCanClick = true;
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogError($"{SlotIndex} 번째 슬롯 가격 설정 오류");
#endif
            isCanClick = false;
            this.GetComponent<Image>().color = new Color(0, 0, 0);// 굳이 이미지를 보여줄 필요 없음
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isCanClick) return; // 선택을 못하는 슬롯이라면 

        string message;
        bool notHave= true;
        if (DataManager.Instance.PlayerData.ShoppingList.ContainsKey(SlotInfo.Name))
        {
            message = "이미 구매한 아이템입니다.";
            notHave = false;
        }
        else
        {
            message = $"{SlotInfo.Price} Gold가 소모됩니다.\n{SlotInfo.Name}를 구매하시겠습니까?";
        }
        
        PurchasePopUp purchasePopUp = ViewManager.GetView<PurchasePopUp>();
        if (purchasePopUp != null)
        {
            purchasePopUp.ClickSlotItemInfo = SlotInfo;
            purchasePopUp.checkAction?.Invoke(notHave); //
            purchasePopUp.SetCheckMessage(message); // 메시지 넘기기
        }


#if UNITY_EDITOR
        Debug.Log($"{SlotIndex} 번째 슬롯 클릭");
#endif
    }
}

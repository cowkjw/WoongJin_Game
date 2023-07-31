using Contents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchasePopUp : PopUpUI, IStatusCheckPopUP
{
    public Action<bool> checkAction; 
    public ItemInfo ClickSlotItemInfo { get; set; } // 전달 받은 슬롯 아이템 정보

    [SerializeField]
    private List<Button> selectBtns;
    [SerializeField]
    Text checkMessage;

    public override void Initialize()
    {
        base.Initialize();

        SetCheckMessage("구매 아이템을 선택해주세요.");
        checkAction += ActivateButtons;
        for (int i = 0; i < selectBtns.Count; i++)
        {
            int index = i; // 변수를 캡쳐하기 때문에 i를 안넣고 로컬로 따로 변수에 할당해서 사용
            selectBtns[i].onClick.AddListener(() => OnSelectPurchase(index));
            selectBtns[i].gameObject.SetActive(false);
        }
    }

    void ActivateButtons(bool flag)
    {
        closeButton?.gameObject.SetActive(!flag); // 반대로 설정해야지 겹쳐 안보임

        foreach (var button in selectBtns)
        {
            button.gameObject.SetActive(flag);
        }
    }

    void OnSelectPurchase(int index)
    {
        switch (index)
        {
            case 0: // 구매
                // TODO : 골드 확인하기 

                PurchaseCheckPopUp popUpUI = ViewManager.GetView<PurchaseCheckPopUp>();
                if (popUpUI != null && popUpUI.name == "Selection_Result_PopUp") // 팝업 창 이름이 동일한지
                {
                    if (DataManager.Instance.PlayerData != null)
                    {
                        if(ClickSlotItemInfo != null) // 선택한 게 있으면
                        {
                            string message = CheckPlayerGold(ClickSlotItemInfo.Price);
                            popUpUI.SetCheckMessage(message);
                        }
                    }
                    ViewManager.Show<PurchaseCheckPopUp>(true, true); // 구매 확인 창
                }
                break;
            case 1: // 취소
                ViewManager.ShowLast(); // 이전 화면으로 
                blocker.gameObject?.SetActive(false); // 블로커 비활성화
                break;
        }
    }


    string CheckPlayerGold(int price)
    {
        int playerGold = DataManager.Instance.PlayerData.Gold;

        if (playerGold >= price) // 살 수 있으면
        {
            DataManager.Instance.PlayerData.Gold -= price;
            ViewManager.GetView<ShopMenuView>().OnShopClick?.Invoke();
            DataManager.Instance.PlayerData.ShoppingList.Add(ClickSlotItemInfo.Name, true);
            DataManager.Instance.SaveData<PlayerData>(DataManager.Instance.PlayerData, "PlayerData");
            return "구매 완료!";
        }
        else
        {
            int difference = Math.Abs(playerGold - price);
            return $"{difference} GOLD가 부족합니다.";
        }
    }

    public void SetCheckMessage(string message)
    {
        checkMessage.text = message;
    }


}

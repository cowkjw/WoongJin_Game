using Contents;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchasePopUp : PopUpUI, IStatusCheckPopUP
{
    public Action<bool> checkAction; // 외부에서 사용 할 델리게이트
    public ItemInfo ClickSlotItemInfo { get; set; } // 전달 받은 슬롯 아이템 정보

    [SerializeField] private List<Button> selectBtns;
    [SerializeField] private Text checkMessage;
    [SerializeField] List<Sprite> checkSprites;
    Image image;
    public override void Initialize()
    {
        base.Initialize();

        SetCheckMessage("Choose the purchase item"); // 기본 메시지
        checkAction += ActivateButtons; // 델리게이트에 버튼 활성화/비활성화 함수 추가
        for (int i = 0; i < selectBtns.Count; i++)
        {
            int index = i; // 변수를 캡쳐하기 때문에 i를 안넣고 로컬로 따로 변수에 할당해서 사용
            selectBtns[i]?.onClick.AddListener(() => OnSelectPurchase(index));
            selectBtns[i]?.onClick.AddListener(() => ShopManager.Instance.InitSlotClicked());
            selectBtns[i]?.gameObject.SetActive(false);
        }
        closeButton?.onClick.AddListener(() => ShopManager.Instance.InitSlotClicked());
    }

    void ActivateButtons(bool flag) // 버튼들 활성화
    {
        closeButton?.gameObject.SetActive(!flag); // 반대로 설정해야지 겹쳐 안보임

        foreach (var button in selectBtns) // 예, 아니오 버튼 활성화
        {
            button.gameObject.SetActive(flag);
        }
    }

    void OnSelectPurchase(int index) // 구매 버튼 
    {
        switch (index)
        {
            case 0: // 구매
                PurchaseCheckPopUp popUpUI = ViewManager.GetView<PurchaseCheckPopUp>();
                if (popUpUI != null && popUpUI.name == "Selection_Result_PopUp") // 팝업 창 이름이 동일한지
                {
                    if (DataManager.Instance.PlayerData != null)
                    {
                        if(ShopManager.Instance.ItemInfo != null) // 선택한 게 있으면
                        {
                            string message = CheckPlayerGold(ShopManager.Instance.ItemInfo.Price);
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
            DataManager.Instance.PlayerData.Gold -= price; // 골드 차감
            DataManager.Instance.PlayerData.ShoppingList.Add(ShopManager.Instance.ItemInfo.Name); // 구매 리스트에 추가
            DataManager.Instance.SavePlayerData(); // 플레이어 데이터 저장
            ViewManager.GetView<ShopMenuView>().OnPurchaseAction?.Invoke(); // 상점 UI 업데이트

            return "Complete purchase!";
        }
        else
        {
            int difference = Math.Abs(playerGold - price); // 부족한 금액
            return $"You ard {difference} Gold short"; 
        }
    }

    public void SetCheckMessage(string message)
    {
        checkMessage.text = message;
    }
}

using Contents;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerDownHandler
{

    public ItemInfo SlotInfo { get; private set; }
    public int SlotIndex { get; private set; }
    bool isCanClick; // 클릭할 수 있는지
    ItemType _itemType = ItemType.Face; // 아이템 타입 설정

    void Start()
    {
        SlotIndex = transform.GetSiblingIndex();
        InitializeSlot(_itemType); // 기본은 얼굴 장식 Slot 초기화
        ShopManager.Instance.UpdateShopItemListAction += InitializeSlot;
        ViewManager.GetView<ShopMenuView>().OnShopClick += ResetSlot;
    }


    void InitializeSlot(ItemType itemType) // 슬롯 정보 셋팅
    {

        _itemType = itemType; // 현재 슬롯의 아이템 타입 설정

        var element = DataManager.Instance.PropsItemDict[itemType.ToString()].ElementAtOrDefault(SlotIndex); // 리스트에 해당 인덱스 데이터가 있다면

        if (element != default)
        {
            SlotInfo = element;
            isCanClick = true;
            if (DataManager.Instance.PlayerData.ShoppingList.Count > 0 && DataManager.Instance.PlayerData.ShoppingList.ContainsKey(SlotInfo.Name))
            {
                this.GetComponent<Image>().color = new Color(255, 255, 255, 1);
            }
            else
            {
                this.GetComponent<Image>().color = new Color(0, 0, 0, 1);
            }
        }
        else
        {
            isCanClick = false;
            this.GetComponent<Image>().color = new Color(0, 0, 0, 0);// 굳이 이미지를 보여줄 필요 없음
        }
    }

    void ResetSlot() // 상점에 변화가 있을 때 리셋
    {
        InitializeSlot(_itemType);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isCanClick) return; // 선택을 못하는 슬롯이라면 

        string message; // 전달 할 메시지
        bool notHave = true; // 가지고 있는 아이템인지

        if (DataManager.Instance.PlayerData.ShoppingList.ContainsKey(SlotInfo.Name)) // 이미 구입한 아이템이라면
        {
            message = "장착 완료.";
            notHave = false;
            ViewManager.GetView<ShopMenuView>().ToggleBuy = false;
        }
        else
        {
            message = $"{SlotInfo.Price} Gold가 소모됩니다.\n{SlotInfo.Name}를 구매하시겠습니까?";
            ViewManager.GetView<ShopMenuView>().ToggleBuy = true;
        }

        ViewManager.GetView<ShopMenuView>()?.UpdateBuyText(notHave); // 구매 버튼에 장착으로 변경할지 선택 후 업데이트
        ShopManager.Instance.ItemInfo = SlotInfo;

        PurchasePopUp purchasePopUp = ViewManager.GetView<PurchasePopUp>();
        if (purchasePopUp != null)
        {
            purchasePopUp.checkAction?.Invoke(notHave);
            purchasePopUp.SetCheckMessage(message); // 메시지 넘기기
        }


#if UNITY_EDITOR
        Debug.Log($"{SlotIndex} 번째 슬롯 클릭");
#endif
    }
}

using Contents;
using System;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerDownHandler
{

    public ItemInfo SlotInfo { get; private set; }
    public int SlotIndex { get; private set; }
    public Action<int> ClickSlotAction = null;
    public bool isCanClick { get; private set; } // 클릭할 수 있는지   
    public bool unLock { get; private set; } // 구매했는지   
    ItemType _itemType = ItemType.Face; // 아이템 타입 설정
    Image _itemImage;
    GameObject _lock;

    void Start()
    {

        SlotIndex = transform.GetSiblingIndex();
        InitializeSlot(_itemType); // 기본은 얼굴 장식 Slot 초기화
        ShopManager.Instance.UpdateShopItemListAction += InitializeSlot; // 슬롯 초기화
        ViewManager.GetView<ShopMenuView>().OnShopClick += ResetSlot;
    }


    void InitializeSlot(ItemType itemType) // 슬롯 정보 셋팅
    {
        if (_lock == null)
        {
            _lock = transform.GetChild(0).gameObject;
        }
        _itemType = itemType; // 현재 슬롯의 아이템 타입 설정

        var element = DataManager.Instance.PropsItemDict[itemType.ToString()].ElementAtOrDefault(SlotIndex); // 리스트에 해당 인덱스 데이터가 있다면

        if (element != default)
        {
            SlotInfo = element;
            isCanClick = true;
            if (DataManager.Instance.PlayerData.ShoppingList.Count > 0 && DataManager.Instance.PlayerData.ShoppingList.ContainsKey(SlotInfo.Name)) // 이미 구입한 상품이라면
            {
                this.GetComponent<Image>().color = new Color(255, 255, 255, 1);
                unLock = true;
                _lock.gameObject?.SetActive(false); // 자물쇠 비활성화 
            }
            else
            {
                this.GetComponent<Image>().color = new Color(0, 0, 0, 1);
                unLock = false;
                _lock.gameObject?.SetActive(true); // 자물쇠 활성화
            }
        }
        else
        {
            isCanClick = false;
            this.GetComponent<Image>().color = new Color(0, 0, 0, 0);// 굳이 이미지를 보여줄 필요 없음
            _lock.gameObject?.SetActive(false); // 자물쇠 비활성화 
        }

        UpdateItemImage();
    }

    void UpdateItemImage() // 슬롯 아이템의 이미지 업데이트
    {
        if (SlotInfo == null) return; // 현재 슬롯에는 들어 갈 아이템이 없는 경우임 

        _itemImage = GetComponent<Image>();
        if (_itemImage == null)
        {
            _itemImage = GetComponent<Image>();
        }

        Sprite itemSprite = Resources.Load<Sprite>($"Sprites/Items/{_itemType.ToString()}/{SlotInfo.Name}");
        if (itemSprite != null)
        {
            _itemImage.sprite = itemSprite;
        }
        else
        {
            _itemImage.sprite = null;
#if UNITY_EDITOR
            Debug.LogError($"리소스 폴더에 아이템 {SlotInfo.Name} 이미지가 없습니다.");
#endif  
        }
    }

    void ResetSlot() // 상점에 변화가 있을 때 리셋
    {
        InitializeSlot(_itemType);
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isCanClick) return; // 선택을 못하는 슬롯이라면 

        ClickSlotAction?.Invoke(SlotIndex);
        string message; // 전달 할 메시지
        bool notHave = true; // 가지고 있는 아이템인지
        ShopMenuView shopMenu = ViewManager.GetView<ShopMenuView>();
        if (DataManager.Instance.PlayerData.ShoppingList.ContainsKey(SlotInfo.Name)) // 이미 구입한 아이템이라면
        {
            message = "이미 구매한 아이템입니다.";
            notHave = false;
            if (shopMenu != null)
            {
                shopMenu.canPutOn = true;
            }
        }
        else
        {
            message = $"{SlotInfo.Price} Gold가 소모됩니다.\n{SlotInfo.Name}를 구매하시겠습니까?";
            if (shopMenu != null)
            {
                shopMenu.canPutOn = false;
            }
        }

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

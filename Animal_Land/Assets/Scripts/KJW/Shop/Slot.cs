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
    public bool IsClicked { get; set; } = false;
    ItemType _itemType = ItemType.Face; // 아이템 타입 설정
    Image _itemImage;
    GameObject _lock;
    void Start()
    {

        SlotIndex = transform.GetSiblingIndex();
        InitializeSlot(_itemType); // 기본은 얼굴 장식 Slot 초기화
        ShopManager.Instance.UpdateShopItemListAction += InitializeSlot; // 슬롯 초기화
        ShopMenuView shopMenuView = ViewManager.GetView<ShopMenuView>();
        if(shopMenuView != null )
        {
            shopMenuView.OnPurchaseAction += ResetSlot;
            shopMenuView.OnShopClick += ResetSlot;
            shopMenuView.OnChangeCharacterAction += UpdateItemImage;
        }
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
            this.GetComponent<Image>().color = new Color(255, 255, 255, 1);
            if (DataManager.Instance.PlayerData.ShoppingList.Count > 0 && DataManager.Instance.PlayerData.ShoppingList.Contains(SlotInfo.Name)) // 이미 구입한 상품이라면
            {
                unLock = true;
                _lock.gameObject?.SetActive(false); // 자물쇠 비활성화 
            }
            else
            {
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

        IsClicked = false;
    }

    void UpdateItemImage() // 슬롯 아이템의 이미지 업데이트
    {
        if (SlotInfo == null) return; // 현재 슬롯에는 들어 갈 아이템이 없는 경우임 

        _itemImage = GetComponent<Image>();
        if (_itemImage == null)
        {
            _itemImage = GetComponent<Image>();
        }

        Sprite itemSprite = null;
        if (_itemType==ItemType.Face) // 얼굴일 때만 각자 캐릭터 얼굴 띄우기
        {
            string ch = string.Empty; // 캐릭터 앞 철자만 따오기
            switch (ShopManager.Instance.CharacterType)
            {
                case CharacterType.Bird:
                    ch = "b";
                    break;
                case CharacterType.Cat:
                    ch = "c";
                    break;
                case CharacterType.Dog:
                    ch = "d";
                    break;
                case CharacterType.Frog:
                    ch = "f";
                    break;
            }
            itemSprite = Resources.Load<Sprite>($"Sprites/Items/{_itemType.ToString()}/{ch + SlotInfo.Name}");
        }
        else
        {
            itemSprite = Resources.Load<Sprite>($"Sprites/Items/{_itemType.ToString()}/{SlotInfo.Name}");
        }

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

        string message; // 전달 할 메시지
        bool notHave = true; // 가지고 있는 아이템인지
        ShopMenuView shopMenu = ViewManager.GetView<ShopMenuView>();
        if (DataManager.Instance.PlayerData.ShoppingList.Contains(SlotInfo.Name)) // 이미 구입한 아이템이라면
        {
            message = "Already been purchased";
            
            notHave = false;
            if (shopMenu != null)
            {
                shopMenu.canPutOn = true;
            }
        }
        else
        {
            message = $"{SlotInfo.Price} Gold will be consumed.\nDo you want to buy it?";
            if (shopMenu != null)
       
     {
                shopMenu.canPutOn = false;
            }
        }

        ClickSlotAction?.Invoke(SlotIndex);
        if(SlotInfo==ShopManager.Instance.ItemInfo) // 지금 상점에서 이미 장착하고 있는거라면
        {
            ShopManager.Instance.ItemInfo = null; // 아이템 정보를 null로
            ShopManager.Instance.CharacterCustom.ItemDict[ShopManager.Instance.ItemType.ToString()] = " "; // 임시 커스텀에 공백으로 처리
            ShopManager.Instance.cManager.ChangeCharacterSpecificPartsSprite(_itemType); // 특정 아이템 스프라이트 변경
            this.GetComponent<Image>().color = Color.white;
        }
        else
        {
            ShopManager.Instance.ItemInfo = SlotInfo; // 상점에 선택 된 아이템 정보를 슬롯의 아이템 정보로 업데이트
            ShopManager.Instance.SetCharacterCustom(); // 현재 누른 아이템을 캐릭터에 임시로 장착
            ShopManager.Instance.cManager.ChangeCharacterSpecificPartsSprite(_itemType); // 특정 아이템 스프라이트 변경
        }
       
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

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
    public bool isCanClick { get; private set; } // Ŭ���� �� �ִ���   
    public bool unLock { get; private set; } // �����ߴ���   
    public bool IsClicked { get; set; } = false;
    ItemType _itemType = ItemType.Face; // ������ Ÿ�� ����
    Image _itemImage;
    GameObject _lock;
    void Start()
    {

        SlotIndex = transform.GetSiblingIndex();
        InitializeSlot(_itemType); // �⺻�� �� ��� Slot �ʱ�ȭ
        ShopManager.Instance.UpdateShopItemListAction += InitializeSlot; // ���� �ʱ�ȭ
        ShopMenuView shopMenuView = ViewManager.GetView<ShopMenuView>();
        if(shopMenuView != null )
        {
            shopMenuView.OnPurchaseAction += ResetSlot;
            shopMenuView.OnShopClick += ResetSlot;
            shopMenuView.OnChangeCharacterAction += UpdateItemImage;
        }
    }


    void InitializeSlot(ItemType itemType) // ���� ���� ����
    {
        if (_lock == null)
        {
            _lock = transform.GetChild(0).gameObject;
        }
        _itemType = itemType; // ���� ������ ������ Ÿ�� ����

        var element = DataManager.Instance.PropsItemDict[itemType.ToString()].ElementAtOrDefault(SlotIndex); // ����Ʈ�� �ش� �ε��� �����Ͱ� �ִٸ�
        
        if (element != default)
        {
            SlotInfo = element;
            isCanClick = true;
            this.GetComponent<Image>().color = new Color(255, 255, 255, 1);
            if (DataManager.Instance.PlayerData.ShoppingList.Count > 0 && DataManager.Instance.PlayerData.ShoppingList.Contains(SlotInfo.Name)) // �̹� ������ ��ǰ�̶��
            {
                unLock = true;
                _lock.gameObject?.SetActive(false); // �ڹ��� ��Ȱ��ȭ 
            }
            else
            {
                unLock = false;
                _lock.gameObject?.SetActive(true); // �ڹ��� Ȱ��ȭ
            }
        }
        else
        {
            isCanClick = false;
            this.GetComponent<Image>().color = new Color(0, 0, 0, 0);// ���� �̹����� ������ �ʿ� ����
            _lock.gameObject?.SetActive(false); // �ڹ��� ��Ȱ��ȭ 
        }

        UpdateItemImage();

        IsClicked = false;
    }

    void UpdateItemImage() // ���� �������� �̹��� ������Ʈ
    {
        if (SlotInfo == null) return; // ���� ���Կ��� ��� �� �������� ���� ����� 

        _itemImage = GetComponent<Image>();
        if (_itemImage == null)
        {
            _itemImage = GetComponent<Image>();
        }

        Sprite itemSprite = null;
        if (_itemType==ItemType.Face) // ���� ���� ���� ĳ���� �� ����
        {
            string ch = string.Empty; // ĳ���� �� ö�ڸ� ������
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
            Debug.LogError($"���ҽ� ������ ������ {SlotInfo.Name} �̹����� �����ϴ�.");
#endif  
        }
    }

    void ResetSlot() // ������ ��ȭ�� ���� �� ����
    {
        InitializeSlot(_itemType);
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isCanClick) return; // ������ ���ϴ� �����̶�� 

        string message; // ���� �� �޽���
        bool notHave = true; // ������ �ִ� ����������
        ShopMenuView shopMenu = ViewManager.GetView<ShopMenuView>();
        if (DataManager.Instance.PlayerData.ShoppingList.Contains(SlotInfo.Name)) // �̹� ������ �������̶��
        {
            message = "Already been purchased.";
            
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
        if(SlotInfo==ShopManager.Instance.ItemInfo) // ���� �������� �̹� �����ϰ� �ִ°Ŷ��
        {
            ShopManager.Instance.ItemInfo = null; // ������ ������ null��
            ShopManager.Instance.CharacterCustom.ItemDict[ShopManager.Instance.ItemType.ToString()] = " "; // �ӽ� Ŀ���ҿ� �������� ó��
            ShopManager.Instance.cManager.ChangeCharacterSpecificPartsSprite(_itemType); // Ư�� ������ ��������Ʈ ����
            this.GetComponent<Image>().color = Color.white;
        }
        else
        {
            ShopManager.Instance.ItemInfo = SlotInfo; // ������ ���� �� ������ ������ ������ ������ ������ ������Ʈ
            ShopManager.Instance.SetCharacterCustom(); // ���� ���� �������� ĳ���Ϳ� �ӽ÷� ����
            ShopManager.Instance.cManager.ChangeCharacterSpecificPartsSprite(_itemType); // Ư�� ������ ��������Ʈ ����
        }
       
        PurchasePopUp purchasePopUp = ViewManager.GetView<PurchasePopUp>();
        if (purchasePopUp != null)
        {
            purchasePopUp.checkAction?.Invoke(notHave);
            purchasePopUp.SetCheckMessage(message); // �޽��� �ѱ��
        }

#if UNITY_EDITOR
        Debug.Log($"{SlotIndex} ��° ���� Ŭ��");
#endif
    }
}

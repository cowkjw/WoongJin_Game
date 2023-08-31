using Contents;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public Action<Contents.ItemType> UpdateShopItemListAction = null;
    public Action<int> ChangeSlotStatusAction = null;
    public ItemInfo ItemInfo { get; set; } = null;
    public CharacterType CharacterType { get; set; } = CharacterType.Bird; // ���� ���� â�� ĳ����
    public ItemType ItemType { get; set; } = ItemType.Face; // ���� ���� â�� ������ ī�װ�

    public CharacterCustom CharacterCustom { get; set; }

    [SerializeField] private List<Slot> slots; // ���� �̹�����
    public CCManager cManager { get; private set; }

    private static ShopManager instance;
    public static ShopManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ShopManager();
            }
            return instance;
        }
    }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        cManager = GetComponent<CCManager>();
        CharacterType = (CharacterType)Enum.Parse(typeof(CharacterType), DataManager.Instance.PlayerData.Character);
        LoadCharacterCustom();

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].ClickSlotAction += OnSlotClicked;
        }
    }

    private void OnEnable()
    {
        if (cManager != null) // ĳ���� Ŀ���� �Ŵ����� null�� �ƴϸ�
        {
            cManager.CharacterOriginPartsSprite();// ĳ���� ���� ���� �������� ���� �ٽ� ����
        }
        CharacterType = (CharacterType)Enum.Parse(typeof(CharacterType), DataManager.Instance.PlayerData.Character); // ĳ����Ÿ���� �ٽ� ��ǥ ĳ���ͷ� ����
                                                                                                                     // �ʱ�ȭ ���ϸ� ������ ĳ���� ��������Ʈ�� ������
    }

    public void InitSlotClicked()
    {
        if (slots.Count == 0)
            return;

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].GetComponent<Image>().color = Color.white;
        }
    }

    public void LoadCharacterCustom()
    {
        CharacterCustom = new CharacterCustom(DataManager.Instance.CharacterCustomData[CharacterType.ToString()]); // ���� ������ ���� ����
        cManager.ChangeCharacterPartsSprite();
    }

    public void SetCharacterCustom() // �����Ϸ� �ÿ� �ӽ� ĳ���� Ŀ���ҿ� �ش� �������� �����صα� ���� �Լ�
    {
        CharacterCustom.ItemDict[ItemType.ToString()] = ItemInfo.Name + "_" + CharacterType.ToString();  // ������ �̸� + ĳ���� Ÿ��
    }

    public void CheckItemList(Contents.ItemType itemType) // ������ ����Ʈ �ҷ��� �� ���
    {
        instance.UpdateShopItemListAction?.Invoke(itemType);
        instance.ItemType = itemType;
    }

    void OnSlotClicked(int slotIndex)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].isCanClick == false) continue; // Ŭ���� �ȵǴ� �����̶�� �ѱ�

            Image slotImage = slots[i].GetComponent<Image>();
            if (slotImage != null)
            {
                if (slots[i].SlotIndex == slotIndex)
                {
                    slotImage.color = Color.gray;
                }
                else
                {
                    slotImage.color = Color.white; //slots[i].unLock ? Color.white : Color.black; // �������� �������� ���� �� ��  
                }
            }
        }
    }

    private void OnDisable()
    {
        if (ItemInfo != null)
        {
            ItemInfo = null;
            ViewManager.GetView<PurchasePopUp>()?.SetCheckMessage("Choose the purchase item");
            ViewManager.GetView<PurchasePopUp>()?.checkAction(false);
        }
    }

}
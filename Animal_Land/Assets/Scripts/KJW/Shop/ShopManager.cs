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
    public CharacterType CharacterType { get; set; } = CharacterType.Frog; // 현재 상점 창의 캐릭터
    public ItemType ItemType { get; set; } = ItemType.Face; // 현재 상점 창의 아이템 카테고리

    public CharacterCustom CharacterCustom { get; set; }

    [SerializeField] private List<Slot> slots; // 슬롯 이미지들
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
        CharacterCustom = new CharacterCustom(DataManager.Instance.CharacterCustomData[CharacterType.ToString()]); // 복사 생성자 깊은 복사
        cManager.ChangeCharacterPartsSprite();
    }

    public void SetCharacterCustom() // 장착완료 시에 임시 캐릭터 커스텀에 해당 아이템을 설정해두기 위한 함수
    {
        // CharacterCustom.ItemDict[ItemType.ToString()] = ItemInfo.Name;
        CharacterCustom.ItemDict[ItemType.ToString()] = ItemInfo.Name + "_" + CharacterType.ToString();  // 아이템 이름 + 캐릭터 타입
    }

    public void CheckItemList(Contents.ItemType itemType) // 아이템 리스트 불러올 때 사용
    {
        instance.UpdateShopItemListAction?.Invoke(itemType);
        instance.ItemType = itemType;
    }

    void OnSlotClicked(int slotIndex)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].isCanClick == false) continue; // 클릭이 안되는 슬롯이라면 넘김

            Image slotImage = slots[i].GetComponent<Image>();
            if (slotImage != null)
            {
                if (slots[i].SlotIndex == slotIndex)
                {
                    slotImage.color = Color.gray;
                }
                else
                {
                    slotImage.color = Color.white; //slots[i].unLock ? Color.white : Color.black; // 구입한지 안한지에 따라 흰 검  
                }
            }
        }
    }

    private void OnDisable()
    {
        if (ItemInfo != null)
        {
            ItemInfo = null;
            ViewManager.GetView<PurchasePopUp>()?.SetCheckMessage("구매 아이템을 선택해주세요.");
            ViewManager.GetView<PurchasePopUp>()?.checkAction(false);
        }
    }

}
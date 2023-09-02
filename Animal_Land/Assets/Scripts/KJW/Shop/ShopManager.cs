using Contents;
using System;
using System.Collections.Generic;
using TexDrawLib;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public Action<Contents.ItemType> UpdateShopItemListAction = null;
    public Action<int> ChangeSlotStatusAction = null;
    public ItemInfo ItemInfo { get; set; } = null;
    public CharacterType CharacterType { get; set; } = CharacterType.Bird; // 현재 상점 창의 캐릭터
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
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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
        if (cManager != null) // 캐릭터 커스텀 매니저가 null이 아니면
        {
            cManager.CharacterOriginPartsSprite();// 캐릭터 파츠 로컬 저장으로 전부 다시 변경
        }
        CharacterType = (CharacterType)Enum.Parse(typeof(CharacterType), DataManager.Instance.PlayerData.Character); // 캐릭터타입을 다시 대표 캐릭터로 설정
                                                                                                                     // 초기화 안하면 엉뚱한 캐릭터 스프라이트로 입혀짐
    }

    public void InitSlotClicked()
    {
        if (slots.Count == 0) return;

        ItemInfo = null;
        foreach (var slot in slots)
        {
            slot.GetComponent<Image>().color = Color.white;
        }
    }

    public void LoadCharacterCustom()
    {
        CharacterCustom = new CharacterCustom(DataManager.Instance.CharacterCustomData[CharacterType.ToString()]); // 복사 생성자 깊은 복사
        cManager.ChangeCharacterPartsSprite();
    }

    public void SetCharacterCustom() // 장착완료 시에 임시 캐릭터 커스텀에 해당 아이템을 설정해두기 위한 함수
    {
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

    public void CheckItemAndSave()
    {
        if (DataManager.Instance == null) return;  

        foreach(var data in CharacterCustom.ItemDict)
        {
            string[] item = data.Value.Split("_");
            if (DataManager.Instance.PlayerData.ShoppingList.TryGetValue(item[0], out string tempItem) && tempItem != null)
            {
                if(tempItem != null) 
                {
                    tempItem += ("_" + CharacterType.ToString());
                    DataManager.Instance.CharacterCustomData[CharacterType.ToString()].ItemDict[data.Key] = tempItem;
                }
            }
            else
            {
                DataManager.Instance.CharacterCustomData[CharacterType.ToString()].ItemDict[data.Key] = " ";
            }
        }
        DataManager.Instance.SaveData<IDictionary<string, Contents.CharacterCustom>>
           (DataManager.Instance.CharacterCustomData, "CustomData");
        DataManager.Instance.ReloadData(); // 다시 저장한 데이터 불러옴
        LoadCharacterCustom();
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
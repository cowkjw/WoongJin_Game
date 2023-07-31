using Contents;
using System;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public Action<Contents.ItemType> UpdateShopItemListAction = null;
    public ItemInfo ItemInfo { get; set; } = null;
    public CharacterType CharacterType { get; set; } = CharacterType.Frog;
    public ItemType ItemType { get; set; } = ItemType.Face;

    public CharacterCustom CharacterCustom { get; set; } = new CharacterCustom();

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

    private void Start()
    {
        LoadCharacterCustom();
    }

    public void LoadCharacterCustom()
    {
        CharacterCustom = DataManager.Instance.CharacterCustomData[CharacterType.ToString()];
    }

    public void SetCharacterCustom() // 장착완료 시에 임시 캐릭터 커스텀에 해당 아이템을 설정해두기 위한 함수
    {
        CharacterCustom.ItemDict[ItemType.ToString()] = ItemInfo.Name; 
    }

    public static void CheckItemList(Contents.ItemType itemType)
    {
        instance.UpdateShopItemListAction.Invoke(itemType);
        instance.ItemType = itemType;
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

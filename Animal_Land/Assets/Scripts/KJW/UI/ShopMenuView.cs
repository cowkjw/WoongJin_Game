using Contents;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenuView : View
{
    public Action OnShopClick = null; // ���� Ŭ���� ���� ������Ʈ ��������Ʈ
    public Action OnPurchaseAction = null; // ������ ���� ���� �� ���� ��������Ʈ
    public Action OnChangeCharacterAction = null;
    public bool canPutOn;

    [SerializeField] private Button backButton; // �ڷΰ��� ��ư
    [SerializeField] private Button buyButton; // ���� ��ư
    [SerializeField] private Button putOnButton; // ���� ��ư
    [SerializeField] private Button representButton; // ��ǥ���� ��ư
    [SerializeField] private List<Button> categoryBtns; // ������ ī�װ� ��ư��
    [SerializeField] private List<Button> selectCharaceterBtns; // ĳ���� ���� ��ư
    [SerializeField] private List<Sprite> representCharaceterSprites; // ĳ���� ���� ��ư
    [SerializeField] private List<Sprite> defaultCharaceterSprites; // ĳ���� ���� ��ư
    [SerializeField] private Image characterChangeImage; // �ٲ� ĳ���� �̹���
    [SerializeField] private List<Sprite> characterSprites; // ĳ���� ��������Ʈ��
    [SerializeField] private Text goldText;
    [SerializeField] private Text buyText;

    CharacterType characterType;

    public override void Initialize()
    {
        InitButtons();
        RepresentCharacter();
        selectCharaceterBtns[(int)characterType].image.sprite = representCharaceterSprites[(int)characterType]; // ĳ���� ���� �̹��� ��ǥ ĳ���ͷ� ����
        OnShopClick += UpdateShopInterface;
        OnShopClick?.Invoke(); ; // ��� UI Text ������Ʈ
        OnPurchaseAction += DataManager.Instance.ReloadData;
        OnPurchaseAction += UpdateGoldText;

        for (int i = 0; i < selectCharaceterBtns.Count; i++) // ĳ���� ���� ��ư�� �ʱ�ȭ
        {
            int characterIndex = i; // ������ ĸ���ϱ� ������ i�� �ȳְ� ���÷� ���� ������ �Ҵ��ؼ� ���
            selectCharaceterBtns[i]?.onClick.AddListener(() => OnCharacterButtonClicked(characterIndex));
        }

        for (int i = 0; i < categoryBtns.Count; i++) // ī�װ� ��ư�� �ʱ�ȭ
        {
            Contents.ItemType itemType = (Contents.ItemType)i; // ������ Ÿ������ ��ȯ
            categoryBtns[i]?.onClick.AddListener(() => OnItemCategoryButtonClicked(itemType));
        }


        categoryBtns[(int)ItemType.Face].image.color = Color.white; // ó�� ���� �� �� ī�װ� ��ư�� ���
    }

    void RepresentCharacter()
    {
        characterType = (CharacterType)Enum.Parse(typeof(CharacterType), DataManager.Instance.PlayerData.Character); // ��ǥ ĳ���� ���� 
        characterChangeImage.sprite = characterSprites[(int)characterType];
    }

    void InitButtons()
    {
        backButton?.onClick.AddListener(() => ViewManager.ShowLast()); // ������ â 
        buyButton?.onClick.AddListener(() => OnBuyButtonClicked()); // ����â Ȱ��ȭ
        putOnButton?.onClick.AddListener(() => OnPutOnButtonClicked()); // ĳ���� Ŀ���� ���� ��ư
        representButton?.onClick.AddListener(() => DataManager.Instance.PlayerData.Character = ShopManager.Instance.CharacterType.ToString());
        representButton?.onClick.AddListener(() => DataManager.Instance.SavePlayerData());
        representButton?.onClick.AddListener(() => OnRepresentativeCharacterImageChange());

        GameObject profile = GameObject.FindWithTag("Profile");
        if (profile!=null)
        {
           ProfileSetting profileSetting = profile.GetComponent<ProfileSetting>();
            if (profileSetting != null)
            {
                putOnButton.onClick.AddListener(() => profileSetting.OnChanageProfileAction?.Invoke());
                representButton.onClick.AddListener(() => profileSetting.OnChanageProfileAction?.Invoke());
            }
        }
    }

    void UpdateShopInterface() // ���� ������Ʈ�� ���� (ų ������)
    {
        DataManager.Instance.ReloadData();
        UpdateGoldText();
        RepresentCharacter(); // ��ǥ ĳ���ͷ� �ʱ�ȭ
    }

    void UpdateGoldText() // ��� ������Ʈ ���� �ÿ� ������ ����
    {
        goldText.text = $"{DataManager.Instance.PlayerData.Gold}";
    }

    #region UI_Buttons

    void OnCharacterButtonClicked(int characterIndex) // ĳ���� ������ ���� �̺�Ʈ
    {
        if (characterChangeImage != null && characterSprites.Count > 0)
        {
            characterChangeImage.sprite = characterSprites[characterIndex]; // ū ĳ���� �̹��� ����
            ShopManager.Instance.CharacterType = (CharacterType)characterIndex; // ������ Ŀ���� �� ĳ���� ����
            ShopManager.Instance.LoadCharacterCustom();
            characterType = (CharacterType)characterIndex;
            OnChangeCharacterAction?.Invoke();
        }
    }

    void OnRepresentativeCharacterImageChange()
    {
        for (int i = 0; i < selectCharaceterBtns.Count; i++)
        {
            if ((int)characterType == i)
            {
                selectCharaceterBtns[i].image.sprite = representCharaceterSprites[i];
            }
            else
            {
                selectCharaceterBtns[i].image.sprite = defaultCharaceterSprites[i];
            }
        }
    }

    void OnItemCategoryButtonClicked(Contents.ItemType itemType) // ������ ī�װ� ����
    {
        ShopManager.Instance.CheckItemList(itemType);

        for (int i = 0; i < categoryBtns.Count; i++) // Ŭ���� ī�װ��� �����ϰ�� ��Ӱ� ó��
        {
            if ((int)itemType != i)
            {
                categoryBtns[i].image.color = Color.gray;
            }
            else
            {
                categoryBtns[i].image.color = Color.white;
            }
        }
    }

    void OnBuyButtonClicked()
    {
        PurchasePopUp popUp = ViewManager.GetView<PurchasePopUp>();

        if (ShopManager.Instance.ItemInfo == null)
        {
            popUp.SetCheckMessage("Choose the purchase item");
            popUp.checkAction(false); // �Ϸ� ��ư�� �ߵ���
        }
        ViewManager.Show<PurchasePopUp>(true, true);
    }

    void OnPutOnButtonClicked()
    {
        CharacterType characterType = ShopManager.Instance.CharacterType;
        PurchasePopUp popUp = ViewManager.GetView<PurchasePopUp>();

        if (popUp == null)
        {
#if UNITY_EDITOR
            Debug.LogError("PurchasePopUp�� ViewManager�� �����ϴ�");
#endif
            return;
        }
        if (ShopManager.Instance.ItemInfo == null)
        {
            if (ShopManager.Instance.CharacterCustom.ItemDict[ShopManager.Instance.ItemType.ToString()] == " ") // �⺻ �����϶�
            {
                popUp.SetCheckMessage("Complete a setting");
            }
            else // �ƹ��͵� ���� �ʾ��� ��
            {
                popUp.SetCheckMessage("Select an item");
            }

        }
        else
        {
            if (DataManager.Instance.PlayerData.ShoppingList.Contains(ShopManager.Instance.ItemInfo.Name) == false) // ������ �̺���
            {
                popUp.SetCheckMessage("You don't have it!");
                ShopManager.Instance.ItemInfo = null;
                popUp.checkAction(false); // �Ϸ� ��ư�� �ߵ���
                ViewManager.Show<PurchasePopUp>(true, true);
                return;
            }
            else
            {
                ShopManager.Instance.SetCharacterCustom(); // ���� �ÿ� ���� ������ �ִ� �������̶��
                popUp.SetCheckMessage("Complete a setting");
                ShopManager.Instance.ItemInfo = null;
            }
        }
        popUp.checkAction(false); // �Ϸ� ��ư�� �ߵ���
        ViewManager.Show<PurchasePopUp>(true, true);

        // ������ �Ŵ����� ĳ���� Ŀ���� �����͸� ���� ���� â�� ĳ���� Ŀ�������� �Ҵ�
        DataManager.Instance.CharacterCustomData[characterType.ToString()] = ShopManager.Instance.CharacterCustom;
        DataManager.Instance.SaveData<IDictionary<string, Contents.CharacterCustom>>
            (DataManager.Instance.CharacterCustomData, "CustomData");
        DataManager.Instance.ReloadData(); // �ٽ� ������ ������ �ҷ���
    }
    #endregion
}

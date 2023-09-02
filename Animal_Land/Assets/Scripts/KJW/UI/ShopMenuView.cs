using Contents;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenuView : View
{
    public Action OnShopClick = null; // 상점 클릭에 대한 업데이트 델리게이트
    public Action OnPurchaseAction = null; // 구매해 대한 슬롯 및 저장 델리게이트
    public Action OnChangeCharacterAction = null;
    public bool canPutOn;

    [SerializeField] private Button backButton; // 뒤로가기 버튼
    [SerializeField] private Button buyButton; // 구매 버튼
    [SerializeField] private Button putOnButton; // 장착 버튼
    [SerializeField] private Button representButton; // 대표지정 버튼
    [SerializeField] private List<Button> categoryBtns; // 아이템 카테고리 버튼들
    [SerializeField] private List<Button> selectCharaceterBtns; // 캐릭터 선택 버튼
    [SerializeField] private List<Sprite> representCharaceterSprites; // 캐릭터 선택 버튼
    [SerializeField] private List<Sprite> defaultCharaceterSprites; // 캐릭터 선택 버튼
    [SerializeField] private Image characterChangeImage; // 바뀔 캐릭터 이미지
    [SerializeField] private List<Sprite> characterSprites; // 캐릭터 스프라이트들
    [SerializeField] private Text goldText;
    [SerializeField] private Text buyText;

    CharacterType characterType;

    public override void Initialize()
    {
        InitButtons();
        RepresentCharacter();
        selectCharaceterBtns[(int)characterType].image.sprite = representCharaceterSprites[(int)characterType]; // 캐릭터 선택 이미지 대표 캐릭터로 변경
        OnShopClick += UpdateShopInterface;
        OnShopClick?.Invoke(); ; // 골드 UI Text 업데이트
        OnPurchaseAction += DataManager.Instance.ReloadData;
        OnPurchaseAction += UpdateGoldText;

        for (int i = 0; i < selectCharaceterBtns.Count; i++) // 캐릭터 선택 버튼들 초기화
        {
            int characterIndex = i; // 변수를 캡쳐하기 때문에 i를 안넣고 로컬로 따로 변수에 할당해서 사용
            selectCharaceterBtns[i]?.onClick.AddListener(() => OnCharacterButtonClicked(characterIndex));
        }

        for (int i = 0; i < categoryBtns.Count; i++) // 카테고리 버튼들 초기화
        {
            Contents.ItemType itemType = (Contents.ItemType)i; // 아이템 타입으로 변환
            categoryBtns[i]?.onClick.AddListener(() => OnItemCategoryButtonClicked(itemType));
        }

        categoryBtns[(int)ItemType.Face].image.color = Color.white; // 처음 켜질 때 얼굴 카테고리 버튼만 밝게
    }

    void RepresentCharacter()
    {
        characterType = (CharacterType)Enum.Parse(typeof(CharacterType), DataManager.Instance.PlayerData.Character); // 대표 캐릭터 설정 
        characterChangeImage.sprite = characterSprites[(int)characterType];
    }

    void InitButtons()
    {
        backButton?.onClick.AddListener(() => ViewManager.ShowLast()); // 마지막 창 
        buyButton?.onClick.AddListener(() => OnBuyButtonClicked()); // 구매창 활성화
        putOnButton?.onClick.AddListener(() => OnPutOnButtonClicked()); // 캐릭터 커스텀 저장 버튼
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

    void UpdateShopInterface() // 상점 업데이트를 위함 (킬 때마다)
    {
        DataManager.Instance.ReloadData();
        UpdateGoldText();
        RepresentCharacter(); // 대표 캐릭터로 초기화
    }

    void UpdateGoldText() // 골드 업데이트 구매 시에 변경을 위함
    {
        goldText.text = $"{DataManager.Instance.PlayerData.Gold}";
    }

    #region UI_Buttons

    void OnCharacterButtonClicked(int characterIndex) // 캐릭터 변경을 위한 이벤트
    {
        if (characterChangeImage != null && characterSprites.Count > 0)
        {
            characterChangeImage.sprite = characterSprites[characterIndex]; // 큰 캐릭터 이미지 변경
            ShopManager.Instance.CharacterType = (CharacterType)characterIndex; // 상점에 커스텀 할 캐릭터 변경
            ShopManager.Instance.LoadCharacterCustom();
            characterType = (CharacterType)characterIndex;
            OnChangeCharacterAction?.Invoke();
            ShopManager.Instance.InitSlotClicked();
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

    void OnItemCategoryButtonClicked(Contents.ItemType itemType) // 아이템 카테고리 변경
    {
        ShopManager.Instance.CheckItemList(itemType);

        for (int i = 0; i < categoryBtns.Count; i++) // 클릭된 카테고리를 제외하고는 어둡게 처리
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
            popUp.checkAction(false); // 완료 버튼만 뜨도록
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
            Debug.LogError("PurchasePopUp이 ViewManager에 없습니다");
#endif
            return;
        }
        if (ShopManager.Instance.ItemInfo == null)
        {
            if (ShopManager.Instance.CharacterCustom.ItemDict[ShopManager.Instance.ItemType.ToString()] == " ") // 기본 셋팅일때
            {
                popUp.SetCheckMessage("Complete a setting!");
            }
            else // 아무것도 고르지 않았을 때
            {
                popUp.SetCheckMessage("Select an item");
            }

        }
        else
        {
            if (DataManager.Instance.PlayerData.ShoppingList.Contains(ShopManager.Instance.ItemInfo.Name) == false) // 아이템 미보유
            {
                popUp.SetCheckMessage("You don't have it!");
                ShopManager.Instance.ItemInfo = null;
                popUp.checkAction(false); // 완료 버튼만 뜨도록
                ViewManager.Show<PurchasePopUp>(true, true);
                return;
            }
            else
            {
                ShopManager.Instance.SetCharacterCustom(); // 선택 시에 만약 가지고 있는 아이템이라면
                popUp.SetCheckMessage("Complete a setting!");
                ShopManager.Instance.ItemInfo = null;
            }
        }
        popUp.checkAction(false); // 완료 버튼만 뜨도록
        ViewManager.Show<PurchasePopUp>(true, true);

        ShopManager.Instance.CheckItemAndSave();
    }
    #endregion
}

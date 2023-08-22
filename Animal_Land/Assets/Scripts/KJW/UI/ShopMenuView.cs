using Contents;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenuView : View
{
    public Action OnShopClick; // 상점 클릭에 대한 업데이트 델리게이트
    public bool canPutOn;

    [SerializeField] private Button backButton; // 뒤로가기 버튼
    [SerializeField] private Button buyButton; // 구매 버튼
    [SerializeField] private Button putOnButton; // 장착 버튼
    [SerializeField] private List<Button> categoryBtns; // 아이템 카테고리 버튼들
    [SerializeField] private List<Button> selectCharaceterBtns; // 캐릭터 선택 버튼
    [SerializeField] private Image characterChangeImage; // 바뀔 캐릭터 이미지
    [SerializeField] private List<Sprite> characterSprites; // 캐릭터 스프라이트들
    [SerializeField] private Text goldText;
    [SerializeField] private Text buyText;

    public override void Initialize()
    {
        backButton?.onClick.AddListener(() => ViewManager.ShowLast()); // 마지막 창  
        buyButton?.onClick.AddListener(() => OnBuyButtonClicked()); // 구매창 활성화
        putOnButton?.onClick.AddListener(() => OnPutOnButtonClicked()); // 캐릭터 커스텀 저장 버튼

        OnShopClick += UpdateShopInterface;
        OnShopClick?.Invoke(); ; // 골드 UI Text 업데이트

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
    }

    void UpdateShopInterface() // 상점 업데이트를 위함
    {
        DataManager.Instance.ReloadData();
        UpdateGoldText();
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
            characterChangeImage.sprite = characterSprites[characterIndex];
            ShopManager.Instance.CharacterType = (CharacterType)characterIndex;
            ShopManager.Instance.LoadCharacterCustom();
        }
    }

    void OnItemCategoryButtonClicked(Contents.ItemType itemType) // 아이템 카테고리 변경
    {
        ShopManager.Instance.CheckItemList(itemType);
    }

    void OnBuyButtonClicked()
    {
        ViewManager.Show<PurchasePopUp>(true, true);
    }

    void OnPutOnButtonClicked()
    {
        CharacterType characterType = ShopManager.Instance.CharacterType;



        ShopManager.Instance.SetCharacterCustom(); // 선택 시에 만약 가지고 있는 아이템이라면
        PurchasePopUp popUp = ViewManager.GetView<PurchasePopUp>();
        if (popUp != null)
        {
            popUp.SetCheckMessage("장착 완료!");
            popUp.checkAction(false); // 완료 버튼만 뜨도록
            ViewManager.Show<PurchasePopUp>(true, true);
        }

        // 데이터 매니저의 캐릭터 커스텀 데이터를 현재 상점 창의 캐릭터 커스텀으로 할당
        DataManager.Instance.CharacterCustomData[characterType.ToString()] = ShopManager.Instance.CharacterCustom;
        DataManager.Instance.SaveData<IDictionary<string, Contents.CharacterCustom>>
            (DataManager.Instance.CharacterCustomData, "CustomData");
        DataManager.Instance.ReloadData(); // 다시 저장한 데이터 불러옴
    }
    #endregion
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenuView : View
{
    [SerializeField]
    private Button backButton; // 뒤로가기 버튼
    [SerializeField]
    private Button buyButton; // 구매 버튼
    [SerializeField]
    private Button saveButton; // 저장 버튼
    [SerializeField]
    private List<Button> selectCharaceterBtns; // 캐릭터 선택 버튼
    [SerializeField]
    private Image characterChangeImage; // 바뀔 캐릭터 이미지
    [SerializeField]
    private List<Sprite> characterSprites; // 캐릭터 스프라이트들
    [SerializeField]
    private Text goldText;

    public override void Initialize()
    {
        backButton.onClick.AddListener(() => ViewManager.ShowLast()); // 마지막 창  
        buyButton.onClick.AddListener(() => ViewManager.Show<PurchasePopUp>(true, true)); // 구매창 활성화
        saveButton.onClick.AddListener(() => DataManager.Instance.SaveData<IDictionary<string, Contents.CharacterCustom>>(DataManager.Instance.CharacterCustomData, "TestJson")); ;
        goldText.text = $"GOLD : {DataManager.Instance.Gold}";

        for (int i = 0; i < selectCharaceterBtns.Count; i++)
        {
            int characterIndex = i; // 변수를 캡쳐하기 때문에 i를 안넣고 로컬로 따로 변수에 할당해서 사용
            selectCharaceterBtns[i].onClick.AddListener(() => OnCharacterButtonClicked(characterIndex));
        }
    }

    void OnCharacterButtonClicked(int characterIndex)
    {
        if (characterChangeImage != null && characterSprites.Count > 0)
        {
            characterChangeImage.sprite = characterSprites[characterIndex];
        }
    }

}

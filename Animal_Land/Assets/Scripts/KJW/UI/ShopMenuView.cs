using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopMenuView : View
{
    [SerializeField]
    private Button backButton;
    [SerializeField]
    private List<Button> selectCharaceterBtn;
    [SerializeField]
    private Image characterChangeImage;
    [SerializeField]
    private List<Sprite> characterSprites;

    public override void Initialize()
    {
        backButton.onClick.AddListener(() => ViewManager.ShowList()); // 마지막 창  

        for (int i = 0; i < selectCharaceterBtn.Count; i++)
        {
            int characterIndex = i; // 변수를 캡쳐하기 때문에 i를 안넣고 로컬로 따로 변수에 할당해서 사용
            selectCharaceterBtn[i].onClick.AddListener(() => OnCharacterButtonClicked(characterIndex)); 
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

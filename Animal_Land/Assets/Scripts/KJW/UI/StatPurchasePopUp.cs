using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatPurchasePopUp :PopUpUI, IStatusCheckPopUP
{
    [SerializeField] private Text noticeText;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private Button okayButton;


    bool clickedPurchase = false;
    

    void Start()
    {
        GameStartView gameStartView = ViewManager.GetView<GameStartView>();
        gameStartView.ClickPurcahseAction +=(()=> clickedPurchase);
        purchaseButton?.onClick.AddListener(() => OnPurchaseButton());
        okayButton?.onClick.AddListener(() => ViewManager.ShowLast());
    }

    public void SetCheckMessage(string message)
    {
        noticeText.text = message;
    }

    public void OnPurchaseButton()
    {
        ViewManager.ShowLast(); // 구매 확인 창 끄기
        clickedPurchase = true; // 구매 버튼 눌렀음
    }

    protected override void OnDisable()
    {
        base.OnDisable(); 
        clickedPurchase = false; // 구매 창이 종료 됐으니 다시 false로 설정
    }

    public void OnOkayButton(bool flag)
    {
        okayButton?.gameObject.SetActive(flag);
        purchaseButton?.gameObject.SetActive(!flag);
        closeButton?.gameObject.SetActive(!flag);
    }

}

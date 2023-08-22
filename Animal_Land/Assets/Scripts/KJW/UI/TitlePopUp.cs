using UnityEngine;
using UnityEngine.UI;

public class TitlePopUp : PopUpUI, IStatusCheckPopUP
{

    public override void Initialize()
    {
        closeButton.onClick.AddListener(() => this.gameObject.SetActive(false)); // 마지막 창  
        if (blocker != null)
        {
            closeButton.onClick.AddListener(() => blocker?.gameObject.SetActive(false)); // OnDisable만 있을 때 테스트 해보기
        }
    }
    [SerializeField] private Text popUPText;
    public void SetCheckMessage(string message)
    {
        popUPText.text = message;
    }
}

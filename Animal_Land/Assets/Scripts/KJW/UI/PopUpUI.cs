using UnityEngine;
using UnityEngine.UI;

public class PopUpUI : View
{
    [SerializeField]
    protected Button closeButton;
    [SerializeField]
    protected Image blocker; // 뒤에 화면이 터치가 안되도록 이미지 활성화

    public override void Initialize()
    {
        closeButton.onClick.AddListener(() => ViewManager.ShowLast()); // 마지막 창  
        closeButton.onClick.AddListener(() => blocker?.gameObject.SetActive(false));
    }

    private void OnEnable()
    {
        if (blocker != null && !blocker.gameObject.activeSelf) // 활성화가 안되어 있을 때
        {
            blocker.gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        if (blocker != null)
        {

            blocker?.gameObject.SetActive(false);
        }
    }
}

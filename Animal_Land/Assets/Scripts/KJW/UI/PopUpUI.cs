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
        closeButton.onClick.AddListener(() => blocker?.gameObject.SetActive(false)); // OnDisable만 있을 때 테스트 해보기
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
        if (blocker != null) // 이중으로 되어있을 경우를 위해서 추가
        {
            blocker?.gameObject.SetActive(false);
        }
    }
}

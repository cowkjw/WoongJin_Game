using UnityEngine;
using UnityEngine.UI;

public class LobbyMenuView : View
{
    [SerializeField]
    private Button shopButton;
    [SerializeField]
    ShopMenuView shopMenuView;

    public override void Initialize()
    {
        shopButton.onClick.AddListener(() => ViewManager.Show<ShopMenuView>());
        shopButton.onClick.AddListener(() => shopMenuView?.OnShopClick());
    }
}

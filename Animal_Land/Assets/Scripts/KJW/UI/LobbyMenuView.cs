using UnityEngine;
using UnityEngine.UI;

public class LobbyMenuView : View
{
    [SerializeField]
    private Button shopButton;

    public override void Initialize()
    {
        shopButton.onClick.AddListener(() => ViewManager.Show<ShopMenuView>());
        //if(DataManager.Instance!=null)
        //{
        //    shopButton.onClick.AddListener(() => DataManager.Instance.ReloadData());
        //}
    }
}

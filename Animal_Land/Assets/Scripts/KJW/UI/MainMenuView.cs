using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : View
{
    [SerializeField]
    private Button shopButton;

    public override void Initialize()
    {
        shopButton.onClick.AddListener(() => ViewManager.Show<ShopMenuView>());
    }
}

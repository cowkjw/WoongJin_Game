using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseCheckPopUp : PopUpUI , IStatusCheckPopUP
{
    public override void Initialize()
    {
        base.Initialize();
        closeButton.onClick.AddListener(() => ViewManager.ShowLast());
    }
}

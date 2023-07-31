using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseCheckPopUp : PopUpUI, IStatusCheckPopUP 
{
    [SerializeField] Text checkMessage;

    public override void Initialize()
    {
        base.Initialize();
        closeButton?.onClick.AddListener(() => ViewManager.ShowLast());
    }

    public void SetCheckMessage(string message)
    {
       checkMessage.text = message;
    }
}

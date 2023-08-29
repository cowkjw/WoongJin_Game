using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum CheckType
{
    Purchase,
    PutOn
}
public class PurchaseCheckPopUp : PopUpUI, IStatusCheckPopUP 
{
    [SerializeField] Text checkMessage;

    public override void Initialize()
    {
        base.Initialize();
        closeButton?.onClick.AddListener(() => ViewManager.ShowLast());
        closeButton?.onClick.AddListener(() => ShopManager.Instance.InitSlotClicked());
    }



    public void SetCheckMessage(string message)
    {
        checkMessage.text = message;
    }
}

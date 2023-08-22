
using UnityEngine;
using UnityEngine.UI;

public class TitlePopUp : PopUpUI,IStatusCheckPopUP
{
    [SerializeField] private Text popUPText;
    public void SetCheckMessage(string message)
    {
        popUPText.text = message;
    }
}

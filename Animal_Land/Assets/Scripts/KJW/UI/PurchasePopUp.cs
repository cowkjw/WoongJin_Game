using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchasePopUp : PopUpUI
{
    [SerializeField]
    private List<Button> selectBtns;

    public override void Initialize()
    {
        base.Initialize();

        for (int i = 0; i < selectBtns.Count; i++)
        {
            int index = i; // 변수를 캡쳐하기 때문에 i를 안넣고 로컬로 따로 변수에 할당해서 사용
            selectBtns[i].onClick.AddListener(() => OnSelectPurchase(index));
        }
    }

    void OnSelectPurchase(int index)
    {
        switch (index)
        {
            case 0: // 구매
                // TODO : 골드 확인하기 

                PurchaseCheckPopUp popUpUI = ViewManager.GetView<PurchaseCheckPopUp>();
                if (popUpUI != null && popUpUI.name == "Selection_Result_PopUp") // 팝업 창 이름이 동일한지
                {
                    ViewManager.Show<PurchaseCheckPopUp>(true, true); // 구매 확인 창
                }
                break;
            case 1: // 취소
                ViewManager.ShowLast(); // 이전 화면으로 
                blocker.gameObject?.SetActive(false); // 블로커 비활성화
                break;
        }
    }


}

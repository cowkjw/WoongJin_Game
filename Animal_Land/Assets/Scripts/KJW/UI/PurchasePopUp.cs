using Contents;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchasePopUp : PopUpUI, IStatusCheckPopUP
{
    public Action<bool> checkAction; // �ܺο��� ��� �� ��������Ʈ
    public ItemInfo ClickSlotItemInfo { get; set; } // ���� ���� ���� ������ ����

    [SerializeField] private List<Button> selectBtns;
    [SerializeField] private Text checkMessage;
    [SerializeField] List<Sprite> checkSprites;
    Image image;
    public override void Initialize()
    {
        base.Initialize();

        SetCheckMessage("Choose the purchase item"); // �⺻ �޽���
        checkAction += ActivateButtons; // ��������Ʈ�� ��ư Ȱ��ȭ/��Ȱ��ȭ �Լ� �߰�
        for (int i = 0; i < selectBtns.Count; i++)
        {
            int index = i; // ������ ĸ���ϱ� ������ i�� �ȳְ� ���÷� ���� ������ �Ҵ��ؼ� ���
            selectBtns[i]?.onClick.AddListener(() => OnSelectPurchase(index));
            selectBtns[i]?.onClick.AddListener(() => ShopManager.Instance.InitSlotClicked());
            selectBtns[i]?.gameObject.SetActive(false);
        }
        closeButton?.onClick.AddListener(() => ShopManager.Instance.InitSlotClicked());
    }

    void ActivateButtons(bool flag) // ��ư�� Ȱ��ȭ
    {
        closeButton?.gameObject.SetActive(!flag); // �ݴ�� �����ؾ��� ���� �Ⱥ���

        foreach (var button in selectBtns) // ��, �ƴϿ� ��ư Ȱ��ȭ
        {
            button.gameObject.SetActive(flag);
        }
    }

    void OnSelectPurchase(int index) // ���� ��ư 
    {
        switch (index)
        {
            case 0: // ����
                PurchaseCheckPopUp popUpUI = ViewManager.GetView<PurchaseCheckPopUp>();
                if (popUpUI != null && popUpUI.name == "Selection_Result_PopUp") // �˾� â �̸��� ��������
                {
                    if (DataManager.Instance.PlayerData != null)
                    {
                        if(ShopManager.Instance.ItemInfo != null) // ������ �� ������
                        {
                            string message = CheckPlayerGold(ShopManager.Instance.ItemInfo.Price);
                            popUpUI.SetCheckMessage(message);
                        }
                    }
                    ViewManager.Show<PurchaseCheckPopUp>(true, true); // ���� Ȯ�� â
                }
                break;
            case 1: // ���
                ViewManager.ShowLast(); // ���� ȭ������ 
                blocker.gameObject?.SetActive(false); // ���Ŀ ��Ȱ��ȭ
                break;
        }
    }


    string CheckPlayerGold(int price)
    {
        int playerGold = DataManager.Instance.PlayerData.Gold;

        if (playerGold >= price) // �� �� ������
        {
            DataManager.Instance.PlayerData.Gold -= price; // ��� ����
            DataManager.Instance.PlayerData.ShoppingList.Add(ShopManager.Instance.ItemInfo.Name); // ���� ����Ʈ�� �߰�
            DataManager.Instance.SavePlayerData(); // �÷��̾� ������ ����
            ViewManager.GetView<ShopMenuView>().OnPurchaseAction?.Invoke(); // ���� UI ������Ʈ

            return "Complete purchase!";
        }
        else
        {
            int difference = Math.Abs(playerGold - price); // ������ �ݾ�
            return $"You are {difference} Gold short"; 
        }
    }

    public void SetCheckMessage(string message)
    {
        checkMessage.text = message;
    }
}

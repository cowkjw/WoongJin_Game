using Contents;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStartView : View
{
    public Func<bool> ClickPurcahseAction = null;
    [SerializeField] private List<Button> statButtons;
    [SerializeField] private Button gameStartButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI goldText;

    private PlayerStat _playerStat = new PlayerStat();
    private int _gold = 0; // �����ص״� â�� ������ ��� �����ֱ� ����

    public override void Initialize()
    {
        closeButton.onClick.AddListener(() =>
        {
            OnCloseGameStartPanel();
            ViewManager.ShowLast();
        });
        gameStartButton.onClick.AddListener(() => OnGameStartButton());
        for (int i = 0; i < statButtons.Count; i++)
        {
            int index = i;
            statButtons[index].onClick.AddListener(() => OnStatPurchaseButton(index));
        }

    }

    private void OnEnable()
    {
       _playerStat = new PlayerStat(); // ���� �� ���� ������ �ʱ�ȭ �ϱ� ���ؼ�
        UpdateGoldText();
    }

    public void UpdateGoldText()
    {
        goldText.text = (DataManager.Instance.PlayerData.Gold - _gold).ToString();
    }


    #region GAME_PANEL_BUTTONS

    private void OnCloseGameStartPanel()
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.PlayerStat = new PlayerStat(); // ���� �� ���ӿ� ���� ��� �� ������ �����͸Ŵ����� �Ѱ���
        }
        _gold = 0; // �ٽ� 0���� �ʱ�ȭ
    }

    IEnumerator WaitForClosePopUP(StatPurchasePopUp popUP, int index, StatType statType) // ���� �˾� ������� ��ٸ��� ���� �ڷ�ƾ
    {

        popUP?.SetCheckMessage($"Want to buy {statType.ToString()}?");

        while (popUP.gameObject.activeSelf) // �˾�â�� ����� ������
        {
            yield return null;
        }

        if (ClickPurcahseAction?.Invoke() == true) // ����â�� ���ȴٸ�
        {
            switch (index)
            {
                case 0: // Speed
                    _playerStat.Speed++;
                    _gold += 30;
                    break;
                case 1: // HP
                    _playerStat.HP++;
                    _gold += 20;
                    break;
                case 2: //Shield
                    _playerStat.Energy++;
                    _gold += 20;
                    break;
            }
            UpdateGoldText();
        }
#if UNITY_EDITOR
        Debug.Log($"Speed: {_playerStat.Speed} HP: {_playerStat.HP} Shield {_playerStat.Energy}");
#endif 
    }

    void OnGameStartButton() // ���� ���� ��ư
    {
        if (DataManager.Instance != null)
        {
            if (DataManager.Instance.PlayerStat == null)
            {
#if UNITY_EDITOR
                Debug.LogError("������ �Ŵ����� �÷��̾� ������ NULL�Դϴ�.");
#endif
                return;
            }
            DataManager.Instance.PlayerStat = _playerStat; // ���� �� ���ӿ� ���� ��� �� ������ �����͸Ŵ����� �Ѱ���
            DataManager.Instance.PlayerData.Gold -= _gold;
            DataManager.Instance.SavePlayerData();
        }
        SceneManager.LoadScene(SStageInfo.StageName); // �̱� ������ �̵�
    }

    void OnStatPurchaseButton(int index)
    {
        StatPurchasePopUp popUP = ViewManager.GetView<StatPurchasePopUp>();
        if (popUP == null)
        {
#if UNITY_EDITOR
            Debug.LogError("���� �˾� â�� view�Ŵ����� �������� �ʽ��ϴ�");
#endif
            return;
        }

        //30 20 20
        int needGold = 0;
        switch (index)
        {
            case 0: // Speed
                needGold = 30;
                break;
            default:
                needGold = 20;
                break;
        }

        if (DataManager.Instance.PlayerData.Gold < needGold)
        {
            popUP.SetCheckMessage("Not enough Gold");
            ViewManager.GetView<StatPurchasePopUp>()?.OnOkayButton(true);
            ViewManager.Show<StatPurchasePopUp>(true, true); // ����â �˾�
            return;
        }

        StatType statType = StatType.SPEED;
        switch (index)
        {
            case 0:
                statType = StatType.SPEED;
                break;
            case 1:
                statType = StatType.HP;
                break;
            case 2:
                statType = StatType.ENERGY;
                break;
        }

        ViewManager.Show<StatPurchasePopUp>(true, true); // ����â �˾�
            ViewManager.GetView<StatPurchasePopUp>()?.OnOkayButton(false);
        if (_playerStat.CheckForPurchase(index))
        {
            StartCoroutine(WaitForClosePopUP(popUP, index, statType));
        }
        else
        {
            ViewManager.GetView<StatPurchasePopUp>()?.OnOkayButton(true);
            popUP.SetCheckMessage("You can't buy more.");
        }
    }
    #endregion
}

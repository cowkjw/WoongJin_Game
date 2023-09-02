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
    private int _gold = 0; // 저장해뒀다 창을 닫으면 골드 돌려주기 위함

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
       _playerStat = new PlayerStat(); // 켜질 때 새로 스텟을 초기화 하기 위해서
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
            DataManager.Instance.PlayerStat = new PlayerStat(); // 꺼질 때 게임에 들어가서 사용 할 스탯을 데이터매니저에 넘겨줌
        }
        _gold = 0; // 다시 0으로 초기화
    }

    IEnumerator WaitForClosePopUP(StatPurchasePopUp popUP, int index, StatType statType) // 구매 팝업 종료까지 기다리기 위한 코루틴
    {

        popUP?.SetCheckMessage($"Want to buy {statType.ToString()}?");

        while (popUP.gameObject.activeSelf) // 팝업창이 종료될 때까지
        {
            yield return null;
        }

        if (ClickPurcahseAction?.Invoke() == true) // 구매창이 눌렸다면
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

    void OnGameStartButton() // 게임 시작 버튼
    {
        if (DataManager.Instance != null)
        {
            if (DataManager.Instance.PlayerStat == null)
            {
#if UNITY_EDITOR
                Debug.LogError("데이터 매니저의 플레이어 스텟이 NULL입니다.");
#endif
                return;
            }
            DataManager.Instance.PlayerStat = _playerStat; // 꺼질 때 게임에 들어가서 사용 할 스탯을 데이터매니저에 넘겨줌
            DataManager.Instance.PlayerData.Gold -= _gold;
            DataManager.Instance.SavePlayerData();
        }
        SceneManager.LoadScene(SStageInfo.StageName); // 싱글 룸으로 이동
    }

    void OnStatPurchaseButton(int index)
    {
        StatPurchasePopUp popUP = ViewManager.GetView<StatPurchasePopUp>();
        if (popUP == null)
        {
#if UNITY_EDITOR
            Debug.LogError("구매 팝업 창이 view매니저에 존재하지 않습니다");
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
            ViewManager.Show<StatPurchasePopUp>(true, true); // 구매창 팝업
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

        ViewManager.Show<StatPurchasePopUp>(true, true); // 구매창 팝업
            ViewManager.GetView<StatPurchasePopUp>()?.OnOkayButton(false);
        if (_playerStat.CheckForPurchase(index))
        {
            StartCoroutine(WaitForClosePopUP(popUP, index, statType));
        }
        else
        {
            ViewManager.GetView<StatPurchasePopUp>()?.OnOkayButton(true);
            popUP.SetCheckMessage("You can't buy more");
        }
    }
    #endregion
}

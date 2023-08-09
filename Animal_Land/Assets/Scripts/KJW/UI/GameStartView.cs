using Contents;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStartView : View
{
    [SerializeField] private List<Button> statButtons;
    [SerializeField] private Button gameStartButton;
    [SerializeField] private Button closeButton;

    private PlayerStat _playerStat = new PlayerStat();
    private int _gold = 0; // 저장해뒀다 창을 닫으면 골드 돌려주기 위함

    public override void Initialize()
    {
        closeButton.onClick.AddListener(() => OnCloseGameStartPanel());
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
    }

    #region GAME_PANEL_BUTTONS

    private void OnCloseGameStartPanel()
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.PlayerStat = new PlayerStat(); // 꺼질 때 게임에 들어가서 사용 할 스탯을 데이터매니저에 넘겨줌
        }
        DataManager.Instance.PlayerData.Gold += _gold; // 골드 되돌려놓기
        _gold = 0; // 다시 0으로 초기화
    }

    void OnGameStartButton()
    {
        if (DataManager.Instance.PlayerStat == null)
        {
#if UNITY_EDITOR
            Debug.LogError("데이터 매니저의 플레이어 스텟이 NULL입니다.");
#endif
            return;
        }
        DataManager.Instance.PlayerStat = _playerStat; // 꺼질 때 게임에 들어가서 사용 할 스탯을 데이터매니저에 넘겨줌
    }

    void OnStatPurchaseButton(int index)
    {
        switch (index)
        {
            case 0:
                if (_playerStat.CheckForPurchase(index))
                {
                    _playerStat.Speed++;
                }
                break;
            case 1:
                if (_playerStat.CheckForPurchase(index))
                {
                    _playerStat.HP++;
                }
                break;
            case 2:
                if (_playerStat.CheckForPurchase(index))
                {
                    _playerStat.Shield++;
                }
                break;
        }

        Debug.Log($"Speed: {_playerStat.Speed} HP: {_playerStat.HP} Shield {_playerStat.Shield}");
    }
    #endregion
}

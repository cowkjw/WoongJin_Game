using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldButton : MonoBehaviour
{
    ShopMenuView _shopMenuView;

    private void Start()
    {
        _shopMenuView = ViewManager.GetView<ShopMenuView>();
    }
    public void ShowMeTheMoney()
    {
        if(DataManager.Instance!=null)
        {
            DataManager.Instance.PlayerData.Gold = DataManager.Instance.PlayerData.Gold+100;
            Debug.Log(DataManager.Instance.PlayerData.Gold);
            DataManager.Instance.SavePlayerData();
        }
    }

    public void StealingGold()
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.PlayerData.Gold = DataManager.Instance.PlayerData.Gold - 100;
            DataManager.Instance.PlayerData.Gold = Mathf.Max(DataManager.Instance.PlayerData.Gold, 0);
            Debug.Log(DataManager.Instance.PlayerData.Gold);
            DataManager.Instance.SavePlayerData();
        }
    }
}

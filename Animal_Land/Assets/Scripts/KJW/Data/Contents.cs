using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Contents
{
    public enum ItemType
    {
        Face,
        Hat,
        Necklace,
        Glass,
        Wing
    }

    public enum CharacterType
    {
        Frog,
        Bird,
        Dog,
        Cat
    }

   public enum StatType
    {
        SPEED,
        HP,
        SHIELD
    }

    public class CharacterCustom
    {
        [JsonProperty("CustomList")]
        public IDictionary<string, string> ItemDict { get; set; } = new Dictionary<string, string>(); // Key : 아이템 타입, Vlaue : 아이템 이름
    }

    public class PlayerData
    {
        [JsonProperty("Gold")]
        public int Gold { get; set; }
        [JsonProperty("ShoppingList")]
        public IDictionary<string, bool> ShoppingList { get; set; } = new Dictionary<string, bool>();
    }

    public class ItemInfo
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Price")]
        public int Price { get; set; }
    }

    public class PlayerStat
    {
        private const int MAX_SPEED_HP = 10;
        private const int MAX_SHIELD = 3;


        private int _hp = 0;
        private int _shield = 0; // Key는 쉴드 횟수, Value는 현재 체력
        private int _speed = 0;

        public int HP
        {
            get => _hp;
            set => _hp = Math.Clamp(value, 0, MAX_SPEED_HP);
        }

        public int Shield
        {
            get => _shield;
            set => _shield = Math.Clamp(value, 0, MAX_SHIELD);
        }

        public int Speed
        {
            get => _speed;
            set => _speed = Math.Clamp(value, 0, MAX_SPEED_HP);
        }

        public bool CheckForPurchase(int index)
        {
            switch (index)
            {
                case 0:
                    if (_speed >= MAX_SPEED_HP)
                    {
                       
#if UNITY_EDITOR
                        Debug.LogWarning("더 이상 SPEED를 구매할 수 없습니다.");
#endif  
                        return false;
                    }
                    break;

                case 1:
                    if (_hp >= MAX_SPEED_HP)
                    {
                       
#if UNITY_EDITOR
                        Debug.LogWarning("더 이상 HP를 구매할 수 없습니다.");
#endif
                        return false;
                    }
                        break;
                case 2:
                    if (_shield >= MAX_SHIELD)
                    {
                        
#if UNITY_EDITOR
                        Debug.LogWarning("더 이상 SHIELD를 구매할 수 없습니다.");
#endif
                        return false;
                    }
                    break;
            }
            return true;
        }
    }

}

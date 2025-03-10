using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Contents
{
    public enum ItemType
    {
        Face,
        Glasses,
        Hat,
        Etc
    }

    public enum CharacterType
    {
        Bird,
        Cat,
        Dog,
        Frog,
        None
    }

    public enum StatType
    {
        SPEED,
        HP,
        ENERGY
    }

    public class Rank
    {
        public Rank() { }
        public Rank(string name, int score)
        {
            this.Name = name;
            this.Score = score;
        }

        public string Name; // 닉네임 설정
        public int Score; // 점수
    }

    public class CharacterCustom
    {
        public CharacterCustom(CharacterCustom characterCustom)
        {
            this.ItemDict = new Dictionary<string, string>(characterCustom.ItemDict);
        }
        public CharacterCustom() { }
        [JsonProperty("CustomList")]
        public IDictionary<string, string> ItemDict { get; set; } = new Dictionary<string, string>(); // Key : 아이템 타입, Vlaue : 아이템 이름
    }

    public class PlayerData
    {
        [JsonProperty("Character")]
        public string Character { get; set; } = "Bird";
        [JsonProperty("Gold")]
        public int Gold { get; set; } = 0;
        [JsonProperty("ShoppingList")]
        public HashSet<string> ShoppingList { get; set; } = new HashSet<string>();
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
        private const int MAX_PURCHASE = 1;

        private int _hp = 0;
        private int _energy = 0; // Key는 쉴드 횟수, Value는 현재 체력
        private int _speed = 0;

        public int HP
        {
            get => _hp;
            set => _hp = value;
        }

        public int Energy
        {
            get => _energy;
            set => _energy = value;
        }

        public int Speed
        {
            get => _speed;
            set => _speed = value;
        }

        public bool CheckForPurchase(int index)
        {
            switch (index)
            {
                case 0:
                    if (_speed >= MAX_PURCHASE)
                    {

#if UNITY_EDITOR
                        Debug.LogWarning("더 이상 SPEED를 구매할 수 없습니다.");
#endif  
                        return false;
                    }
                    break;

                case 1:
                    if (_hp >= MAX_PURCHASE)
                    {

#if UNITY_EDITOR
                        Debug.LogWarning("더 이상 HP를 구매할 수 없습니다.");
#endif
                        return false;
                    }
                    break;
                case 2:
                    if (_energy >= MAX_PURCHASE)
                    {

#if UNITY_EDITOR
                        Debug.LogWarning("더 이상 ENERGY를 구매할 수 없습니다.");
#endif
                        return false;
                    }
                    break;
            }
            return true;
        }
    }

}
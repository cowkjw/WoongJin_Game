using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Contents
{
    //[Serializable]
    public class CharacterCustom
    {
        [JsonProperty("Hat")]
        string hat;
        [JsonProperty("Glass")]
        string glass;
        [JsonProperty("Necklace")]
        string necklace;
    }

    public class PlayerData
    {
        [JsonProperty("Gold")]
        public int Gold { get; set; }
        [JsonProperty("ShoppingList")]
        public IDictionary<string,bool> ShoppingList { get; set; } = new Dictionary<string,bool>();
    }

    public class ItemInfo
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Price")]
        public int Price { get; set; }
    }

}

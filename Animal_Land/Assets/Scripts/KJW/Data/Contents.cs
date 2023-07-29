using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Contents
{
    [Serializable]
    public class CharacterCustom
    {
        private string head;
        private string glass;
        private string necklace;
    }

    public class PlayerData
    {
        [JsonProperty("Player Gold")]
        int gold;
    }
}

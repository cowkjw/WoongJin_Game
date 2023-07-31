using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Contents;

public class DataManager : MonoBehaviour
{

    private static DataManager instance;
    public static DataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DataManager();
            }
            return instance;
        }
    }

    private PlayerData _playerData = new PlayerData();
    private IDictionary<string, CharacterCustom> _characterCustomData = new Dictionary<string, Contents.CharacterCustom>();
    private IList<ItemInfo> _hatItemInfoList = new List<ItemInfo>();

    public IDictionary<string, CharacterCustom> CharacterCustomData => _characterCustomData;
    public PlayerData PlayerData => _playerData;
    public IList<ItemInfo> HatItemInfoList => _hatItemInfoList;


    private void Awake()
    {
        instance = this;
        //_playerData.Gold = 30000;
        //SaveData<PlayerData>(PlayerData, "PlayerData");
        //ItemInfo temp = new ItemInfo();
        //ItemInfo temp2 = new ItemInfo();
        //ItemInfo temp3 = new ItemInfo();
        //ItemInfo temp4 = new ItemInfo();
        //temp.Name = "Hat1";
        //temp2.Name = "Hat2";
        //temp3.Name = "Hat3";
        //temp4.Name = "Hat4";
        //temp.Price = 100000;
        //temp2.Price = 500;
        //temp3.Price = 300000;
        //temp4.Price = 10000;

        //_hatItemInfoList.Add(temp);
        //_hatItemInfoList.Add(temp2);
        //_hatItemInfoList.Add(temp3);
        //_hatItemInfoList.Add(temp4);
        //SaveData<IList<ItemInfo>>(_hatItemInfoList, "Item_Hat");
        _hatItemInfoList = LoadData<IList<ItemInfo>>("Item_Hat", _hatItemInfoList);
        _characterCustomData = LoadData<IDictionary<string, CharacterCustom>>("CustomData", _characterCustomData);
        _playerData = LoadData<PlayerData>("PlayerData", _playerData);
    }

    private string GetSavePath(string fileName)
    {
        string filePath;
#if UNITY_EDITOR
        // For Unity Editor
        filePath = Path.Combine(Application.dataPath, $"StreamingAssets/Data/{fileName}.json");
        //string filePath = Path.Combine(Application.streamingAssetsPath, $"StreamingAssets/Data/{fileName}.json");
#elif UNITY_ANDROID
        filePath = Path.Combine(Application.persistentDataPath, $"Data/{fileName}.json");
#endif
        if (!File.Exists(filePath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }

        return filePath;
    }

    public void SaveData<T>(T data, string fileName)
    {
        string jsonData = JsonConvert.SerializeObject(data);
        string savePath = GetSavePath(fileName);
        File.WriteAllText(savePath, jsonData);

#if UNITY_EDITOR
        Debug.Log($"{data} 저장 완료");
#endif
    }

    public T LoadData<T>(string fileName, T defaultData)
    {
        string savePath = GetSavePath(fileName);
        if (File.Exists(savePath))
        {
            string jsonData = File.ReadAllText(savePath);
            T loadedData = JsonConvert.DeserializeObject<T>(jsonData);
#if UNITY_EDITOR
            Debug.Log("데이터 불러오기 완료");
#endif
            return loadedData;
        }
        else
        {
            return defaultData;
        }
    }

}

using Contents;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.UI.CanvasScaler;

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
    private IDictionary<string, CharacterCustom> _characterCustomData = new Dictionary<string, CharacterCustom>();
    private IDictionary<string, IList<ItemInfo>> _propsItemDict = new Dictionary<string, IList<ItemInfo>>();



    public IDictionary<string, CharacterCustom> CharacterCustomData => _characterCustomData;
    public PlayerData PlayerData => _playerData;    
    public IDictionary<string,IList<ItemInfo>> PropsItemDict => _propsItemDict;
    public Contents.PlayerStat PlayerStat = new Contents.PlayerStat(); // 게임 내에 쓰일 플레이어 스탯

    private void Awake()
    {

        Init();
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

        _characterCustomData = LoadData<IDictionary<string, CharacterCustom>>("CustomData", _characterCustomData);
        _propsItemDict = LoadData<IDictionary<string, IList<ItemInfo>>>("ItemData", _propsItemDict);
        _playerData = LoadData<PlayerData>("PlayerData", _playerData);
    }

    void Init()
    {
        instance = this;
        DontDestroyOnLoad(instance);
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

    public void ReloadData()
    {
        _characterCustomData = LoadData<IDictionary<string, CharacterCustom>>("CustomData", _characterCustomData);
        _propsItemDict = LoadData<IDictionary<string, IList<ItemInfo>>>("ItemData", _propsItemDict);
        _playerData = LoadData<PlayerData>("PlayerData", _playerData);
    }
}

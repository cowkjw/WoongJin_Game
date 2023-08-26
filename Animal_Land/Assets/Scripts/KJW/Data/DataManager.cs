using Contents;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
    private JSONFileDownloader downloader;

    public JSONFileDownloader Downloader=> downloader;
    public IDictionary<string, CharacterCustom> CharacterCustomData => _characterCustomData;
    public PlayerData PlayerData => _playerData;
    public IDictionary<string, IList<ItemInfo>> PropsItemDict => _propsItemDict;
    public Contents.PlayerStat PlayerStat = new Contents.PlayerStat(); // 게임 내에 쓰일 플레이어 스탯
    private void Awake()
    {
        Init();
        ReloadData();
    }

    void Init()
    {
        downloader = GetComponent<JSONFileDownloader>();
        instance = this;
        DontDestroyOnLoad(instance);
    }

    public string GetSavePath(string fileName)
    {
        string filePath;
#if UNITY_ANDROID
        filePath = Path.Combine(Application.persistentDataPath, $"Data/{fileName}.json");
#elif UNITY_EDITOR
        filePath = Path.Combine(Application.dataPath, $"StreamingAssets/Data/{fileName}.json");
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

    void SaveAllData()
    {
        SaveData<IDictionary<string, CharacterCustom>>(_characterCustomData,"CustomData");
        SaveData<IDictionary<string, IList<ItemInfo>>>(_propsItemDict, "ItemData");
        SaveData<PlayerData>(_playerData, "PlayerData");
    }

    public void ReloadData()
    {
        _characterCustomData = LoadData<IDictionary<string, CharacterCustom>>("CustomData", _characterCustomData);
        _propsItemDict = LoadData<IDictionary<string, IList<ItemInfo>>>("ItemData", _propsItemDict);
        _playerData = LoadData<PlayerData>("PlayerData", _playerData);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

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

    public IDictionary<string, Contents.CharacterCustom> CharacterCustomData { get => characterCustomData; }
    public int Gold { get => _gold; }

    int _gold;
    IDictionary<string, Contents.CharacterCustom> characterCustomData = new Dictionary<string, Contents.CharacterCustom>();


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _gold = LoadData<int>("PlayerData", _gold);
        characterCustomData = LoadData<IDictionary<string, Contents.CharacterCustom>>("TestJson", characterCustomData);
    }

    private string GetSavePath(string fileName)
    {
        string filePath = Path.Combine(Application.dataPath, $"StreamingAssets/Data/{fileName}.json");

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
        Debug.Log("캐릭터 커스텀 저장 완료");
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

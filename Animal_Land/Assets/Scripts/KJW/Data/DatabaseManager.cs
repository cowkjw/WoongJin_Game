using Firebase;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

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

public enum DataType // 데이터 타입 enum
{
    Users,
    ItemData,
    CustomData
}

public class DatabaseManager : MonoBehaviour
{

    public string USER_NAME { get; private set; } = string.Empty;
    public static DatabaseManager Instance => instance;
    public Dictionary<string,int> RankingList = new Dictionary<string, int>(); // 유저 랭킹 리스트 딕셔너리
    public DatabaseReference reference { get; set; }
    [SerializeField] private string DB_URL = "https://animalland-d2718-default-rtdb.firebaseio.com"; // DB URL

    private static DatabaseManager instance = null;

    void Start()
    {
        instance = this;
        DontDestroyOnLoad(instance);
        InitDBM();
    }

    async Task InitDBM()
    {
        await FirebaseApp.CheckAndFixDependenciesAsync(); // 파베에 필요한 모든 종속 항목과 전제 조건 확인
        FirebaseApp app = FirebaseApp.DefaultInstance;
        app.Options.DatabaseUrl = new Uri(DB_URL); // URL 설정
        reference = FirebaseDatabase.DefaultInstance.RootReference; // 루트로 설정

        await ReadDB(DataType.Users); // 불러올 때까지 기다림
        await WriteDB();
    }


    public async Task WriteDB(int score = 0)
    {
        string uId = this.GetComponent<WJ_Connector>().UserID; // 웅진 API 아이디

        reference = FirebaseDatabase.DefaultInstance.RootReference; // DB 루트로 초기화

        if (USER_NAME == string.Empty)
        {
            USER_NAME = "Player" + uId.Substring(16, 4);
        }

        int saveTotalScore = score;

        if (RankingList.ContainsKey(USER_NAME)) // 이미 DB에 있는지 판단
        {
            saveTotalScore = RankingList[USER_NAME] + saveTotalScore;
        }

        Rank rankDate = new Rank(USER_NAME, saveTotalScore);
        string jsonData = JsonUtility.ToJson(rankDate);
        await reference.Child("Users").Child(uId).SetRawJsonValueAsync(jsonData);

        await ReadDB(DataType.Users); // 데이터 다시 읽음
    }

    public async Task ReadDB(DataType root) // DB 읽어오는 메소드
    { 
        reference = FirebaseDatabase.DefaultInstance.GetReference(root.ToString());

        try
        {
            switch(root)
            {
                case DataType.Users: // 유저 정보
                   await ReadUsersData();
                    break;
                case DataType.ItemData: // 상점 아이템 정보
                    await ReadItemData();
                    break;
                case DataType.CustomData: // 만약 처음 실행이라면 커스텀 목록 json 파일 형식 받아와서 저장
                    await ReadCustomData();
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"데이터 베이스를 읽는데 실패했습니다.: {e.Message}");
        }
    }

    async Task ReadUsersData()
    {
        try
        {
            DataSnapshot snapshot = await reference.GetValueAsync();

            foreach (DataSnapshot data in snapshot.Children)
            {
                Rank userInfo = JsonUtility.FromJson<Rank>(data.GetRawJsonValue());
                if (RankingList.ContainsKey(userInfo.Name))
                {
                    RankingList[userInfo.Name] = userInfo.Score;
                }
                else
                {
                    RankingList.Add(userInfo.Name, userInfo.Score);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"유저 데이터를 읽는데 실패했습니다.: {e.Message}");
        }
    }

    async Task ReadItemData()
    {
        try
        {
            DataSnapshot dataSnapshot = await reference.GetValueAsync();
            string jsonData = dataSnapshot.GetRawJsonValue();
            string savePath = Path.Combine(Application.persistentDataPath, $"Data/{DataType.ItemData.ToString()}.json");
            File.WriteAllText(savePath, jsonData);
            DataManager.Instance?.ReloadData();
        }
        catch (Exception e)
        {
            Debug.LogError($"아이템 데이터를 읽는데 실패했습니다.: {e.Message}");
        }
    }

    async Task ReadCustomData()
    {
        try
        {
            DataSnapshot dataSnapshot = await reference.GetValueAsync();
            string jsonData = dataSnapshot.GetRawJsonValue();
            string savePath = Path.Combine(Application.persistentDataPath, $"Data/{DataType.CustomData.ToString()}.json");
            File.WriteAllText(savePath, jsonData);
            DataManager.Instance?.ReloadData();
        }
        catch (Exception e)
        {
            Debug.LogError($"아이템 데이터를 읽는데 실패했습니다.: {e.Message}");
        }
    }
}



using Firebase;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.IO;
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
    public static DatabaseManager Instance => instance;
    public bool isReadDB = true;
    public Dictionary<string,int> RankingList = new Dictionary<string, int>(); // 유저 랭킹 리스트 딕셔너리
    public DatabaseReference reference { get; set; }
    [SerializeField] private string DB_URL = "https://animalland-d2718-default-rtdb.firebaseio.com"; // DB URL

    private static DatabaseManager instance = null;

    void Start()
    {
        instance = this;
        DontDestroyOnLoad(instance);
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new Uri(DB_URL);
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        WriteDB();
        ReadDB(DataType.Users);
    }

    void WriteDB() // TODO : 자기 자신의 아이디와 닉네임으로 저장할 수 있는 함수 구현
    {
        string uId = this.GetComponent<WJ_Connector>().UserID;

        Rank rankDate = new Rank("Player" + uId.Substring(16, 4), 0);

        string jsonData = JsonUtility.ToJson(rankDate);
        reference.Child("Users").Child(uId).SetRawJsonValueAsync(jsonData);

        //string js = JsonConvert.SerializeObject(DataManager.Instance.CharacterCustomData);
        //reference.Child("CustomData").SetRawJsonValueAsync(js);
    }

    public void ReadDB(DataType root) // DB 읽어오는 메소드
    { 
        reference = FirebaseDatabase.DefaultInstance.GetReference(root.ToString());

        try
        {
            switch(root)
            {
                case DataType.Users: // 유저 정보
                    ReadUsersData();
                    break;
                case DataType.ItemData: // 상점 아이템 정보
                     ReadItemData();
                    break;
                case DataType.CustomData: // 만약 처음 실행이라면 커스텀 목록 json 파일 형식 받아와서 저장
                    ReadCustomData();
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"데이터 베이스를 읽는데 실패했습니다.: {e.Message}");
        }
    }

    async void ReadUsersData()
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
            isReadDB = false;
        }
        catch (Exception e)
        {
            Debug.LogError($"유저 데이터를 읽는데 실패했습니다.: {e.Message}");
        }
    }

    async void ReadItemData()
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

    async void ReadCustomData()
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



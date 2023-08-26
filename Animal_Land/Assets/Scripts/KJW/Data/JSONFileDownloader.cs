using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Collections;

public class JSONFileDownloader : MonoBehaviour
{
    public bool isDownloading = false;
    private string localFilePath;
    private const string itemDataFileURL = "https://docs.google.com/uc?id=1QPD8tKtIZ4yHys9GV-ksrj0XZ2lU0BiS&export=download"; // Replace this with the actual URL
    public void NeedDownload(string filePath,string fileName)
    {
#if UNITY_ANDROID
        localFilePath = Path.Combine(Application.persistentDataPath, $"Data/{fileName}.json");
#elif UNITY_EDITOR
        localFilePath = Path.Combine(Application.dataPath, $"StreamingAssets/Data/{fileName}.json");
#endif
        StartCoroutine(DownloadFile(filePath));
    }

    private IEnumerator DownloadFile(string filePath)
    {

        isDownloading = true;

        UnityWebRequest www = UnityWebRequest.Get(itemDataFileURL);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            byte[] data = www.downloadHandler.data;
            string directoryPath = Path.GetDirectoryName(localFilePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            File.WriteAllBytes(localFilePath, data);
        }
        else
        {
            Debug.LogError("File download failed: " + www.error);
        }

        isDownloading = false;
    }
}

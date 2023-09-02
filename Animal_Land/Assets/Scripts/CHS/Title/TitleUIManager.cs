using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUIManager : MonoBehaviour
{
    [Header("타이틀 텍스트 리스트")]
    [SerializeField]
    private string TestText = "Touch To Test";
    [SerializeField]
    private string GoToLobbyText = "Go To Lobby";

    [Header("버튼")]
    public Button BtChangeScene;
    public TextMeshProUGUI BtText;

    [Header("패널")]
    public GameObject HowToPlayPanel;

    // 멤버 변수
    private bool _isCanMoveToLobby = false;

    public void SetCanMoveToLobby(bool value)
    {
        _isCanMoveToLobby = value;
        ChangeButtonText(_isCanMoveToLobby);
    }
    public void JoinRoom()
    {
        if (_isCanMoveToLobby)
        {
            GoToLobby();
        }
        else
        {
            GoToTest();
        }
    }

    void ChangeButtonText(bool isCanMoveToLobby)
    {
        if (isCanMoveToLobby)
        {
            BtText.text = GoToLobbyText;
        }
        else
        {
            BtText.text = TestText;
        }
    }

    async Task GoToLobby()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable) // 데이터 다운로드 또는 인터넷 연결
        {
            TitlePopUp popUp = ViewManager.GetView<TitlePopUp>();
            if (popUp != null)
            {
                popUp.SetCheckMessage("Check your internet connection.");
                ViewManager.Show<TitlePopUp>(true, true);
            }
            Debug.LogError("인터넷 연결을 확인하세요.");
            return;
        }

        if (DataManager.Instance.PropsItemDict.Count <= 0) // 데이터가 안불러와진 경우
        {
            TitlePopUp popUp = ViewManager.GetView<TitlePopUp>();
            if (popUp != null)
            {
                popUp.SetCheckMessage("Drawing the ground...");
                ViewManager.Show<TitlePopUp>(true, true);
            }

            Debug.LogError("데이터를 다운받는 중입니다.");
            if (DatabaseManager.Instance != null)
            {
              await DatabaseManager.Instance.ReadDB(DataType.ItemData);
            }
            return;
        }

        if(DataManager.Instance.CharacterCustomData.Count <= 0)
        {
            TitlePopUp popUp = ViewManager.GetView<TitlePopUp>();
            if (popUp != null)
            {
                popUp.SetCheckMessage("I'm almost done!!");
                ViewManager.Show<TitlePopUp>(true, true);
            }

            if (DatabaseManager.Instance != null)
            {
                await DatabaseManager.Instance.ReadDB(DataType.CustomData);
            }
            return;
        }

        ChangeRoom("Lobby 1");
    }

    void GoToTest()
    {
        ChangeRoom("Test");
    }

    void ChangeRoom(string roomName)
    {
        GetComponent<SoundManager>().PlayEffect(Effect.Button);

        SceneManager.LoadScene(roomName);
    }

    public void OpenHowToPlay()
    {
        GetComponent<SoundManager>().PlayEffect(Effect.Button);
        HowToPlayPanel.SetActive(true);
    }

    public void CloseHowToPlay()
    {
        GetComponent<SoundManager>().PlayEffect(Effect.Back);
        HowToPlayPanel.SetActive(false);
    }
}

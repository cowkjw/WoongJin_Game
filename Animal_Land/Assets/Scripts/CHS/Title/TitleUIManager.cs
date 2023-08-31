using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUIManager : MonoBehaviour
{
    [Header("Ÿ��Ʋ �ؽ�Ʈ ����Ʈ")]
    [SerializeField]
    private string TestText = "Touch To Test";
    [SerializeField]
    private string GoToLobbyText = "Go To Lobby";

    [Header("��ư")]
    public Button BtChangeScene;
    public TextMeshProUGUI BtText;

    [Header("�г�")]
    public GameObject HowToPlayPanel;

    // ��� ����
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
        if (Application.internetReachability == NetworkReachability.NotReachable) // ������ �ٿ�ε� �Ǵ� ���ͳ� ����
        {
            TitlePopUp popUp = ViewManager.GetView<TitlePopUp>();
            if (popUp != null)
            {
                popUp.SetCheckMessage("Check your internet connection.");
                ViewManager.Show<TitlePopUp>(true, true);
            }
            Debug.LogError("���ͳ� ������ Ȯ���ϼ���.");
            return;
        }

        if (DataManager.Instance.PropsItemDict.Count <= 0) // �����Ͱ� �Ⱥҷ����� ���
        {
            TitlePopUp popUp = ViewManager.GetView<TitlePopUp>();
            if (popUp != null)
            {
                popUp.SetCheckMessage("Drawing the ground...");
                ViewManager.Show<TitlePopUp>(true, true);
            }

            Debug.LogError("�����͸� �ٿ�޴� ���Դϴ�.");
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
                popUp.SetCheckMessage("I'm almost done!!!");
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

using System.Collections.Generic;
using UnityEngine;

public class ViewManager : MonoBehaviour
{
    private static ViewManager instance;
    public static ViewManager Instance
    {
        get
        {
            if (instance == null)
                instance = new ViewManager();
            return instance;
        }
    }

    [Header("시작 화면")]
    [SerializeField]
    private View startingView; // 시작 화면
    [Header("창 목록")]
    [SerializeField]
    private List<View> views = new List<View>();

    private View _currentView; // 현재 창

    private readonly Stack<View> _viewHistory = new Stack<View>(); // UI Stack

    private void Awake()
    {
        instance = this; // 싱글톤 인스턴스 
    }

    private void Start()
    {
        foreach (var view in Instance.views) // 순회하면서 초기화 및 숨기기
        {
            view.Initialize();

            view.Hide();
        }

        if (startingView != null) // 시작할 때 보여질 창
        {
            Show(startingView); // 처음 시작하는 화면은 기억하기
        }
    }

    public static T GetView<T>() where T : View // View를 상속받거나 하는 객체들로 한정
    {
        foreach (var view in Instance.views)
        {
            if (view is T tView)
            {
                return tView;
            }
        }
#if UNITY_EDITOR
        Debug.LogWarning("UI 찾을 수 없음");
#endif
        return null;
    }

    public static void Show<T>(bool remember = true, bool popUpUI = false) where T : View
    {
        foreach (var view in Instance.views)
        {
            if (view is T)
            {
                if (Instance._currentView != null) // 현재 창이 null이 아니면
                {
                    if (remember) // 기억해야하는 거라면 
                    {
                        Instance._viewHistory.Push(Instance._currentView); // stack에 삽입
                    }

                    if (!popUpUI) // popup이 아니면
                    {
                        Instance._currentView.Hide(); // 현재 창은 숨기기
                    }
                }

                view.Show(); // 새로 보여질 창 보이기
                Instance._currentView = view; // 현재 창으로 변경
                return;
            }
        }
    }

    public static void Show(View view, bool remember = true)
    {
        if (Instance._currentView != null) // 현재 보여지는 창이 null이면 return
        {
            if (remember) // 기억해야하는 창이라면
            {
                Instance._viewHistory.Push(Instance._currentView);
            }

            Instance._currentView.Hide(); // 현재 창은 숨긴다
        }

        view.Show(); // 선택 한 창을 보여준다

        Instance._currentView = view; // 현재 창을 선택 창으로 변경
    }

    public static void ShowLast() // 마지막 창(이전 창) 보여주기
    {
        if (Instance._viewHistory.Count > 0)
        {
            Show(Instance._viewHistory.Pop(), false);
        }
    }
}

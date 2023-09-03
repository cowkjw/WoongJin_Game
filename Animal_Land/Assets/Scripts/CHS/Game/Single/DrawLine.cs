using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    [SerializeField]
    public GameObject Prefab_line;

    [SerializeField]
    public SpriteRenderer DrawingPaper;

    LineRenderer lr;
    List<Vector2> List_Points;
    Stack<GameObject> Stack_Line;   // 되돌리기 기능을 위해 오브젝트를 Stack으로 보관
    GameObject ob;

    private void Start()
    {
        List_Points = new List<Vector2>();
        if (List_Points == null)
        {
            Debug.LogError("List_Points가 생성되지 않았습니다.");
        }

        Stack_Line = new Stack<GameObject>();
        if (Stack_Line == null)
        {
            Debug.LogError("Stack_Line이 생성되지 않았습니다.");
        }
    }

    private void Update()
    {
        if (DetectHoverDrawingPaper() == false)
        {
            return;
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // 시작 위치에서 생성
            GameObject go = Instantiate(Prefab_line);
            ob = go;
            if (go == null)
            {
                Debug.LogError("Prefab_Line을 생성하지 못하였습니다.");
                return;
            }

            lr = go.GetComponent<LineRenderer>();
            if (lr == null)
                return;

            // 입력된 마우스의 위치값을 리스트에 추가
            List_Points.Add(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            // Line Renderer에 카운트를 증가시키고 그림을 그린다.
            lr.positionCount = 1;
            lr.SetPosition(0, List_Points[0]);

            // 스택에 Line을 추가한다.
            Stack_Line.Push(go);
        }
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 추가적으로 Line Renderer를 이용하여 그린다.
            List_Points.Add(pos);
            if (ob != null)
                return;

            lr = ob.GetComponent<LineRenderer>();
            if (lr == null)
                return;

            lr.positionCount++;
            lr.SetPosition(lr.positionCount - 1, pos);
        }
        else if (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled))
        {
            List_Points.Clear();
        }
    }

    private bool DetectHoverDrawingPaper()
    {
        // 마우스의 위치를 스크린 좌표에서 월드 좌표로 변환
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 레이캐스트를 통해 충돌 여부 확인
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity);

        // 충돌한 스프라이트가 있는지 확인
        if (hit.collider != null)
        {
            return hit.collider.gameObject.tag == "paper" ? true : false;
        }
        else
        {
            return false;
        }
    }

    public void RevertLine()
    {
        if (Stack_Line == null || Stack_Line.Count == 0) return;

        // Stack에서 라인을 받아와서 Destroy
        GameObject line = Stack_Line.Pop();

        if (line == null) return;

        // 효과음 추가
        GetComponent<SoundManager>().PlayEffect(Effect.Erase);

        GameObject.Destroy(line);
    }

    public void ClearLineNS()
    {
        if (Stack_Line == null || Stack_Line.Count == 0) return;

        while (Stack_Line.Count > 0)
        {
            RevertLine();
        }
    }

    public void ClearLine()
    {
        ClearLineNS();

        // 효과음 추가
        GetComponent<SoundManager>().PlayEffect(Effect.Erase);
    }
}

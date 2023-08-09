using UnityEngine;

public abstract class View : MonoBehaviour // 추상 클래스 구현 (각자 초기화 할 게 다름)
{
    public abstract void Initialize();

    public virtual void Hide() => gameObject.SetActive(false);

    public virtual void Show() => gameObject.SetActive(true);
}

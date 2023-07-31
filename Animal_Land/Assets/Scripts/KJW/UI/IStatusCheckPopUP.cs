using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 체크화면 팝업에서 유연하게 사용하기 위해서
public interface IStatusCheckPopUP // 확인용 팝업 UI 인터페이스(사용할지는 모르겠음..)
{
    public void SetCheckMessage(string message);
}

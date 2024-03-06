using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI_Popup : UI_Base
{
    public override bool Init()
    {
        base.Init();
        return true;
    }

    public virtual void ClosePopupUI()
    {
        //Manager.Sound.Play(Define.Sound.Effect, "ClosePopup");
        Manager.UI.ClosePopupUI(this);
    }
}

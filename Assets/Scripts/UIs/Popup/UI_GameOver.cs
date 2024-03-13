using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GameOver : UI_Popup
{
    enum Buttons
    {
        RegenButton
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindButton(typeof(Buttons));
        GetButton((int)Buttons.RegenButton).onClick.AddListener(ClickButton);
        return true;
    }

    void ClickButton()
    {
        Manager.Scene.RegenLoad();
    }

}

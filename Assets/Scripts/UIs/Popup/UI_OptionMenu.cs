using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_OptionMenu : UI_Popup
{
    enum Buttons
    {
        ReturnButton,
        Restart,
        TitleScene
    }

    public override bool Init()
    {
        base.Init();
        BindButton(typeof(Buttons));
        GetButton((int)Buttons.ReturnButton).onClick.AddListener(ReturnBtnClick);
        GetButton((int)Buttons.Restart).onClick.AddListener(RestartBtnClick);
        GetButton((int)Buttons.TitleScene).onClick.AddListener(TitleSceneBtnClick);
        return true;
    }


    private void ReturnBtnClick()
    {
        Debug.Log("클릭");
        Manager.UI.ClosePopupUI(this);
    }

    private void RestartBtnClick()
    {
        Debug.Log("클릭");
        Manager.Scene.LoadScene(Define.Scene.GameScene);
    }

    private void TitleSceneBtnClick()
    {
        Debug.Log("클릭");
        Manager.Scene.LoadScene(Define.Scene.TitleScene);
    }


}

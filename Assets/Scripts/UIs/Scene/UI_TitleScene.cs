using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_TitleScene : UI_Scene
{
    enum Buttons
    {
        StartButton
    }

    private void Update()
    {
        if (Input.anyKeyDown)
            ClickStartButton();
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        BindButton(typeof(Buttons));
        GetButton((int)Buttons.StartButton).onClick.AddListener(ClickStartButton);
        return true;
    }

    void ClickStartButton()
    {
        Manager.Scene.LoadScene(Define.Scene.GameScene);
    }
}

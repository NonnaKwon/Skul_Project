using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.TitleScene;
        UI_TitleScene titleSceneUI = Manager.Resource.Load<UI_TitleScene>("Prefabs/UIs/Scene/UI_TitleScene");
        Manager.UI.ShowInGameUI<UI_TitleScene>("UI_TitleScene");
        Manager.Game.IsBoss = false;
    }

    public override IEnumerator LoadingRoutine()
    {
        yield return null;
        Init();
    }

    public override void Clear()
    {
        Debug.Log("LoginScene Clear!");
    }
}

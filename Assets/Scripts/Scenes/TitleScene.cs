using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.Title;
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

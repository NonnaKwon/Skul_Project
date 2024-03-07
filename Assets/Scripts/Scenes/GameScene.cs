using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    PooledObject _doubleJumpEffect;
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.TitleScene;
        _doubleJumpEffect = Manager.Resource.Load<PooledObject>("Prefabs/Effects/JumpEffect");
        Manager.Pool.CreatePool(_doubleJumpEffect, 3, 3);
    }

    public override IEnumerator LoadingRoutine()
    {
        yield return null;
        Init();
    }

    public override void Clear()
    {
        Debug.Log("GameScene Clear!");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.TitleScene;
        Manager.Game.Player = FindObjectOfType<PlayerController>();

        PooledObject doubleJumpEffect = Manager.Resource.Load<PooledObject>("Prefabs/Effects/JumpEffect");
        PooledObject dashEffect = Manager.Resource.Load<PooledObject>("Prefabs/Effects/DashEffect");
        Manager.Pool.CreatePool(doubleJumpEffect, 3, 3);
        Manager.Pool.CreatePool(dashEffect, 5, 5);
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

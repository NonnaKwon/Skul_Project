using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameScene : BaseScene
{
    public UI_GameScene UI_GameScene;

    UI_OptionMenu _ui_OptionMenu;
    protected override void Init()
    {
        base.Init();
        SceneType = Define.Scene.TitleScene;
        Manager.Game.Player = FindObjectOfType<PlayerController>();

        PooledObject doubleJumpEffect = Manager.Resource.Load<PooledObject>("Prefabs/Effects/JumpEffect");
        PooledObject dashEffect = Manager.Resource.Load<PooledObject>("Prefabs/Effects/DashEffect");
        PooledObject attackEffect = Manager.Resource.Load<PooledObject>("Prefabs/Effects/AttackEffect");
        PooledObject dieEffect = Manager.Resource.Load<PooledObject>("Prefabs/Effects/DieEffect");
        Debug.Log(dieEffect);

        Manager.Pool.CreatePool(doubleJumpEffect, 3, 3);
        Manager.Pool.CreatePool(dashEffect, 5, 5);
        Manager.Pool.CreatePool(attackEffect, 10, 10);
        Manager.Pool.CreatePool(dieEffect, 5, 5);

        _ui_OptionMenu = Manager.Resource.Load<UI_OptionMenu>("Prefabs/UIs/Popup/UI_OptionMenu");
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

    private void OnOption(InputValue value)
    {
        Debug.Log("옵션 확인");
        Manager.UI.ShowPopUpUI<UI_OptionMenu>(_ui_OptionMenu);
    }
}

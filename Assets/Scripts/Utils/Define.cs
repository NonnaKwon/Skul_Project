using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public const int MAX_JUMP_COUNT = 2;

    public enum Scene
    {
        TitleScene,
        GameScene,
        None
    }

    public enum CurMap
    {

    }

    public enum PlayerState
    {
        Idle,
        Damaged,
        Interact,
        Die,
    }

    public enum MonsterState
    {
        Idle,
        Trace,
        Damaged,
        Attack,
        Die
    }


    public enum UIEvent
    {
        Click,
        PointerDown,
        PointerUp,
        Drag,
    }

    public enum MouseEvent
    {
        Press,
        PointerDown,
        PointerUp,
        Click,
    }

}

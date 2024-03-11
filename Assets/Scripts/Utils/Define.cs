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

    public enum GameSceneState
    {
        TutorialMap01,
        TutorialMap02
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

    public enum StoryIndex
    {
        Tutorial01_1,
        Tutorial01_2,
        Tutorial01_3,
        Tutorial01_4,
    }

}

using UnityEngine;
using static Define;

public class GameManager : Singleton<GameManager>
{
    public PlayerController Player;
    public StoryIndex CurrentStory = StoryIndex.Tutorial01_1;
    GameSceneState _sceneState = GameSceneState.TutorialMap01;

    private void Start()
    {

    }
}

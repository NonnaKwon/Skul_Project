using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public PlayerController Player;
    public void Test()
    {
        Debug.Log(GetInstanceID());
    }
}

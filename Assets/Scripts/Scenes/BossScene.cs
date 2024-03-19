using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScene : MonoBehaviour
{
    private void Awake()
    {
        Manager.Game.IsBoss = true;
    }
}

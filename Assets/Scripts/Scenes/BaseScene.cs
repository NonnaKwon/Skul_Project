using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseScene : MonoBehaviour
{
    public Define.Scene SceneType { get; protected set; } = Define.Scene.None;
    public abstract IEnumerator LoadingRoutine();

    void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
    }


    public abstract void Clear();
}

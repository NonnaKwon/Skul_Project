using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skul : Head
{

    private void Awake()
    {
        Data = Manager.Resource.Load<HeadData>("Data/Head/Skul");
    }
    public override void InputHead()
    {

    }

    public override void Skill()
    {
        Debug.Log("Skul : Skill");

    }
}

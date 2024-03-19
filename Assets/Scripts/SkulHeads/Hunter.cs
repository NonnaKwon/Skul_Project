using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunter : Head
{
    private void Start()
    {
        Data = Manager.Resource.Load<HeadData>("Data/Head/Hunter");
    }
    public override void InputHead()
    {
        throw new System.NotImplementedException();
    }

    public override bool SkillA()
    {
        throw new System.NotImplementedException();
    }

    public override bool SkillS()
    {
        throw new System.NotImplementedException();
    }
}

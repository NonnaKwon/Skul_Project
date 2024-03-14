using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Head : MonoBehaviour
{
    public HeadData Data;

    public abstract void InputHead();
    public abstract void SkillS();
    public abstract void SkillA();
}

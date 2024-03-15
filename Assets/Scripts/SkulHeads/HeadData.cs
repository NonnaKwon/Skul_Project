using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu (fileName = "HeadData",menuName = "Data/Head")]
public class HeadData : ScriptableObject
{
    public Sprite sprite;
    public AnimatorController animator;
    public string headName;
    public int jumpCount;
    public float maxHp;
    public float power;
    public float skillPower;
    public float defencePower;
    public int attackCount;
    public float coolTimeA;
    public float coolTimeS;
}

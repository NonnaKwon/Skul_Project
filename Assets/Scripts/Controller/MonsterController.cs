using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour, IDamagable
{
    public void TakeDamage(float damage)
    {
        Debug.Log("데미지를 받았다!");
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class AttackPointSkill : MonoBehaviour
{
    [SerializeField] LayerMask _attackMask;
    public float SkillPower;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_attackMask.Contain(collision.gameObject.layer))
        {
            collision.GetComponent<IDamagable>().TakeDamage(SkillPower);
        }
    }
}

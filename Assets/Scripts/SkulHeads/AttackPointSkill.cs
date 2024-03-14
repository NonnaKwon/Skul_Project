using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class AttackPointSkill : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        MonsterController monster = collision.gameObject.GetComponent<MonsterController>();
        if (monster != null)
        {
            monster.TakeDamage(Manager.Game.Player.CurrentHead.Data.skillPower);
        }
    }
}

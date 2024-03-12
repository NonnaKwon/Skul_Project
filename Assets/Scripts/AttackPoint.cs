using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPoint : MonoBehaviour
{
    [SerializeField] LayerMask _mask;
    [SerializeField] float _attackRange = 1;


    IAttackable _fightController;
    Collider2D[] colliders = new Collider2D[30];

    private void Start()
    {
        _fightController = GetComponentInParent<IAttackable>();
    }
    public void Attack()
    {
        int size = Physics2D.OverlapCircleNonAlloc(transform.position, _attackRange, colliders,_mask);
        Debug.Log(size);
        for (int i = 0; i < size; i++)
        {
            IDamagable damagable = colliders[i].gameObject.GetComponent<IDamagable>();
            if (damagable != null)
                damagable.TakeDamage(_fightController.GetPower());
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}

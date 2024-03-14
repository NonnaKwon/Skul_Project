using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkulSkillObject : MonoBehaviour
{
    private float throwSpeed = 20;
    Rigidbody2D _rigid;
    PooledObject _fallEffect;
    Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigid = GetComponent<Rigidbody2D>();
        _fallEffect = Manager.Resource.Load<PooledObject>("Prefabs/Effects/AttackEffect");
        StartCoroutine(ThrowHead());
    }


    IEnumerator ThrowHead()
    {
        _rigid.isKinematic = true;
        if (Manager.Game.Player.IsRight)
            _rigid.velocity = Vector2.right * throwSpeed;
        else
            _rigid.velocity = Vector2.left * throwSpeed;
        yield return new WaitForSeconds(1.5f);
        _animator.Play("Fall");
        Manager.Pool.GetPool(_fallEffect, transform.position, transform.rotation);
        _rigid.isKinematic = false;
        _rigid.velocity = Vector2.zero;
    }
}

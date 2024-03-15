using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        Debug.Log(Manager.Game.Player.IsRight); 
        _rigid.isKinematic = true;
        if (Manager.Game.Player.IsRight)
            _rigid.velocity = Vector2.right * throwSpeed;
        else
            _rigid.velocity = Vector2.left * throwSpeed;
        yield return new WaitForSeconds(1.5f);
        Fall();
    }

    private void Fall()
    {
        _animator.Play("Fall");
        Manager.Pool.GetPool(_fallEffect, transform.position, transform.rotation);
        _rigid.isKinematic = false;
        _rigid.velocity = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
            player.CurrentHead.InputHead();
        else
            Fall();
    }
}

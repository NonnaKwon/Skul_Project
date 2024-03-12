using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;
using static Define;

public class FightController : MonoBehaviour,IDamagable,IAttackable
{
    [SerializeField] AttackPoint _baseAttackPoint;
    [SerializeField] int _maxAttackCount;

    Animator _animator;

    //플레이어면, 플레이어 컨트롤러에서 대가리바뀔때마다 (이름, 스킬, 점프파워) 얘도 바뀜
    private float _maxHp;
    private float _hp;
    private float _power;
    private float _defencePower;

    private int _attackCount;
    private Vector3 _attackPointPosition;

    PlayerController _controller;
    PooledObject _damageEffect;

    private void Start()
    {

        _attackCount = 0;
        _maxHp = 100;
        _hp = _maxHp;
        _animator = GetComponent<Animator>();
        _attackPointPosition = _baseAttackPoint.gameObject.GetComponent<Transform>().localPosition;
        _controller = GetComponent<PlayerController>();
        _damageEffect = Resources.Load("Prefabs/Effects/AttackEffect").GetComponent<PooledObject>();
    }

    public float GetPower() { 
        return _power; 
    } //어택포인트에 보냄

    public void TakeDamage(float damage)
    {
        Debug.Log("Player : 데미지를 받았다!");
        _hp -= damage;
        StartCoroutine(CoTakeDamage());
    }


    IEnumerator CoTakeDamage()
    {
        _controller.StateMachine.ChangeState(PlayerState.Damaged);
        yield return new WaitForSeconds(0.1f);
        Vector3 randomVec = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0);
        Manager.Pool.GetPool(_damageEffect, transform.position + randomVec, transform.rotation);
        yield return new WaitForSeconds(0.5f);
        _controller.StateMachine.ChangeState(PlayerState.Idle);
    }
    public void Attack()
    {
        _attackCount++;
        if (_attackCount > _maxAttackCount)
            return;
        _animator.SetInteger("AttackCount", _attackCount);
        StartCoroutine(CoAttack());
    }

    private void OnAttack(InputValue value)
    {
        if(_attackCount < _maxAttackCount)
        {
            Attack();
        }
    }

    private void OnMove(InputValue value)
    {
        float movePos = 0;
        if (value.Get<Vector2>().x < 0)
            movePos = _attackPointPosition.x < 0 ? _attackPointPosition.x : _attackPointPosition.x * -1;
        else if (value.Get<Vector2>().x > 0)
            movePos = _attackPointPosition.x > 0 ? _attackPointPosition.x : _attackPointPosition.x * -1;
        else
            movePos = _attackPointPosition.x;
        _attackPointPosition.x = movePos;
        _baseAttackPoint.transform.localPosition = new Vector3(movePos, _attackPointPosition.y, _attackPointPosition.z);
    }



    private IEnumerator CoAttack()
    {
        _baseAttackPoint.Attack();
        yield return new WaitForSeconds(0.5f);
        if (_attackCount > 1)
            yield return new WaitForSeconds(0.3f * _maxAttackCount);
        _attackCount = 0;
        _animator.SetInteger("AttackCount", _attackCount);
    }

}

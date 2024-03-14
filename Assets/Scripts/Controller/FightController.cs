using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Define;

public class FightController : MonoBehaviour,IDamagable,IAttackable
{
    [SerializeField] AttackPoint _baseAttackPoint;
    [SerializeField] UI_GameScene _connectUI;

    public float MaxHp { get { return _currentHead.Data.maxHp; } }
    public float Hp { get { return _hp; } }

    private float _hp;
    private Vector3 _attackPointPosition;
    PlayerController _controller;
    PooledObject _damageEffect;
    Animator _animator;
    Head _currentHead;
    Rigidbody2D _rigid;

    int _attackCount;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _attackPointPosition = _baseAttackPoint.gameObject.GetComponent<Transform>().localPosition;
        _damageEffect = Resources.Load("Prefabs/Effects/AttackEffect").GetComponent<PooledObject>();
        _controller = GetComponent<PlayerController>();
        _rigid = GetComponent<Rigidbody2D>();

        _currentHead = _controller.CurrentHead;
        _hp = _currentHead.Data.maxHp;
        PlayerInit();
    }

    private void Update()
    {
        if (_hp <= 0 && Manager.Game.Player.StateMachine.CurState != PlayerState.Die)
            Manager.Game.Player.StateMachine.ChangeState(PlayerState.Die);
    }

    public void PlayerInit()
    {
        _attackCount = 0;
        if(_currentHead != null)
        {
            _hp = _currentHead.Data.maxHp;
            _connectUI.InitHPBar();
        }
    }

    public float GetPower() { 
        return _currentHead.Data.power; 
    } //어택포인트에 보냄

    public void TakeDamage(float damage)
    {
        Debug.Log("Player : 데미지를 받았다!");
        _hp -= damage;
        _connectUI.DecreaseHP(damage);
        StartCoroutine(CoTakeDamage());
    }


    IEnumerator CoTakeDamage()
    {
        _animator.Play("Damaged");
        if (_controller.IsRight) //왼쪽으로 돌아있으면
            _rigid.velocity = new Vector2(DAMAGED_POWER, 0);
        else
            _rigid.velocity = new Vector2(-DAMAGED_POWER, 1);
        yield return new WaitForSeconds(0.1f);
        Vector3 randomVec = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0);
        Manager.Pool.GetPool(_damageEffect, transform.position + randomVec, transform.rotation);
        yield return new WaitForSeconds(0.5f);
    }
    public void Attack()
    {
        _attackCount++;
        if (_attackCount > _currentHead.Data.attackCount)
            return;
        _animator.SetInteger("AttackCount", _attackCount);
        StartCoroutine(CoAttack());
    }


    private void OnAttack(InputValue value)
    {
        if(_attackCount < _currentHead.Data.attackCount)
            Attack();
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

    private void OnSkillS(InputValue value)
    {
        _currentHead.SkillS();
    }
    
    private void OnSkillA(InputValue value)
    {
        _currentHead.SkillA();
    }



    private IEnumerator CoAttack()
    {
        _baseAttackPoint.Attack();
        yield return new WaitForSeconds(0.5f);
        if (_attackCount > 1)
            yield return new WaitForSeconds(0.3f * _attackCount);
        _attackCount = 0;
        _animator.SetInteger("AttackCount", _attackCount);
    }
}

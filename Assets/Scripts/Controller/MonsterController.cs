using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MonsterController : MonoBehaviour, IDamagable, IAttackable
{
    [SerializeField] AttackPoint _baseAttackPoint;

    private int _maxHp;
    private int _hp;
    private float _power;
    private float _defencePower;
    private float _moveSpeed;

    private int _maxAttackCount;
    private int _attackCount;
    private float _traceRange;
    private float _attackRange;
    private bool _targetFind;

    Animator _animator;
    GameObject _target;
    Rigidbody2D _rigid;
    SpriteRenderer _renderer;

    private void Update()
    {
        if (Vector2.Distance(_target.transform.position, transform.position) < _attackRange)
        {
            Attack();
            _animator.SetBool("IsMove", false);
            return;
        }

        if (_targetFind)
        {
            Move();
            
        }
        else if(Vector2.Distance(_target.transform.position, transform.position) < _traceRange)
        {
            Debug.Log("타겟이 범위에 들어왔다");
            _targetFind = true;
            _animator.SetBool("IsMove", true);
        }
    }

    private void Start()
    {
        _maxAttackCount = 1;
        _attackCount = 0;
        _maxHp = 20;
        _traceRange = 10;
        _attackRange = 3;
        _hp = _maxHp;
        _moveSpeed = 5f;
        _targetFind = false;
        _animator = GetComponent<Animator>();
        _rigid = GetComponent<Rigidbody2D>();
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _target = FindObjectOfType<PlayerController>().gameObject;
    }

    private void Move()
    {
        if (_attackCount > 0)
            return;
        Vector2 targetRotation = (_target.transform.position - transform.position).normalized;
        if(targetRotation.x < 0)
            _renderer.flipX = true;
        else
            _renderer.flipX = false;
        transform.Translate(new Vector3(targetRotation.x * _moveSpeed * Time.deltaTime,0,0),Space.World);
    }

    public float GetPower() 
    { 
        return _power; 
    }

    public void Attack()
    {
        _attackCount++;
        if (_attackCount > _maxAttackCount)
            return;
        _animator.SetInteger("AttackCount", _attackCount);
        _animator.SetBool("IsMove", false);
        StartCoroutine(CoAttack());
    }

    private IEnumerator CoAttack()
    {
        Debug.Log("공격");
        _baseAttackPoint.Attack();
        yield return new WaitForSeconds(0.5f);
        if (_attackCount > 1)
            yield return new WaitForSeconds(0.3f * _maxAttackCount);
        _attackCount = 0;
        _animator.SetInteger("AttackCount", _attackCount);
        _animator.SetBool("IsMove", true);
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("데미지를 받았다!");
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] LayerMask _groundFind;

    //대가리 (이름, 스킬, 점프파워)
    private int _maxHp;
    private int _hp;
    private float _moveSpeed;
    private float _jumpPower;
    private int _jumpCount;
    

    Vector3 _moveDir;
    Rigidbody2D _rigid;
    Animator _animator;
    SpriteRenderer _renderer;
    Platform _onPlatform;

    PooledObject _jumpEffect;
    
    private void Start()
    {
        _jumpCount = 0;
        _rigid = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _moveSpeed = 8.4f;
        _jumpPower = 13f;
        _maxHp = 100;
        _hp = _maxHp;

        _jumpEffect = Manager.Resource.Load<PooledObject>("Prefabs/Effects/JumpEffect");
    }

    private void Update()
    {
        Move();
        if (_jumpCount > 0)
            _animator.SetFloat("ySpeed", _rigid.velocity.y);
    }

    private void Move()
    {
        transform.Translate(_moveDir.x * _moveSpeed * Time.deltaTime, 0, 0);
        if (_moveDir.x != 0)
        {
            _animator.SetBool("IsWalk", true);
            if (_moveDir.x < 0)
                _renderer.flipX = true;
            else
                _renderer.flipX = false;
        }
        else
            _animator.SetBool("IsWalk", false);
    }

    private void Jump()
    {
        _jumpCount++;
        if (_jumpCount == 2)
        {
            Vector3 insPosition = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
            Manager.Pool.GetPool(_jumpEffect, insPosition, transform.rotation);
        }
        _rigid.velocity = new Vector2(_rigid.velocity.x, _jumpPower);
    }

    private void PlatformDown()
    {
        _onPlatform.TurnOnTrigger();
    }

    private void OnJump(InputValue value)
    {
        if(_jumpCount < Define.MAX_JUMP_COUNT)
            Jump();
    }

    private void OnDown(InputValue value)
    {
        if(_onPlatform != null)
            PlatformDown();
    }

    private void OnMove(InputValue value)
    {
        Vector2 moveDistance = value.Get<Vector2>();
        _moveDir.x = moveDistance.x;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_groundFind.Contain(collision.gameObject.layer))
        {
            _jumpCount = 0;
            _onPlatform = collision.gameObject.GetComponent<Platform>();
            _animator.SetFloat("ySpeed", 0);
        }
    }
}

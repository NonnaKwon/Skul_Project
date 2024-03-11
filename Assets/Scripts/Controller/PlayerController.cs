using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //플레이어의 상태 : Idle, Attack, Interact, Die, 만약 공격중(점프공격아님)이면 움직일수없게
    public float XVelocity { get { return _rigid.velocity.x; } }
    public bool Movable { get; set; } = true;
    [SerializeField] LayerMask _groundFind;

    private float _moveSpeed;
    private float _jumpPower;
    private float _dashPower;
    private int _jumpCount;
    private int _inputMoveCount;
    private char _dashDir;
   
    Vector3 _moveDir;
    Rigidbody2D _rigid;
    Animator _animator;
    SpriteRenderer _renderer;
    Platform _onPlatform;

    PooledObject _jumpEffect;
    PooledObject _dashEffect;
    private void Start()
    {
        _jumpCount = 0;
        _rigid = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _moveSpeed = 8.4f;
        _jumpPower = 13f;
        _dashPower = 20f;
        _jumpEffect = Manager.Resource.Load<PooledObject>("Prefabs/Effects/JumpEffect");
        _dashEffect = Manager.Resource.Load<PooledObject>("Prefabs/Effects/DashEffect");
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

    private void Dash()
    {
        Debug.Log("Dash Play");

        _animator.Play("Dash");
        Vector3 insPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Manager.Pool.GetPool(_dashEffect, insPosition, transform.rotation);

        _rigid.AddForce(Vector2.right * _moveDir * _dashPower,ForceMode2D.Impulse);
        _rigid.velocity = new Vector2(_moveDir.x * _dashPower, _rigid.velocity.y);
    }

    private void OnJump(InputValue value)
    {
        if (!Movable)
            return;
        if (_jumpCount < Define.MAX_JUMP_COUNT)
            Jump();
    }

    private void OnDown(InputValue value)
    {
        if (!Movable)
            return;
        if (_onPlatform != null)
            PlatformDown();
    }

    private void OnMove(InputValue value)
    {
        if (!Movable)
            return;
        Vector2 moveDistance = value.Get<Vector2>();
        _moveDir.x = moveDistance.x;
    }

    private void OnDashR(InputValue value)
    {
        if (!Movable)
            return;
        StartCoroutine(CoDash('R'));
    }

    private void OnDashL(InputValue value)
    {
        if (!Movable)
            return;
        StartCoroutine(CoDash('L'));
    }

    IEnumerator CoDash(char dir)
    {
        float correntVelocityX = _rigid.velocity.x;

        _inputMoveCount++;
        if (_dashDir == dir && _inputMoveCount == 2)
            Dash();
        _dashDir = dir;

        yield return new WaitForSeconds(0.3f);

        _inputMoveCount = 0;
        _dashDir = ' ';
        _rigid.velocity = new Vector2(correntVelocityX,_rigid.velocity.y);
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

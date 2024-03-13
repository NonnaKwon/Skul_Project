using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static Define;

public class PlayerController : MonoBehaviour
{
    //�÷��̾��� ���� : Idle, Attack, Interact, Die, ���� ������(�������ݾƴ�)�̸� �����ϼ�����
    public float XVelocity { get { return _rigid.velocity.x; } }
    [SerializeField] LayerMask _groundFind;

    private float _moveSpeed;
    private float _jumpPower;
    private float _dashPower;
    private int _jumpCount;
    private char _dashDir;
    private int _inputMoveCount;
    private float _damagedPower;


    Vector3 _moveDir;
    Rigidbody2D _rigid;
    Animator _animator;
    SpriteRenderer _renderer;
    Platform _onPlatform;

    PooledObject _jumpEffect;
    PooledObject _dashEffect;

    private StateMachine<PlayerState> stateMachine;
    public StateMachine<PlayerState> StateMachine { get { return stateMachine; } }

    private void Awake()
    {
        Manager.Game.Player = this;
        stateMachine = new StateMachine<PlayerState>();
        stateMachine.AddState(PlayerState.Idle, new IdleState(this));
        stateMachine.AddState(PlayerState.Damaged, new DamagedState(this));
        stateMachine.AddState(PlayerState.Interact, new InteractState(this));
        stateMachine.AddState(PlayerState.Die, new DieState(this));
        stateMachine.Start(PlayerState.Idle);
    }

    private void Start()
    {
        PlayerInit();
        _rigid = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _jumpEffect = Manager.Resource.Load<PooledObject>("Prefabs/Effects/JumpEffect");
        _dashEffect = Manager.Resource.Load<PooledObject>("Prefabs/Effects/DashEffect");
    }

    private void Update()
    {
        stateMachine.Update();
    }


    public void PlayerInit()
    {
        _damagedPower = 3;
        _jumpCount = 0;
        _rigid = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _moveSpeed = 8.4f;
        _jumpPower = 13f;
        _dashPower = 20f;
        GetComponent<FightController>().PlayerInit();
        stateMachine.ChangeState(PlayerState.Idle);
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
        _animator.Play("Dash");
        Vector3 insPosition = transform.position;
        Manager.Pool.GetPool(_dashEffect, insPosition, transform.rotation);
        _rigid.velocity = new Vector2(_moveDir.x * _dashPower, _rigid.velocity.y);
    }

    private void OnJump(InputValue value)
    {
        if (stateMachine.CurState == PlayerState.Idle)
        {
            if (_jumpCount < Define.MAX_JUMP_COUNT)
                Jump();
        }   
    }

    private void OnDown(InputValue value)
    {
        if (stateMachine.CurState == PlayerState.Idle)
        {
            if (_onPlatform != null)
                PlatformDown();
        }
    }

    private void OnMove(InputValue value)
    {
        Vector2 moveDistance = value.Get<Vector2>();
        _moveDir.x = moveDistance.x;
    }

    private void OnDashR(InputValue value)
    {
        if (stateMachine.CurState == PlayerState.Idle)
            StartCoroutine(CoDash('R'));
    }

    private void OnDashL(InputValue value)
    {
        if (stateMachine.CurState == PlayerState.Idle)
            StartCoroutine(CoDash('L'));
    }

    IEnumerator CoDash(char dir)
    {
        float correntVelocityX = _rigid.velocity.x;

        _inputMoveCount++;
        if (_dashDir == dir && _inputMoveCount == 2)
            Dash();
        _dashDir = dir;

        yield return new WaitForSeconds(0.2f);

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

    private class PlayerStateClass : BaseState<PlayerState>
    {
        protected PlayerController owner;
        protected float _jumpPower => owner._jumpPower;
        protected Rigidbody2D _rigid => owner._rigid;
        protected Animator _animator => owner._animator;
        protected Vector3 _moveDir => owner._moveDir;
        protected float _moveSpeed => owner._moveSpeed;
        protected SpriteRenderer _renderer => owner._renderer;

        public PlayerStateClass(PlayerController owner)
        {
            this.owner = owner;
        }
    }

    private class IdleState : PlayerStateClass
    {
        public IdleState(PlayerController owner) : base(owner) { }

        public override void Update()
        {
            Move();
            if (_jumpPower > 0)
                _animator.SetFloat("ySpeed", _rigid.velocity.y);
        }


        private void Move()
        {
            owner.transform.Translate(_moveDir.x * _moveSpeed * Time.deltaTime, 0, 0);
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
    }


    private class DamagedState : PlayerStateClass
    {
        public DamagedState(PlayerController owner) : base(owner) { }

        public override void Enter()
        {
            owner._animator.Play("Damaged");
            if (_renderer.flipX) //�������� ����������
                _rigid.velocity = new Vector2(owner._damagedPower, 0);
            else
                _rigid.velocity = new Vector2(-owner._damagedPower, 1);
        }
        public override void Transition()
        {

        }
    }

    private class InteractState : PlayerStateClass
    {
        public InteractState(PlayerController owner) : base(owner) { }

        public override void Enter()
        {
            owner._animator.SetTrigger("Interact");
        }
        public override void Transition()
        {

        }

    }

    
    private class DieState : PlayerStateClass
    {
        public DieState(PlayerController owner) : base(owner) { }

        public override void Enter()
        {
            Debug.Log("Player : Die");
            _animator.Play("Die");
            UI_GameOver gameoverUI = Resources.Load<UI_GameOver>("Prefabs/UIs/Popup/UI_GameOver");
            Debug.Log(gameoverUI);
            Manager.UI.ShowPopUpUI(gameoverUI,false);
        }

        public override void Transition()
        {

        }

        public override void Exit()
        {
            Manager.UI.ClearPopUpUI();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static Define;

public class PlayerController : MonoBehaviour
{
    //플레이어의 상태 : Idle, Attack, Interact, Die, 만약 공격중(점프공격아님)이면 움직일수없게
    public float XVelocity { get { return _rigid.velocity.x; } }
    [SerializeField] LayerMask _groundFind;

    public Head CurrentHead;
    public bool IsRight { get; set; } //오른쪽으로 가고있나

    private float _moveSpeed;
    private float _jumpPower;
    private float _dashPower;
    private char _dashDir;
    private int _inputMoveCount;
    private int _stackJumpCount;

    Vector3 _moveDir;
    Rigidbody2D _rigid;
    Animator _animator;
    Platform _onPlatform;
    Collider2D _collider;
    SpriteRenderer _renderer;

    PooledObject _jumpEffect;
    PooledObject _dashEffect;

    private StateMachine<PlayerState> stateMachine;
    public StateMachine<PlayerState> StateMachine { get { return stateMachine; } }

    private void Awake()
    {
        CurrentHead = Util.GetOrAddComponent<Skul>(gameObject);
        stateMachine = new StateMachine<PlayerState>();
        stateMachine.AddState(PlayerState.Idle, new IdleState(this));
        stateMachine.AddState(PlayerState.Interact, new InteractState(this));
        stateMachine.AddState(PlayerState.Die, new DieState(this));
        stateMachine.Start(PlayerState.Idle);
    }

    private void Start()
    {
        PlayerInit();
        _rigid = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
        _jumpEffect = Manager.Resource.Load<PooledObject>("Prefabs/Effects/JumpEffect");
        _dashEffect = Manager.Resource.Load<PooledObject>("Prefabs/Effects/DashEffect");
        _renderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        stateMachine.Update();
    }


    public void PlayerInit()
    {
        IsRight = true;
        _stackJumpCount = 0;
        _moveSpeed = 8.4f;
        _jumpPower = 13f;
        _dashPower = 20f;
        GetComponent<FightController>().PlayerInit();
        stateMachine.ChangeState(PlayerState.Idle);
    }

    public void ChangeHead()
    {

    }

    private void Jump()
    {
        _stackJumpCount++;
        if (_stackJumpCount >= CurrentHead.Data.jumpCount)
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
        _collider.isTrigger = true;
        _animator.Play("Dash");
        Vector3 insPosition = transform.position;
        Manager.Pool.GetPool(_dashEffect, insPosition, transform.rotation);
        _rigid.velocity = new Vector2(_moveDir.x * _dashPower, _rigid.velocity.y);
    }

    private void OnJump(InputValue value)
    {
        if (stateMachine.CurState == PlayerState.Idle)
        {
            if (_stackJumpCount < CurrentHead.Data.jumpCount)
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
        _collider.isTrigger = false;

        yield return new WaitForSeconds(0.2f);

        _inputMoveCount = 0;
        _dashDir = ' ';
        _rigid.velocity = new Vector2(correntVelocityX,_rigid.velocity.y);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_groundFind.Contain(collision.gameObject.layer))
        {
            _stackJumpCount = 0;
            _onPlatform = collision.gameObject.GetComponent<Platform>();
            _animator.SetFloat("ySpeed", 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_groundFind.Contain(collision.gameObject.layer))
        {
            _stackJumpCount = 0;
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
                {
                    _renderer.flipX = true;
                    owner.IsRight = false;
                }
                else
                {
                    _renderer.flipX = false;
                    owner.IsRight = true;
                }
            }
            else
                _animator.SetBool("IsWalk", false);
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

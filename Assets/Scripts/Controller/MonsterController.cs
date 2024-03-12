using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Define;

public class MonsterController : MonoBehaviour, IDamagable, IAttackable
{
    [SerializeField] AttackPoint _baseAttackPoint;

    //몬스터 스트립터블 데이터
    private float _maxHp;
    private float _hp;
    private float _power;
    private float _defencePower;
    private float _moveSpeed;
    private int _maxAttackCount;
    private float _traceRange;

    private int _attackCount;
    private float _attackRange;
    private bool _targetFind;

    Animator _animator;
    GameObject _target;
    Rigidbody2D _rigid;
    SpriteRenderer _renderer;
    Vector3 _attackPointPosition;

    private StateMachine<MonsterState> stateMachine;
    public StateMachine<MonsterState> StateMachine { get { return stateMachine; } }


    private void Awake()
    {
        stateMachine = new StateMachine<MonsterState>();
        stateMachine.AddState(MonsterState.Idle, new IdleState(this));
        stateMachine.AddState(MonsterState.Trace, new TraceState(this));
        stateMachine.AddState(MonsterState.Damaged, new DamagedState(this));
        stateMachine.AddState(MonsterState.Attack, new AttackState(this));
        stateMachine.AddState(MonsterState.Die, new DieState(this));
        stateMachine.Start(MonsterState.Idle);
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
        _attackPointPosition = _baseAttackPoint.gameObject.GetComponent<Transform>().localPosition;
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void Move()
    {
        if (_attackCount > 0)
            return;
        Vector2 targetRotation = (_target.transform.position - transform.position).normalized;
        float movePos = 0;
        if (targetRotation.x < 0)
        {
            _renderer.flipX = true;
            movePos = _attackPointPosition.x < 0 ? _attackPointPosition.x : _attackPointPosition.x * -1;
        }
        else
        {
            _renderer.flipX = false;
            movePos = _attackPointPosition.x > 0 ? _attackPointPosition.x : _attackPointPosition.x * -1;
        }
        _attackPointPosition.x = movePos;
        _baseAttackPoint.transform.localPosition = new Vector3(movePos, _attackPointPosition.y, _attackPointPosition.z);
        transform.Translate(new Vector3(targetRotation.x * _moveSpeed * Time.deltaTime, 0, 0), Space.World);
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
        _hp -= damage;
        if (_renderer.flipX) //왼쪽으로 돌아있으면
            _rigid.AddForce(new Vector2(1,1));
        else
            _rigid.AddForce(new Vector2(-1,1));

    }

    private class MonsterStateClass : BaseState<MonsterState>
    {
        protected MonsterController owner;
        public MonsterStateClass(MonsterController owner)
        {
            this.owner = owner;
        }
    }

    private class IdleState : MonsterStateClass
    {
        public IdleState(MonsterController owner) : base(owner) { }

        public override void Update()
        {

        }

        public override void Transition()
        {
            if((Vector2.Distance(owner._target.transform.position, owner.transform.position) < owner._traceRange))
            {
                ChangeState(MonsterState.Trace);
            }
        }

    }


    private class TraceState : MonsterStateClass
    {
        public TraceState(MonsterController owner) : base(owner) { }

        public override void Enter()
        {
            Debug.Log("타겟이 범위에 들어왔다");
            owner._animator.SetBool("IsMove", true);
        }
        public override void Update()
        {
            owner.Move();
        }

        public override void Exit()
        {
            owner._animator.SetBool("IsMove", false);
        }

        public override void Transition()
        {
            if(Vector2.Distance(owner._target.transform.position, owner.transform.position) < owner._attackRange)
            {
                ChangeState(MonsterState.Attack);
            }
        }

    }



    private class DamagedState : MonsterStateClass
    {
        public DamagedState(MonsterController owner) : base(owner) { }

        public override void Update()
        {

        }

    }

    private class AttackState : MonsterStateClass
    {
        public AttackState(MonsterController owner) : base(owner) { }

        public override void Update()
        {
            owner.Attack();
        }

        public override void Transition()
        {
            if (Vector2.Distance(owner._target.transform.position, owner.transform.position) >= owner._attackRange)
            {
                ChangeState(MonsterState.Trace);
            }
        }
    }


    private class DieState : MonsterStateClass
    {
        public DieState(MonsterController owner) : base(owner) { }

        public override void Update()
        {

        }

    }

}

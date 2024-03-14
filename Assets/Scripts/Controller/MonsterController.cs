using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Define;

public class MonsterController : MonoBehaviour, IDamagable, IAttackable
{
    [SerializeField] AttackPoint _baseAttackPoint;
    [SerializeField] Slider _hpSlider;

    [Header("Monster Info")]
    [SerializeField] private float _maxHp = 12;
    [SerializeField] private float _power = 3;
    [SerializeField] private int _maxAttackCount = 1;
    [SerializeField] private float _attackTime = 2;

    //몬스터 스트립터블 데이터
    private float _hp;
    private float _defencePower;
    private float _moveSpeed;
    private int _attackCount;

    private float _attackRange;
    private float _traceRange;

    Animator _animator;
    GameObject _target;
    Rigidbody2D _rigid;
    SpriteRenderer _renderer;
    Vector3 _attackPointPosition;
    PooledObject _damageEffect;

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
        _attackCount = 0;
        _traceRange = 10;
        _attackRange = 3;
        _hp = _maxHp;
        _moveSpeed = 5f;
        _animator = GetComponent<Animator>();
        _rigid = GetComponent<Rigidbody2D>();
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _target = FindObjectOfType<PlayerController>().gameObject;
        _attackPointPosition = _baseAttackPoint.gameObject.GetComponent<Transform>().localPosition;
        _damageEffect = Resources.Load("Prefabs/Effects/AttackEffect").GetComponent<PooledObject>();
        _hpSlider.maxValue = _maxHp;
        _hpSlider.value = _maxHp;
        stateMachine.Start(MonsterState.Idle);
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

    private void Die()
    {
        StartCoroutine(CoDie());
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
        _hpSlider.value -= damage;
        StartCoroutine(CoTakeDamage());
    }

    
    IEnumerator CoTakeDamage()
    {
        stateMachine.ChangeState(MonsterState.Damaged);
        Vector3 randomVec = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0);
        yield return new WaitForSeconds(0.1f);
        Manager.Pool.GetPool(_damageEffect, transform.position + randomVec, transform.rotation);
        
        if (_renderer.flipX) //왼쪽으로 돌아있으면
            _rigid.velocity = new Vector2(DAMAGED_POWER, 0);
        else
            _rigid.velocity = new Vector2(-DAMAGED_POWER, 1);
        yield return new WaitForSeconds(1f);
        stateMachine.ChangeState(MonsterState.Trace);
    }

    IEnumerator CoDie()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
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

            if(owner._hp <= 0)
                ChangeState(MonsterState.Die);
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
            if (owner._hp <= 0)
                ChangeState(MonsterState.Die);
        }

    }



    private class DamagedState : MonsterStateClass
    {
        public DamagedState(MonsterController owner) : base(owner) { }

        public override void Enter()
        {
            owner._animator.Play("Damaged");
        }
        public override void Update()
        {
            if (owner._hp <= 0)
                ChangeState(MonsterState.Die);
        }

    }

    private class AttackState : MonsterStateClass
    {
        private float curAttackTime;
        public AttackState(MonsterController owner) : base(owner) { }

        public override void Enter()
        {
            curAttackTime = 1.5f;
        }
        public override void Update()
        {
            curAttackTime += Time.deltaTime;
            if (curAttackTime < owner._attackTime)
                return;
            if (Manager.Game.Player.StateMachine.CurState != PlayerState.Die)
            {
                curAttackTime = 0;
                owner.Attack();
            }
        }

        public override void Transition()
        {
            if (Vector2.Distance(owner._target.transform.position, owner.transform.position) >= owner._attackRange)
            {
                ChangeState(MonsterState.Trace);
            }
            if (owner._hp <= 0)
                ChangeState(MonsterState.Die);
            if (Manager.Game.Player.StateMachine.CurState == PlayerState.Die)
                ChangeState(MonsterState.Trace);
        }
    }

    
    private class DieState : MonsterStateClass
    {
        public DieState(MonsterController owner) : base(owner) { }

        public override void Enter()
        {
            owner._animator.Play("Die");
            owner.Die();
        }


        public override void Update()
        {

        }

    }

}

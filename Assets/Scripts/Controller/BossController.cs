using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using static Define;

public class BossController : MonoBehaviour, IDamagable, IAttackable
{
    [SerializeField] UI_BossMap _connectUI;
    [SerializeField] float _maxTime;
    [SerializeField] private float _minTIme;

    private int _curPower;
    private float _time;
    private float _maxHp;
    private float _hp;
    private float _skillHp;

    Animator _animator;
    PooledObject _damageEffect;

    private StateMachine<BossState> stateMachine;
    

    private void Awake()
    {
        stateMachine = new StateMachine<BossState>();
        stateMachine.AddState(BossState.Intro, new IntroState(this));
        stateMachine.AddState(BossState.Idle, new IdleState(this));
        stateMachine.AddState(BossState.Pattern, new PatternState(this));
        stateMachine.AddState(BossState.Down, new DownState(this));
        stateMachine.AddState(BossState.Phase, new PhaseState(this));
        stateMachine.AddState(BossState.Die, new DieState(this));
    }

    void Start()
    {
        _maxHp = 100;
        _time = 0;
        _hp = _maxHp;
        _connectUI.InitHPBar(_maxHp);
        _skillHp = 40;

        _damageEffect = Resources.Load("Prefabs/Effects/AttackEffect").GetComponent<PooledObject>();
        _animator = GetComponent<Animator>();
        stateMachine.Start(BossState.Intro);
        Debug.Log("완료");
    }

    private void Update()
    {
        stateMachine.Update();
        if (stateMachine.CurState != BossState.Intro)
        {
            _time += Time.deltaTime;
            _connectUI.TimeUI = _time;
        }
    }

    private void Skill()
    {
        stateMachine.ChangeState(BossState.Pattern);
        if (_hp < _skillHp)
            EnergyBall();
        int random = Random.Range(0, 2);
        switch(random)
        {
            case 0:
                DownHit();
                break;
            case 1:
                SlideHit();
                break;
        }
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("Boss : 데미지를 받았다!");
        _hp -= damage;
        _connectUI.DecreaseHP(damage);
        StartCoroutine(CoTakeDamage());
    }

    IEnumerator CoTakeDamage()
    {
        Vector3 randomVec = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0);
        Manager.Pool.GetPool(_damageEffect, transform.position + randomVec, transform.rotation);
        yield return new WaitForSeconds(0.5f);
    }

    public void Attack()
    {
        

    }


    public float GetPower()
    {
        return _curPower;
    }


    private void DownHit()
    {
        Debug.Log("DownHit Skill()");
        //_animator.Play("DownHit");
        StartCoroutine(CoDown());
    }

    IEnumerator CoDown()
    {
        Vector2 targetPosition = new Vector2(0, 40);

        //Lerp 사용하여 위로 올라갔다가 타겟으로 내려찍어버리기 (찍을때 hitPoint Attack 호출)
        yield return null;
    }

    private void SlideHit()
    {
        Debug.Log("SlideHit Skill()");
        _animator.Play("SliceHit");
        Vector3 dir = (Manager.Game.Player.transform.position - transform.position).normalized;
        _animator.SetBool("IsRight", dir.x > 0);
    }


    private void EnergyBall()
    {
        // 볼 프리팹을 들고와서 여러방향으로 Instantiate 하면됨. 
        // 8방향 볼 쏘기
        // 3번 하고 쓰러짐 -> 상태 Down
        // 플레이어 방향으로 큰 볼 던지기
    }


    private class BossStateClass : BaseState<BossState>
    {
        protected BossController owner;
        public BossStateClass(BossController owner)
        {
            this.owner = owner;
        }
    }

    private class IntroState : BossStateClass
    {
        public IntroState(BossController owner) : base(owner) { }

        public override void Enter()
        {
            Manager.Scene.StoryLoad("tutorial01_4");
        }

        public override void Update()
        {

        }

        public override void Transition()
        {
            if (!Manager.Scene.IsStoryMode)
                owner.stateMachine.ChangeState(BossState.Idle);
        }

    }
    private class IdleState : BossStateClass
    {
        public IdleState(BossController owner) : base(owner) { }
        private float _skillTerm;
        private float _skillTime;

        public override void Enter()
        {
            Debug.Log("help");
            _skillTerm = Random.Range(owner._minTIme, owner._maxTime);
            _skillTime = 0;
        }

        public override void Update()
        {
            _skillTerm += Time.deltaTime;
            if (_skillTime > _skillTerm)
                owner.Skill();
        }

        public override void Transition()
        {

        }

    }


    private class PatternState : BossStateClass
    {
        public PatternState(BossController owner) : base(owner) { }

        public override void Enter()
        {

        }
        public override void Transition()
        {

        }

    }


    private class DownState : BossStateClass
    {
        public DownState(BossController owner) : base(owner) { }
        private float _downTime;
        private float _time;
        public override void Enter()
        {
            _downTime = 5;
            _time = 0;
        }

        public override void Update()
        {
            _downTime += Time.deltaTime;
        }

        public override void Transition()
        {

        }

    }

    private class PhaseState : BossStateClass //hp 배열에 따라서
    {
        public PhaseState(BossController owner) : base(owner) { }

        public override void Enter()
        {

        }
        public override void Transition()
        {

        }

    }


    private class DieState : BossStateClass
    {
        public DieState(BossController owner) : base(owner) { }

        public override void Enter()
        {
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

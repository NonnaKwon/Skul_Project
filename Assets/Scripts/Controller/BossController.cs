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
    [SerializeField] GameObject _rightHand;
    [SerializeField] GameObject _leftHand;
    [SerializeField] Transform _energyBallPos;

    private int _curPower;
    private float _time;
    private float _maxHp;
    private float _hp;
    private float _skillHp;

    Animator _animator;
    PooledObject _damageEffect;
    AttackPointSkill _rightHandCollider;
    AttackPointSkill _leftHandCollider;
    GameObject _energyBallPrefab;
    GameObject _energyMainBallPrefab;

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
        _maxHp = 200;
        _time = 0;
        _hp = _maxHp;
        _connectUI.InitHPBar(_maxHp);
        _skillHp = 80;

        _damageEffect = Resources.Load("Prefabs/Effects/AttackEffect").GetComponent<PooledObject>();
        _rightHandCollider = _rightHand.GetComponentInChildren<AttackPointSkill>();
        _leftHandCollider = _leftHand.GetComponentInChildren<AttackPointSkill>();
        _rightHandCollider.SkillPower = 30;
        _leftHandCollider.SkillPower = 30;


        _rightHandCollider.gameObject.SetActive(false);
        _leftHandCollider.gameObject.SetActive(false);

        _animator = GetComponent<Animator>();
        _energyBallPrefab = Manager.Resource.Load<GameObject>("Prefabs/Effects/EnergyBall");
        _energyMainBallPrefab = Manager.Resource.Load<GameObject>("Prefabs/Effects/MainBall");
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
        if (_hp <= 0)
            stateMachine.ChangeState(BossState.Die);
    }

    private void Skill()
    {
        Debug.Log("Skill");
        stateMachine.ChangeState(BossState.Pattern);
        if (_hp < _skillHp)
        {
            Debug.Log("zz");
            EnergyBall();
        }
        else
        {
            int random = Random.Range(0, 2);
            Debug.Log(random);
            switch (random)
            {
                case 0:
                    _curPower = 50;
                    DownHit();
                    break;
                case 1:
                    SlideHit();
                    break;
            }
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
        _animator.Play("DownHit");
        StartCoroutine(CoDown());
    }

    IEnumerator CoDown()
    {
        Vector2 curPosL = _leftHand.transform.position;
        Vector2 curPosR = _rightHand.transform.position;
        //위로 올리기
        Vector2 targetPosition = new Vector2(5, 10);
        while (_leftHand.transform.position.y <= targetPosition.y - 1)
        {
            Vector2 transPosL = Vector2.Lerp(_leftHand.transform.position, new Vector2(_leftHand.transform.position.x - targetPosition.x, targetPosition.y), Time.deltaTime);
            Vector2 transPosR = Vector2.Lerp(_rightHand.transform.position, new Vector2(_rightHand.transform.position.x + targetPosition.x, targetPosition.y), Time.deltaTime);
            _leftHand.transform.position = transPosL;
            _rightHand.transform.position = transPosR;
            yield return null;
        }

        int moveSpeed = 10;
        targetPosition = Manager.Game.Player.gameObject.transform.position;
        Vector2 dir = (targetPosition - new Vector2(transform.position.x, transform.position.y)).normalized;
        if (dir.x < 0)
        {
            while (_leftHand.transform.position.y >= targetPosition.y+3)
            {
                Vector2 transPosL = Vector2.Lerp(_leftHand.transform.position, targetPosition, Time.deltaTime* moveSpeed);
                _leftHand.transform.position = transPosL;
                yield return null;
            }
            _leftHand.GetComponentInChildren<AttackPoint>().Attack();
        }
        else
        {
            while (_rightHand.transform.position.y >= targetPosition.y+3)
            {
                Vector2 transPosL = Vector2.Lerp(_rightHand.transform.position, targetPosition, Time.deltaTime* moveSpeed);
                _rightHand.transform.position = transPosL;
                yield return null;
            }
            _rightHand.GetComponentInChildren<AttackPoint>().Attack();
        }
        yield return new WaitForSeconds(1f);
        _leftHand.transform.position = curPosL;
        _rightHand.transform.position = curPosR;
        stateMachine.ChangeState(BossState.Idle);
    }

    private void SlideHit()
    {
        Debug.Log("SlideHit Skill()");
        StartCoroutine(CoSlideHit());
    }

    IEnumerator CoSlideHit()
    {
        Vector3 dir = (Manager.Game.Player.transform.position - transform.position).normalized;
        Vector2 curPosL = _leftHand.transform.position;
        Vector2 curPosR = _rightHand.transform.position;
        //위로 올리기
        Vector2 targetPosition = new Vector2(60, 0);
        while (_leftHand.transform.position.x >= - targetPosition.x + 1)
        {
            Vector2 transPosL = Vector2.Lerp(_leftHand.transform.position, new Vector2(_leftHand.transform.position.x - targetPosition.x, _leftHand.transform.position.y), Time.deltaTime);
            Vector2 transPosR = Vector2.Lerp(_rightHand.transform.position, new Vector2(_rightHand.transform.position.x + targetPosition.x, _rightHand.transform.position.y), Time.deltaTime);
            _leftHand.transform.position = transPosL;
            _rightHand.transform.position = transPosR;
            yield return null;
        }

        int moveSpeed = 1;
        targetPosition = new Vector2(60, 0);
        if (dir.x < 0)
        {
            _leftHandCollider.gameObject.SetActive(true);
            while (_leftHand.transform.position.x <= targetPosition.x - 3)
            {
                Vector2 transPosL = Vector2.Lerp(_leftHand.transform.position, new Vector2(_leftHand.transform.position.x + targetPosition.x, _leftHand.transform.position.y), Time.deltaTime * moveSpeed);
                _leftHand.transform.position = transPosL;
                yield return null;
            }
            _leftHandCollider.gameObject.SetActive(false);
        }
        else
        {
            _rightHandCollider.gameObject.SetActive(true);
            while (_rightHand.transform.position.x >= -targetPosition.x + 3)
            {
                Vector2 transPosR = Vector2.Lerp(_rightHand.transform.position, new Vector2(_rightHand.transform.position.x - targetPosition.x, _rightHand.transform.position.y), Time.deltaTime * moveSpeed);
                _rightHand.transform.position = transPosR;
                yield return null;
            }
            _rightHandCollider.gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(1f);
        _leftHand.transform.position = curPosL;
        _rightHand.transform.position = curPosR;
        stateMachine.ChangeState(BossState.Idle);
    }


    private void EnergyBall()
    {
        Debug.Log("EnergyBall Skill()");
        StartCoroutine(CoEnergyBall());
    }

    IEnumerator CoEnergyBall()
    {
        int count = 3;
        int speed = 3;

        _animator.Play("EnergyBall");
        yield return new WaitForSeconds(3f);
        GameObject ball = Instantiate(_energyBallPrefab, _energyBallPos.position, _energyBallPos.rotation);
        GameObject mainBall = Instantiate(_energyMainBallPrefab, _energyBallPos.position, _energyBallPos.rotation);
        while (count-- > 0)
        {
            ball.GetComponent<Animator>().Play("Ball");
            mainBall.transform.position = _energyBallPos.position;
            Vector2 targetDir = Manager.Game.Player.transform.position - mainBall.transform.position;
            while (mainBall.transform.position.y >= -20)
            {
                mainBall.transform.Translate(targetDir * speed * Time.deltaTime);
                yield return null;
            }
            yield return new WaitForSeconds(2f);
        }

        Destroy(ball);
        Destroy(mainBall);
        stateMachine.ChangeState(BossState.Down);
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
            owner._animator.Play("Idle");
            _skillTerm = Random.Range(owner._minTIme, owner._maxTime);
            Debug.Log(_skillTerm);
            _skillTime = 0;
        }

        public override void Update()
        {
            _skillTime += Time.deltaTime;
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
            owner._animator.Play("DownState");
            _downTime = 7;
            _time = 0;
        }

        public override void Update()
        {
            _downTime += Time.deltaTime;
        }

        public override void Transition()
        {
            if (_downTime < _time)
                owner.stateMachine.ChangeState(BossState.Idle);
        }

        public override void Exit()
        {
            owner._animator.Play("Idle");
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

using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;

public class FightController : MonoBehaviour,IDamagable
{
    [SerializeField] Collider2D _baseAttackPoint;
    [SerializeField] int _maxAttackCount;
    [SerializeField] LayerMask _mask;
    Animator _animator;

    //�÷��̾��, �÷��̾� ��Ʈ�ѷ����� �밡���ٲ𶧸��� (�̸�, ��ų, �����Ŀ�) �굵 �ٲ�
    private int _maxHp;
    private int _hp;
    private float _power;
    private float _defencePower;
    private int _attackCount;

    public float Power { get { return _power; } } //��������Ʈ�� ����

    private void Start()
    {
        _attackCount = 0;
        _maxHp = 100;
        _hp = _maxHp;
        _animator = GetComponent<Animator>();
    }


    public void TakeDamage(float damage)
    {

    }

    private void Attack()
    {
        _attackCount++;
        if (_attackCount > _maxAttackCount)
            return;
        _animator.SetInteger("AttackCount", _attackCount);
        StartCoroutine(CoAttack());
    }

    private void OnAttack(InputValue value)
    {
        if(_attackCount < _maxAttackCount)
            Attack();
    }

    private void OnMove(InputValue value)
    {
        
    }

    private IEnumerator CoAttack()
    {
        _baseAttackPoint.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        _baseAttackPoint.gameObject.SetActive(false);
        if(_attackCount > 1)
            yield return new WaitForSeconds(0.3f * _maxAttackCount);
        _attackCount = 0;
        _animator.SetInteger("AttackCount", _attackCount);
    }
}

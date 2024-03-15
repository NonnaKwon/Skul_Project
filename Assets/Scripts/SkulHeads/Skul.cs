using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEditor.Animations;
using UnityEngine;

public class Skul : Head
{
    //캔버스로 쿨타임도 연동
    SkulSkillObject _head;
    SkulSkillObject _headPrefab;
    Animator _playerAnimator;
    AnimatorController _playerAniController;
    AnimatorController _skillAniController;

    private float RegenTime;
    private float curTime = 0;

    private void Start()
    {
        Data = Manager.Resource.Load<HeadData>("Data/Head/Skul");
        _playerAnimator = Manager.Game.Player.gameObject.GetComponent<Animator>();
        _playerAniController = Manager.Resource.Load<AnimatorController>("Animations/Player/Base/PlayerAnimation2");
        _skillAniController = Manager.Resource.Load<AnimatorController>("Animations/Player/SkulSkill/PlayerAnimationSkul");
        _headPrefab = Manager.Resource.Load<SkulSkillObject>("Prefabs/SkulHead");
        RegenTime = Data.coolTimeA;
    }

    private void Update()
    {
        curTime += Time.deltaTime;
        if (_head != null && curTime >= RegenTime)
            InputHead();
    }

    public override void InputHead() //머리 꼈을때
    {
        WearHead(false);
    }

    private void WearHead(bool isAnimation = true)
    {
        Debug.Log("Wear Head");
        //코루틴으로 Interact 상태로 변경
        if(_head != null)
        {
            Destroy(_head.gameObject);
            _head = null;
        }
        _playerAnimator.runtimeAnimatorController = _playerAniController;
        if(isAnimation)
            _playerAnimator.Play("Reborn");
    }

    public override void SkillS()
    {
        if (_head == null)
            return;

        Manager.Game.Player.transform.position = _head.transform.position;
        WearHead();
    }

    public override void SkillA()
    {
        if (_head != null)
            return;
        curTime = 0;
        float dis = 1;
        if (!Manager.Game.Player.IsRight)
            dis *= -4;

        Vector3 insPos = new Vector3(transform.position.x + dis, transform.position.y + 2.3f, transform.position.z );
        _head = Instantiate(_headPrefab, insPos, transform.rotation).GetComponent<SkulSkillObject>();
        _playerAnimator.runtimeAnimatorController = _skillAniController;
    }
}

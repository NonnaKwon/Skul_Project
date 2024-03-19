using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEditor.Animations;
using UnityEngine;

public class Skul : Head
{
    //Äµ¹ö½º·Î ÄðÅ¸ÀÓµµ ¿¬µ¿
    SkulSkillObject _head;
    SkulSkillObject _headPrefab;
    Animator _playerAnimator;
    AnimatorController _playerAniController;
    AnimatorController _skillAniController;

    private float RegenTime;
    private float curTime;

    private void Start()
    {
        Data = Manager.Resource.Load<HeadData>("Data/Head/Skul");
        _playerAnimator = Manager.Game.Player.gameObject.GetComponent<Animator>();
        _playerAniController = Manager.Resource.Load<AnimatorController>("Animations/Player/Base/PlayerAnimation2");
        _skillAniController = Manager.Resource.Load<AnimatorController>("Animations/Player/SkulSkill/PlayerAnimationSkul");
        _headPrefab = Manager.Resource.Load<SkulSkillObject>("Prefabs/SkulHead");
        RegenTime = Data.coolTimeA;
        curTime = RegenTime;
    }

    private void Update()
    {
        curTime += Time.deltaTime;
        if (_head != null && curTime >= RegenTime)
            InputHead();
    }

    public override void InputHead() //¸Ó¸® ²¼À»¶§
    {
        WearHead(false);
    }

    private void WearHead(bool isAnimation = true)
    {
        if(_head != null)
        {
            Destroy(_head.gameObject);
            _head = null;
        }
        _playerAnimator.runtimeAnimatorController = _playerAniController;
        if(isAnimation)
            _playerAnimator.Play("Reborn");
    }

    public override bool SkillS()
    {
        if (_head == null)
            return false;

        Manager.Game.Player.transform.position = _head.transform.position;
        WearHead();
        return true;
    }

    public override bool SkillA()
    {
        if (curTime < RegenTime || _head != null)
            return false;
        curTime = 0;
        float dis = 1;
        if (!Manager.Game.Player.IsRight)
            dis *= -4;

        Vector3 insPos = new Vector3(transform.position.x + dis, transform.position.y + 2.3f, transform.position.z );
        _head = Instantiate(_headPrefab, insPos, transform.rotation).GetComponent<SkulSkillObject>();
        _playerAnimator.runtimeAnimatorController = _skillAniController;
        return true;
    }
}

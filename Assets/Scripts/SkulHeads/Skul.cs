using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skul : Head
{
    //캔버스로 쿨타임도 연동
    SkulSkillObject _head;
    Animator _playerAnimator;

    private float RegenTime = 5;
    private float curTime = 0;
    private void Awake()
    {
        Data = Manager.Resource.Load<HeadData>("Data/Head/Skul");
        _playerAnimator = Manager.Game.Player.GetComponent<Animator>();
        //PooledObject skillHead = Manager.Resource.Load<PooledObject>("Prefabs/SkulHead");
        //Manager.Pool.CreatePool(skillHead, 1, 1);
    }

    private void Update()
    {
        curTime += Time.deltaTime;
        if (_head != null && curTime >= RegenTime)
            LoadHead();
    }
    public override void InputHead() //머리 꼈을때
    {

    }

    private void LoadHead()
    {
        //시간 지나서 머리 낌
        WearHead();
    }

    private void WearHead()
    {
        Destroy(_head);
        _head = null;
    }

    public override void SkillS()
    {
        Manager.Game.Player.gameObject.transform.position = transform.position;
        WearHead();
    }

    public override void SkillA()
    {
        if (_head != null)
            return;
        curTime = 0;
        PooledObject prefab = Manager.Resource.Load<PooledObject>("Prefabs/SkulHead");
        Vector3 insPos = new Vector3(transform.position.x, transform.position.y + 4f, transform.position.z);
        _head = Instantiate(prefab, insPos, transform.rotation).GetComponent<SkulSkillObject>();
        Debug.Log("스킬 A 호출");
    }
}

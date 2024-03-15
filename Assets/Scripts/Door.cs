using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour
{
    [SerializeField] Transform LoadPos;
    [SerializeField] List<MonsterController> monsterList;
    [SerializeField] bool _isEnter = false;
    [SerializeField] private bool _isActive = false;
    
    Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        if (monsterList.Count == 0)
            DoorActive();
        Debug.Log(_animator);
    }

    private void Update()
    {
        if (_isActive)
            return;

        //���� ���Ͱ� �� �׾�����(���¸ӽ�), ��Ƽ�갡 Ȱ��ȭ��
        for(int i=0;i<monsterList.Count; i++)
        {
            if (monsterList[i] == null)
                monsterList.RemoveAt(i);
        }

        if (monsterList.Count == 0 && !_isActive)
            DoorActive();
    }

    private void DoorActive()
    {
        _isActive = true;
        Debug.Log(_animator);
        _animator.Play("Active");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag.Equals("Player"))
        {
            _isEnter = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
            _isEnter = false;
    }


    private void OnEnter(InputValue value)
    {
        if(_isEnter && _isActive)
        {
            if (LoadPos == null)
                Manager.Scene.LoadScene(Define.Scene.BossScene);
            Manager.Scene.LoadNextStory(LoadPos);
        }
    }
}

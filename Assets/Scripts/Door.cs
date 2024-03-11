using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour
{
    [SerializeField] Transform LoadPos;
    private bool _isEnter = false;
    private bool _isActive = false;
    private MonsterController[] monsterList;

    private void Start()
    {
        monsterList = FindObjectsByType<MonsterController>(FindObjectsSortMode.None);
        if (monsterList.Length == 0)
            _isActive = true;
    }

    private void Update()
    { 
        //만약 몬스터가 다 죽었으면(상태머신), 엑티브가 활성화됨
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _isEnter = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _isEnter = false;
    }

    private void OnEnter(InputValue value)
    {
        if(_isEnter)
        {
            Manager.Scene.LoadNextStory(LoadPos);
        }
    }
}

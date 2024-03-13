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

    private void Start()
    {
        monsterList = FindObjectsByType<MonsterController>(FindObjectsSortMode.None).ToList();
        if (monsterList.Count == 0)
            _isActive = true;
    }

    private void Update()
    {
        if (_isActive)
            return;

        //만약 몬스터가 다 죽었으면(상태머신), 엑티브가 활성화됨
        for(int i=0;i<monsterList.Count; i++)
        {
            if (monsterList[i] == null)
                monsterList.RemoveAt(i);
        }

        if (monsterList.Count == 0)
            _isActive = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag.Equals("Player"))
            _isEnter = true;
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
            Manager.Scene.LoadNextStory(LoadPos);
        }
    }
}

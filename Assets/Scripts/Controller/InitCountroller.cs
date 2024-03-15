using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCounter : MonoBehaviour
{
    [SerializeField] Door _connectDoor;

    UI_GameScene _ui;
    bool _isActive;

    private void Start()
    {
        _ui = Manager.Scene.GetCurScene<GameScene>().UI_GameScene;
        Debug.Log(_ui);
        _isActive = false;
    }

    private void LateUpdate()
    {
        if (_isActive)
            _ui.MonsterCountUpdate(_connectDoor.MonsterCount);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            _ui.MonsterCountUpdate(_connectDoor.MonsterCount);
            _isActive = true;
        }
    }
}

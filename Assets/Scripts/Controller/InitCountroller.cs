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
        _isActive = false;
    }

    private void LateUpdate()
    {
        if (!_isActive)
            return;
        _ui.MonsterCountUpdate(_connectDoor.MonsterCount);

        if (_connectDoor.OnDoorActive)
            _isActive = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            Manager.Scene.RegenPos = transform;
            _ui.MonsterCountUpdate(_connectDoor.MonsterCount);
            _isActive = true;
        }
    }
}

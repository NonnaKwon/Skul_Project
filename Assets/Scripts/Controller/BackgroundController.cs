using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;
using UnityEngine.InputSystem;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera _cam;
    public Vector3 PlayerPos { get { return _player.transform.position; } }
    PlayerController _player;

    private void Start()
    {
        _player = FindAnyObjectByType<PlayerController>();
    }

    private void LateUpdate()
    {
        Vector3 movePos = new Vector3(_cam.transform.position.x, transform.position.y, transform.position.z);
        transform.position = movePos;
    }
}

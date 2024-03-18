using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera _cam;
    public float[] MoveSpeeds; //배경 개수+1 에 맞게 넣어야함.
    public Transform[] Layers;


    List<SpriteRenderer[]> _backgrounds;
    BackgroundController _controller;

    float spriteSize = 30f;
    float leftPosX = 0f;
    float leftPatchPos = 0;
    float rightPosX = 0f;
    float rightPatchPos = 0;
    float _xMove;

    private void Start()
    {
        leftPosX = -(spriteSize * 1.5f);
        rightPosX = spriteSize * 1.5f;

        _backgrounds = new List<SpriteRenderer[]>();
        for (int i = 0; i < Layers.Length; i++)
        {
            SpriteRenderer[] backgroundList = Layers[i].GetComponentsInChildren<SpriteRenderer>();
            _backgrounds.Add(backgroundList);
        }
        leftPatchPos = -spriteSize * _backgrounds[0].Length;
        rightPatchPos = spriteSize * _backgrounds[0].Length;

        _controller = GetComponent<BackgroundController>();
    }

    private void LateUpdate()
    {
        //플레이어 좌표 + 로 이동해야함, 비교토 player좌표 + 로 해야함.
        for (int i = 0; i < _backgrounds.Count; i++)
        {
            for (int j = 0; j < _backgrounds[i].Length; j++)
            {
                float playerPosX = _controller.PlayerPos.x;
                Transform spritePosition = _backgrounds[i][j].gameObject.transform.transform;
                int dir = 0;
                if (_xMove < 0)
                    dir = -1;
                else if(_xMove > 0)
                    dir = 1;
                spritePosition.Translate(Vector3.right * dir * MoveSpeeds[i] * Time.deltaTime);

                if (spritePosition.position.x < playerPosX + leftPosX)
                {
                    Vector3 nextPos = spritePosition.position;
                    nextPos = new Vector3(nextPos.x + rightPatchPos, nextPos.y, nextPos.z);
                    spritePosition.position = nextPos;
                }else if(spritePosition.position.x > playerPosX + rightPosX)
                {
                    Vector3 nextPos = spritePosition.position;
                    nextPos = new Vector3(nextPos.x + leftPatchPos, nextPos.y, nextPos.z);
                    spritePosition.position = nextPos;
                }
            }
        }
    }

    private void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        _xMove = input.x;
    }
}

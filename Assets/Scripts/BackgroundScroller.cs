using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BackgroundScroller : MonoBehaviour
{
    public float[] MoveSpeeds; //배경 개수+1 에 맞게 넣어야함.
    public Transform[] Layers;

    List<SpriteRenderer[]> _backgrounds;

    float leftPosX = 0f;
    float leftPatchPos = 0;
    float rightPosX = 0f;
    float rightPatchPos = 0;
    float xScreenHalfSize;
    float yScreenHalfSize;
    float _xMove;

    private void Start()
    {
        yScreenHalfSize = Camera.main.orthographicSize;
        xScreenHalfSize = yScreenHalfSize * Camera.main.aspect;

        leftPosX = -(xScreenHalfSize * 2);
        rightPosX = xScreenHalfSize * 2;

        _backgrounds = new List<SpriteRenderer[]>();
        for (int i = 0; i < Layers.Length; i++)
        {
            SpriteRenderer[] backgroundList = Layers[i].GetComponentsInChildren<SpriteRenderer>();
            _backgrounds.Add(backgroundList);
        }
        rightPatchPos = rightPosX * _backgrounds[0].Length;
        leftPatchPos = leftPosX * _backgrounds[0].Length;
    }

    private void Update()
    {
        //플레이어 좌표 + 로 이동해야함, 비교토 player좌표 + 로 해야함.
        for (int i = 0; i < _backgrounds.Count; i++)
        {
            for (int j = 0; j < _backgrounds[i].Length; j++)
            {
                Transform spritePosition = _backgrounds[i][j].gameObject.transform.transform;
                spritePosition.Translate(Vector3.right * _xMove * MoveSpeeds[i] * Time.deltaTime);

                if (spritePosition.position.x < leftPosX)
                {
                    Vector3 nextPos = spritePosition.position;
                    nextPos = new Vector3(nextPos.x + rightPatchPos, nextPos.y, nextPos.z);
                    spritePosition.position = nextPos;
                }else if(spritePosition.position.x > rightPosX)
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

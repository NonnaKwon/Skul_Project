using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Platform : MonoBehaviour
{
    TilemapCollider2D _collider;

    private void Start()
    {
        _collider = GetComponent<TilemapCollider2D>();
    }

    public void TurnOnTrigger()
    {
        StartCoroutine(CoOffTrigger());
    }

    IEnumerator CoOffTrigger()
    {
        _collider.isTrigger = true;
        yield return new WaitForSeconds(0.5f);
        _collider.isTrigger = false;
    }
}

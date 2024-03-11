using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkPoint : MonoBehaviour
{
    [SerializeField] string id;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag.Equals("Player"))
        {
            Manager.Scene.StoryLoad(id);
            gameObject.SetActive(false);
        }
    }
}

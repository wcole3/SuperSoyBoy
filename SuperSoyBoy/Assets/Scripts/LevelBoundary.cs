using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBoundary : MonoBehaviour {
    //destroy anything that leaves the level

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //if the player has left the level
            GameManager.instance.RestartLevel(0f);
        }
        else
        {
            //destroy the gameobject, it might be a rock or something
            Destroy(collision.gameObject);
        }
    }
}

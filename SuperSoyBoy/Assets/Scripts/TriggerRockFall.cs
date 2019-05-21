using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerRockFall : Hazard {
    //check if the rock can crush the player
    private bool CanCrush;

    private void Start()
    {
        CanCrush = true;
    }

    public void TriggerRock()
    {
        transform.parent = null;
        GetComponent<Rigidbody2D>().isKinematic = false;
        CanCrush = true;
    }

    private new void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 0 && CanCrush)
        {
            //if the rock has landed
            CanCrush = false;
            //put it in default layer
            gameObject.layer = 0;
        }
        if (CanCrush)
        {
            base.OnCollisionEnter2D(collision);
        }

    }
}

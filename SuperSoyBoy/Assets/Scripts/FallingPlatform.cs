using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour {
    private float timeOnPlatform;
    private float timeTillDrop = 1f;
    private bool OnPlatform = false;
    private bool Dropped = false;
    
	
	// Update is called once per frame
	void Update () {
        if (OnPlatform)
        {
            timeOnPlatform += Time.deltaTime;
        }
        if(timeOnPlatform >= timeTillDrop && !Dropped)
        {
            GetComponent<Rigidbody2D>().isKinematic = false;
            Dropped = true;
        }

	}
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !OnPlatform && 
            collision.gameObject.GetComponent<PlayerController>().PlayerIsOnGround())
        {
            //the player landed on the platform, start countdown
            timeOnPlatform = 0f;
            OnPlatform = true;
        }
    }
}

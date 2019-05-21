using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallRockHazard : MonoBehaviour {
    //a rock to crush the player
    private bool IsTriggered = false;
	
	// Update is called once per frame
	void Update () {
        if (IsTriggered)
        {
            GetComponentInChildren<TriggerRockFall>().TriggerRock();//detach rock and let it fall
            Destroy(gameObject);

        }
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            IsTriggered = true;
        }
    }
}

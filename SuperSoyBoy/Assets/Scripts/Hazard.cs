using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour {
    //control course hazards
    public GameObject playerDeathPrefab;
    public AudioClip deathClip;//soyboy's death rattle
    public Sprite hitSprite;//change the sprite when soyboy is hit

    private SpriteRenderer sr;

	// Use this for initialization
	void Awake () {
        sr = GetComponent<SpriteRenderer>();
	}

    //test if the player has collided with hazard
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            var audioSource = GetComponent<AudioSource>();
            if(audioSource != null && deathClip != null)
            {
                audioSource.PlayOneShot(deathClip);
            }
            //get player death prefab and the hitsprite
            Instantiate(playerDeathPrefab, collision.GetContact(0).point, Quaternion.identity);
            sr.sprite = hitSprite;
            Destroy(collision.gameObject);//destroy player
            GameManager.instance.RestartLevel(2f);//restart game in 4 seconds
        }
    }
}

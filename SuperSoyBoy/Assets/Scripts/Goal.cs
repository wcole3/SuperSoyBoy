using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {
    //the goal of a level
    public AudioClip goalClip;
    //the noodlesplash effect
    public GameObject splashParticles;
    public AudioClip splashSound;

    //whether the player has finished
    private bool IsFinished = false;
    private Vector2 jumpPoint;//the location Soyboy will start their celebration
    private GameObject soyboy;

    private void Start()
    {
        IsFinished = false;
        jumpPoint = new Vector2(transform.position.x, transform.position.y + 1f);
        soyboy = GameObject.Find("SoyBoy");
    }

    private void Update()
    {
        if ((soyboy != null) && IsFinished && (soyboy.transform.position.y < transform.position.y))
        {
            GetComponent<AudioSource>().PlayOneShot(splashSound);
            Destroy(soyboy);
            GameObject splash = (GameObject)Instantiate(splashParticles, transform.position, Quaternion.identity);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player" && !IsFinished)
        {
            var audioSource = GetComponent<AudioSource>();
            if(!IsFinished && audioSource != null && goalClip != null)
            {
                audioSource.PlayOneShot(goalClip);
            }
            //save the time
            Timer timer = FindObjectOfType<Timer>();
            GameManager.instance.SaveTime(timer.time);
            //freeze player
            collision.rigidbody.velocity = Vector2.zero;
            DoBackflip(collision.gameObject);
            collision.gameObject.GetComponent<PlayerController>().enabled = false;//turn off control
            timer.enabled = false;//stop timer for player
            //restart the level
            GameManager.instance.RestartLevel(2f);
        }
    }
    //make player do a backflip
    private void DoBackflip(GameObject player)
    {
        player.transform.position = jumpPoint;
        player.layer = 9;//make sure the player can go through the goal
        player.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 10f);
        player.GetComponent<Animator>().SetTrigger("DoFlip");//start spinning
        //set the finished flag
        IsFinished = true;
    }

}

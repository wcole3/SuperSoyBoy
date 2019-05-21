using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//make sure we have the need comps
[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(Animator))]
public class PlayerController : MonoBehaviour {
    //controller for player character

    //player variables
    public float speed = 14f;
    public float accel = 6f;
    public float airAccel = 3f;
    public bool isJumping;
    public float jumpSpeed = 8f;
    public float jumpDurationLimit = 0.25f;//the limit to upwards accel
    public float jump = 14f;
    //sound effect
    public AudioClip runClip;
    public AudioClip jumpClip;
    public AudioClip slideClip;

    //private
    private Vector2 input;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private AudioSource audioSource;
    private Animator animator;
    private float EPSILON = 0.001f;
    //raycasting variables
    private float rayCastCheckLength = 0.005f;
    private float width;//player width
    private float height;//player height
    private float jumpDuration;//length of current jump


    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        //get collider width and height
        width = GetComponent<Collider2D>().bounds.extents.x + 0.1f;//NOTE due to extent call this is half the true width
        height = GetComponent<Collider2D>().bounds.extents.y + 0.2f;//add a little buffer
    }

    //control player movement
    private void Update()
    {
        //get input direction
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Jump"));
        animator.SetFloat("Speed", Mathf.Abs(input.x));
        //set player facing direction
        if (input.x > 0f)
        {
            sr.flipX = false;
        }
        else if(input.x < 0f)
        {
            sr.flipX = true;
        }
        //check for jump
        if(PlayerIsOnGround() && !isJumping)
        {
            if(input.y > 0f)
            {
                isJumping = true;
                audioSource.PlayOneShot(jumpClip);
            }
            animator.SetBool("IsOnWall", false);
            if(Mathf.Abs(input.x) > 0f)
            {
                PLayAudioClip(runClip);
            }
        }
        if(jumpDuration >= jumpDurationLimit)
        {
            input.y = 0f;//turn off jump
        }
        //time the jump
        if(input.y > 0f)
        {
            jumpDuration += Time.deltaTime;
            animator.SetBool("IsJumping", true);
        }
        else
        {
            jumpDuration = 0f;
            isJumping = false;
            animator.SetBool("IsJumping", false);
        }

    }

    //change motion on fixed update
    private void FixedUpdate()
    {
        var acceleration = 0f;
        //get whether the player is on ground or not
        if (PlayerIsOnGround())
        {
            acceleration = accel;
        }
        else
        {
            acceleration = airAccel;
        }
        var xVelocity = 0f;
        if((Mathf.Abs(input.x) <= EPSILON) && PlayerIsOnGround())
        {
            xVelocity = 0f;//only stop player on ground
        }
        else
        {
            xVelocity = rb.velocity.x;//set velocity to rigidbody velocity
        }
        //update rigidbody velocity
        var yVelocity = 0f;
        if(PlayerIsTouchingGroundOrWall() && input.y > 0f)
        {
            yVelocity = jump;
        }
        else
        {
            yVelocity = rb.velocity.y;
        }
        rb.AddForce(new Vector2(((input.x * speed) - rb.velocity.x) * acceleration, 0));

        rb.velocity = new Vector2(xVelocity, yVelocity);

        //check for wall jump
        if (IsWallOnLeftOrRight() && !PlayerIsOnGround() && input.y > 0f)
        {
            rb.velocity = new Vector2(GetWallDirection() * 0.75f * speed, rb.velocity.y);
            animator.SetBool("IsOnWall", false);
            animator.SetBool("IsJumping", true);
            audioSource.PlayOneShot(jumpClip);//play jump over other sounds
        }
        //check if onwall
        else if (IsWallOnLeftOrRight() && !PlayerIsOnGround())
        {
            animator.SetBool("IsOnWall", true);
            PLayAudioClip(slideClip);
        }
        if (!IsWallOnLeftOrRight())
        {
            animator.SetBool("IsOnWall", false);
        }

        if (isJumping && jumpDuration < jumpDurationLimit)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);//this continues to move the character up after inital jump
        }
    }

    //method to check if player is on ground
    public bool PlayerIsOnGround()
    {
        //going to check three points

        bool groundCheck1 = Physics2D.Raycast(new Vector2(transform.position.x + (width - 0.2f), transform.position.y - height),
                Vector2.down, rayCastCheckLength, LayerMask.GetMask("Default"));//right
        bool groundCheck2 = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - height), 
                Vector2.down, rayCastCheckLength, LayerMask.GetMask("Default"));//middle
        bool groundCheck3 = Physics2D.Raycast(new Vector2(transform.position.x - (width - 0.2f), transform.position.y - height), 
                Vector2.down, rayCastCheckLength, LayerMask.GetMask("Default"));//left
        if(groundCheck1 || groundCheck2 || groundCheck3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //check for walls to the left or right of player
    public bool IsWallOnLeftOrRight()
    {
        //cast once to left and once to right
        bool leftCheck = Physics2D.Raycast(new Vector2(transform.position.x - width, transform.position.y), 
                Vector2.left, rayCastCheckLength, LayerMask.GetMask("Default"));
        bool rightCheck = Physics2D.Raycast(new Vector2(transform.position.x + width, transform.position.y), 
                Vector2.right, rayCastCheckLength, LayerMask.GetMask("Default"));
        //only care if there is a wall
        if(leftCheck || rightCheck)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //figure out which side the wall is on
    public int GetWallDirection()
    {
        //cast once to left and once to right
        bool leftCheck = Physics2D.Raycast(new Vector2(transform.position.x - width, transform.position.y),
                Vector2.left, rayCastCheckLength, LayerMask.GetMask("Default"));
        bool rightCheck = Physics2D.Raycast(new Vector2(transform.position.x + width, transform.position.y),
                Vector2.right, rayCastCheckLength, LayerMask.GetMask("Default"));
        if (leftCheck)
        {
            return 1;//return the direction away from the wall
        }
        else if (rightCheck)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
    //now want to know if player is touching wall or ground
    public bool PlayerIsTouchingGroundOrWall()
    {
        if(PlayerIsOnGround() || IsWallOnLeftOrRight())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void PLayAudioClip(AudioClip clip)
    {
        if(audioSource != null && clip != null)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(clip);
            }
        }
    }
}

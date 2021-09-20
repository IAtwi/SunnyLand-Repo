using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : Character
{

    [SerializeField] private float jumpVelocity = 5f;


    private Vector3 leftGapPos;
    private Vector3 rightGapPos;


    private float directionForce = -1;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        leftGapPos = transform.GetChild(0).transform.position;
        rightGapPos = transform.GetChild(1).transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if(facingRight)
        {
            directionForce = 1;
            spriteRenderer.flipX = true;
        }
    }

    private void Update()
    {
        animator.SetFloat("yVelocity", rigidBody.velocity.y);
        CheckDirection();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(StaticInfo.groundTag))
        {
            animator.Play("idle");
        }
    }

    public void Jump()
    {
        Vector2 direction = new Vector2(directionForce, 2);
        direction.Normalize();
        rigidBody.velocity = direction * jumpVelocity;
    }

    private void CheckDirection()
    {
        if ((transform.position.x < leftGapPos.x && directionForce < 0) || (transform.position.x > rightGapPos.x && directionForce > 0))
        {
            directionForce *= -1;
            rigidBody.velocity = new Vector2(rigidBody.velocity.x * -1, rigidBody.velocity.y);
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
    }


}

using UnityEngine;


public class Opossum : Character
{
    [SerializeField] private float velocity = 5f;

    private Vector3 leftGapPosition;
    private Vector3 rightGapPosition;


    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        leftGapPosition = transform.GetChild(0).transform.position;
        rightGapPosition = transform.GetChild(1).transform.position;
    }

    private void Start()
    {
        if (!facingRight)
        {
            velocity *= -1;
            spriteRenderer.flipX = false;
        }
    }

    private void Update()
    {
        CheckDirection();
    }

    private void FixedUpdate()
    {
        move();
    }

    private void move()
    {
        rigidBody.velocity = new Vector2(velocity, rigidBody.velocity.y);
    }

    private void CheckDirection()
    {
        if ( (transform.position.x < leftGapPosition.x && velocity <0) || (transform.position.x > rightGapPosition.x && velocity > 0) )
        {
            velocity *= -1;
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
    }

}

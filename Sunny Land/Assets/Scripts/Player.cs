using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Player : Character
{
	private enum State { idle,run, crouch, jump,fall,hurt};
	private State playerState = State.idle;

	//[SerializeField] private float m_JumpForce = 400f;                        // Amount of force added when the player jumps.
	[SerializeField] private float jumpVelocity = 6f;
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
	[SerializeField] private bool m_AirControl = true;                          // Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching
	


	private float velocityAddedWhenDamaged = 7f;
	private float timeForHurt = 0.65f;
	private bool isTakingDamage = false;
	private float horizontalMovement = 0f;
    private bool wantToJump= false;
    private bool jump = false;
    private bool crouch = false;
	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private bool m_wasCrouching = false;
	private Vector3 m_Velocity = Vector3.zero;

	[Header("Events")]
	[Space]
	public UnityEvent OnLandEvent;
	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }
	public BoolEvent OnCrouchEvent;


    
    private void Awake()
    {
		rigidBody = GetComponent<Rigidbody2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();


		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();

	}

    private void Update()
    {
		GetInputs();
		SetPlayerState();
		PlayAnimation();
	}

    private void FixedUpdate()
    {
        Move();

		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded)
					OnLandEvent.Invoke();
			}
		}
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
		if (collision.gameObject.CompareTag(StaticInfo.enemyTag) && !isTakingDamage)
		{
			StartCoroutine(TookDamage(collision.transform.position));
		}
	}

    private void OnCollisionStay2D(Collision2D collision)
    {
		if (collision.gameObject.CompareTag(StaticInfo.enemyTag) && !isTakingDamage)
		{
			StartCoroutine(TookDamage(collision.transform.position));
		}
	}

    private IEnumerator TookDamage(Vector3 enemyPos)
    {
		isTakingDamage = true;
		Vector2 forceDirection = transform.position - enemyPos;
		forceDirection.Normalize();
		rigidBody.velocity = forceDirection * velocityAddedWhenDamaged;
		playerState = State.hurt;
		yield return new WaitForSeconds(timeForHurt);
		isTakingDamage = false;
	}

    private void GetInputs()
    {
		horizontalMovement = Input.GetAxisRaw("Horizontal");
		wantToJump = Input.GetButtonDown("Jump");
		if (wantToJump)
			jump = true;
		crouch = Input.GetButton("Crouch");
	}

	private void SetPlayerState()
	{
		if (isTakingDamage)
		{
			playerState = State.hurt;
		}
		else if (rigidBody.velocity.y > 0.1)
		{
			playerState = State.jump;
		}
		else if (rigidBody.velocity.y < -0.1)
		{
			playerState = State.fall;
		}
		else if (crouch)
		{
			playerState = State.crouch;
		}
		else if (Mathf.Abs(rigidBody.velocity.x) > 0.1)
		{
			playerState = State.run;
		}
		else
		{
			playerState = State.idle;
		}
	}

	private void PlayAnimation()
    {
		switch (playerState)
		{
			case State.idle: { animator.Play("idle"); break; }
			case State.run: { animator.Play("run"); break; }
			case State.crouch: { animator.Play("crouch"); break; }
			case State.jump: { animator.Play("jump"); break; }
			case State.fall: { animator.Play("fall"); break; }
			case State.hurt: { animator.Play("hurt"); break; }
		}
    }

	private void Move()
	{
		// If crouching, check to see if the character can stand up
		if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			// If crouching
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				horizontalMovement *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			}
			else
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(horizontalMovement * 10f, rigidBody.velocity.y);
			// And then smoothing it out and applying it to the character
			rigidBody.velocity = Vector3.SmoothDamp(rigidBody.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (horizontalMovement > 0 && !facingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (horizontalMovement < 0 && facingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if (m_Grounded && jump)
		{
			// Add a vertical force to the player.
			m_Grounded = false;
			rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpVelocity);
			//rigidBody.AddForce(new Vector2(0f, m_JumpForce));
			jump = false;
		}
	}

	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		facingRight = !facingRight;

		spriteRenderer.flipX = !spriteRenderer.flipX;
	}

}
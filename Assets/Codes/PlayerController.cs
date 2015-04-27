using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	int jumpPower = 4;


	Animator animator;
	bool isDead = false;


	GameSceneEvents eventHandler = null;
	GameManager gameMgr = null;

	bool isJumping = false;
	bool isFalling = false;
	bool isHolding = true;

	Rigidbody2D MyRigidBody;
	void Start () 
	{
		animator = GetComponent<Animator>();

		eventHandler = GameObject.Find("eventHandler").GetComponent<GameSceneEvents>();

		gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();

		MyRigidBody = GetComponent<Rigidbody2D> ();
	}



	void UpdateAnimator()
	{
		animator.SetBool("IsOnGround", true);
		animator.SetBool("IsDead", isDead);
	}

	void Die()
	{
		isDead = true;
		eventHandler.onPlayerDead ();

		MyRigidBody.velocity = Vector3.zero;
	}

	void Update () 
	{
		bool ButtonJumpDown, ButtonJumpHold, ButtonJumpUp;

		UpdateAnimator();

		if (isDead)
		{
			return;
		}




		ButtonJumpDown = false;
		ButtonJumpHold = false;
		ButtonJumpUp = false;


		#if UNITY_STANDALONE || UNITY_WEBPLAYER


		// jump check
		if ( Input.GetButton ("Jump") )
		{
			ButtonJumpHold = true;
		}
		else
		{
			ButtonJumpHold = false;
		}

		if ( Input.GetButtonDown ("Jump") )
		{
			ButtonJumpDown = true;
		}
		else
		{
			ButtonJumpDown = false;
		}

		if ( Input.GetButtonUp ("Jump") )
		{
			ButtonJumpUp = true;
		}
		else
		{
			ButtonJumpUp = false;
		}

	

		#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

		if (Input.touchCount > 0) 
		{

			for (int i = 0; i < Input.touchCount; ++i)
			{
				Touch touch = Input.GetTouch(i);

				if (true)//(touch.position.x >= Screen.width / 2)
				{
					// jump
					if (touch.phase == TouchPhase.Began)
					{
						ButtonJumpDown = true;
					}
					else
					{
						ButtonJumpDown = false;
					}
					
					if (touch.phase == TouchPhase.Ended)
					{
						ButtonJumpUp = true;
					}
					else
					{
						ButtonJumpUp = false;
					}
					
					if (touch.phase == TouchPhase.Moved)
					{
						ButtonJumpHold = true;
					}
					else
					{
						ButtonJumpHold = false;
					}
				}
				 
			}
		}
		
		#endif

		HandleInput (ButtonJumpDown, ButtonJumpHold, ButtonJumpUp);

	}

	

	void HandleInput(bool bButtonJumpDown, bool bButtonJumpHold, bool bButtonJumpUp)
	{
		if (bButtonJumpDown && isHolding) {
			isJumping = true; isHolding = false;
			isFalling = false;
		
			MyRigidBody.AddForce(new Vector3(0,jumpPower * 100f,0));
			MyRigidBody.gravityScale = 1;

		}

		if (bButtonJumpHold && isJumping) {

		}

		if (bButtonJumpUp) {
			isJumping = false;
			isFalling = true;
		
		}

		if (isFalling && checkBars ()) {
			isHolding = true;
			MyRigidBody.gravityScale = 0;
			MyRigidBody.velocity = Vector3.zero;
		}

		if (!isAlive ())
			Die ();

	}

	bool checkBars ()
	{
		return true;
	}

	bool isAlive()
	{
		return gameMgr.MainCam.transform.position.y - gameObject.transform.position.y < gameMgr.MainCam.GetComponent<Camera>().orthographicSize;
	}

}

using UnityEngine;
using System.Collections;


public enum EObjectColor
{
	RED = 0,
	GREEN = 1,
	BLUE = 2,
	YELLOW =3,
	MAXCOLORNUM
};

public class PlayerController : MonoBehaviour {

	int jumpPower = 4;
	int maxJumpCount = 1;
	int currentJumpCount = 0;

	Animator animator;
	bool isDead = false;


	GameSceneEvents eventHandler = null;
	GameManager gameMgr = null;
	Rigidbody2D MyRigidBody;
	SpriteRenderer spriteRenderer;

	public bool jumped {get; private set;}


	EObjectColor currentColor = EObjectColor.RED;


	void Start () 
	{
		animator = GetComponent<Animator>();

		eventHandler = GameObject.Find("eventHandler").GetComponent<GameSceneEvents>();

		gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();

		MyRigidBody = GetComponent<Rigidbody2D> ();

		spriteRenderer = GetComponent<SpriteRenderer>();

		jumped = false;

		currentColor = (EObjectColor)Random.Range (0,(int)EObjectColor.MAXCOLORNUM);
		ChangeColor ();

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

		gameMgr.EndGame ();
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

		if( Input.GetButtonDown("Fire1"))
			ChangeColor();
	

		#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

		if (Input.touchCount > 0) 
		{

			for (int i = 0; i < Input.touchCount; ++i)
			{
				Touch touch = Input.GetTouch(i);

				if (touch.position.x >= Screen.width / 2)
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
				else
				{
					if (touch.phase == TouchPhase.Began)
						ChangeColor();
				}
				 
			}
		}
		
		#endif

		HandleInput (ButtonJumpDown, ButtonJumpHold, ButtonJumpUp);

		
		if (!isAlive ())
			Die ();
	}

	void ChangeColor()
	{
		currentColor = currentColor + 1;
		if (currentColor == EObjectColor.MAXCOLORNUM)
			currentColor = 0;

		Utils.addLog("Current Color: " + currentColor.ToString());

		gameObject.layer = 10 + (int)currentColor;
		spriteRenderer.color = getColorBuyColorEnum (currentColor);
	}

	Color getColorBuyColorEnum(EObjectColor oc)
	{
		Color rtColor = new Color();
		switch (oc) {
		case EObjectColor.RED:
			rtColor = new Color(1,0,0,1);
			break;
		case EObjectColor.BLUE:
			rtColor = new Color(0,0,1,1);
			break;
		case EObjectColor.GREEN:
			rtColor = new Color(0,1,0,1);
			break;
		case EObjectColor.YELLOW:
			rtColor = new Color(1,1,0,1);
			break;
		}

		return rtColor;
	}
	

	void HandleInput(bool bButtonJumpDown, bool bButtonJumpHold, bool bButtonJumpUp)
	{
		if (bButtonJumpDown && maxJumpCount > currentJumpCount) {
			jumped = true;
			currentJumpCount += 1;

			MyRigidBody.AddForce(new Vector3(0,jumpPower * 100f,0));
			MyRigidBody.gravityScale = 1;

		}

		if (bButtonJumpHold) {

		}

		if (bButtonJumpUp && jumped) {
			jumped = false;
			TryHold();
		}



	}


	bool isAlive()
	{
		return gameMgr.MainCam.transform.position.y - gameObject.transform.position.y < gameMgr.MainCam.GetComponent<Camera>().orthographicSize && !CheckHead();
	}

	bool CheckHead()
	{
		float halfPlayerSizeY = gameObject.GetComponent<BoxCollider2D> ().size.y/2 * gameObject.transform.localScale.y;
		Vector2 myPos = new Vector2 (gameObject.transform.position.x, gameObject.transform.position.y + halfPlayerSizeY);

		RaycastHit2D hitup = Physics2D.Raycast (myPos, Vector2.up, 0.1f,~(1 <<  (gameObject.layer)));
		if (hitup.collider != null )
			return true;//knock into bar
		return false;
	}

	void TryHold()
	{ 
		Vector2 myPos = new Vector2 (gameObject.transform.position.x, gameObject.transform.position.y);
		float halfBarSizeY = gameMgr.barGen.barTemmplate.GetComponent<BoxCollider2D> ().size.y/2 * gameMgr.barGen.barTemmplate.transform.localScale.y;
		RaycastHit2D hitup = Physics2D.Raycast (myPos, Vector2.up, halfBarSizeY, 1 << LayerMask.NameToLayer("Level"));
		if (hitup.collider != null)
			Hold (hitup.collider.gameObject.GetComponent<BarController> ());
		else {
			RaycastHit2D hitdown = Physics2D.Raycast (myPos, -Vector2.up, halfBarSizeY, 1 << LayerMask.NameToLayer("Level"));
			if (hitdown.collider != null)
				Hold (hitdown.collider.gameObject.GetComponent<BarController> ());
		}
	}

	void Hold(BarController bc)
	{
		if(bc)
			bc.onPlayerHold ();

		MyRigidBody.gravityScale = 0;
		MyRigidBody.velocity = Vector3.zero;
		currentJumpCount = 0;
	}

	public void ResetJumpCount(int num)
	{
		if(!isDead)
			currentJumpCount = Mathf.Min (Mathf.Max (0, num), maxJumpCount);
	}



}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	int airCombo = 0;
	bool isDead = false;
	bool allowInput = true;

	GameSceneEvents eventHandler = null;
	GameManager gameMgr = null;
	Rigidbody2D MyRigidBody;
	SpriteRenderer spriteRenderer;

	public bool jumped {get; private set;}


	EObjectColor currentColor = EObjectColor.RED;
	AudioSource audioSource;
	public List<AudioClip> audioClips = new List<AudioClip>();
	GameObject fullScreenFlashImage;
	void Start () 
	{
		eventHandler = GameObject.Find("eventHandler").GetComponent<GameSceneEvents>();

		gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();

		MyRigidBody = GetComponent<Rigidbody2D> ();

		spriteRenderer = GetComponent<SpriteRenderer>();

		audioSource = GetComponent<AudioSource> ();

		fullScreenFlashImage = GameObject.Find ("FullScreenFlashImage");

		jumped = false;

		currentColor = (EObjectColor)Random.Range (0,(int)EObjectColor.MAXCOLORNUM);
		ChangeColor ();

	}


	void Die()
	{
		playSound (audioClips [3]);
		allowInput = false;
		isDead = true;
		eventHandler.onPlayerDead ();

		MyRigidBody.velocity = Vector3.zero;

		gameMgr.EndGame ();
	}

	void Update () 
	{
		bool ButtonJumpDown, ButtonJumpHold, ButtonJumpUp;

		if (!isAlive () && !isDead)
			Die ();

		if (isDead || !allowInput)
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

		if(Input.GetButtonDown("Fire1"))
		{
			playSound(audioClips[2]);
			ChangeColor();
		}
	

		#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

		if (Input.touchCount > 0) 
		{

			for (int i = 0; i < Input.touchCount; ++i)
			{
				Touch touch = Input.GetTouch(i);

				if (touch.position.x <= Screen.width / 2)
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

		


		GameObject bar = HeadKnocked ();
		if (bar != null) {
			playSound(audioClips[1]);
			fullScreenFlashImage.GetComponent<Animator>().Play("fading",0);
			MyRigidBody.velocity = -MyRigidBody.velocity * 0.8f;
			ChangeColor ((int)bar.GetComponent<BarController> ().getColor ());

			if( gameMgr.AddLife(-1) <= 0)
			{
				allowInput = false;
				gameObject.layer = LayerMask.NameToLayer("NoCollision");
			}
		}
	}

	void ChangeColor(int newColor = -1)
	{
		if (newColor < 0) {
			currentColor = currentColor + 1;
			if (currentColor == EObjectColor.MAXCOLORNUM)
				currentColor = 0;
		} else
			currentColor = (EObjectColor)newColor;

			gameObject.layer = 10 + (int)currentColor;
			spriteRenderer.color = getColorBuyColorEnum (currentColor);

		if (jumped)
			airCombo += 1;
		else
			airCombo = 0;
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
			playSound(audioClips[0]);

		}
		/*
		if (bButtonJumpHold) {

		}*/

		if (bButtonJumpUp && maxJumpCount > currentJumpCount) {

		}



	}


	bool isAlive()
	{
		return gameMgr.MainCam.gameObject.transform.position.y - gameObject.transform.position.y < gameMgr.MainCam.orthographicSize;
	}

	GameObject HeadKnocked()
	{
		float halfPlayerSizeY = gameObject.GetComponent<BoxCollider2D> ().size.y/2 * gameObject.transform.localScale.y;
		Vector2 myPos = new Vector2 (gameObject.transform.position.x, gameObject.transform.position.y + halfPlayerSizeY);
		Debug.DrawRay (myPos, myPos + Vector2.up * 0.1f,new Color(1,0,0,1));
		RaycastHit2D hitup = Physics2D.Raycast (myPos, Vector2.up, 0.1f,~(1 <<  (gameObject.layer)));
		if (hitup.collider != null )
			return hitup.collider.gameObject;//knock into bar
		return null;
	}

	GameObject FootTouched()
	{
		float halfPlayerSizeY = gameObject.GetComponent<BoxCollider2D> ().size.y/2 * gameObject.transform.localScale.y;
		Vector2 myPos = new Vector2 (gameObject.transform.position.x, gameObject.transform.position.y + halfPlayerSizeY);
		
		RaycastHit2D hitup = Physics2D.Raycast (myPos, -Vector2.up, 0.1f,~(1 <<  (gameObject.layer)));
		if (hitup.collider != null )
			return hitup.collider.gameObject;//knock into bar
		return null;
	}


	public void ResetJumpCount(int num, BarController barController)
	{
		if (!isDead) {
			currentJumpCount = Mathf.Min (Mathf.Max (0, num), maxJumpCount);
			jumped = false;
			airCombo = 0;
		}
	}

	public void AddScore(int s)
	{
		gameMgr.AddScore (s + s * (Mathf.Max(0,airCombo - 1)));
	}

	public void playSound(AudioClip ac, bool loop = false)
	{
		audioSource.clip = ac;
		audioSource.loop = loop;
		audioSource.Play();
	}


}

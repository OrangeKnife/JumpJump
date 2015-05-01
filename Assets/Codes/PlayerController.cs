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

	public int jumpPower = 4;
	public int maxJumpCount = 1;
	int currentJumpCount = 0;
	int airCombo = 0;
	bool isDead = false;
	bool allowInput = true;

	GameSceneEvents eventHandler = null;
	GameManager gameMgr = null;
	Rigidbody2D MyRigidBody;
	SpriteRenderer spriteRenderer;

	public bool jumped {get; private set;}
	public Vector3 popUpTextOffset;
	public GUIStyle popUpComboGUIStyle;
	public GUIStyle popUpLifeGUIStyle;
	public GUIStyle popUpScoreGUIStyle;

	EObjectColor currentColor = EObjectColor.RED;
	AudioSource audioSource;
	public List<AudioClip> audioClips = new List<AudioClip>();
	GameObject fullScreenFlashImage;

	static Color RedColor = new Color(234f/255f,46f/255f,73f/255f);
	static Color BlueColor = new Color(119f/255f,196f/255f,211f/255f);
	static Color GreenColor = new Color(102f/255f,196f/255f,50f/255f);
	static Color YellowColor = new Color(246f/255f,247f/255f,74f/255f);

	List<string> popUpText = new List<string>();
	List<Vector3> popUpScreenPos = new List<Vector3>();
	List<float> popUpShowedTime = new List<float>();
	List<GUIStyle> popUpGUIStyle = new List<GUIStyle>();

	int tempPlayerScore = 0;
	public int scoreToLife;
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
			ChangeColor(-1,true);
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
					if (touch.phase == TouchPhase.Ended)
					{
						playSound(audioClips[2]);
						ChangeColor(-1,true);
					}
				}
				 
			}
		}
		
		#endif

		HandleInput (ButtonJumpDown, ButtonJumpHold, ButtonJumpUp);

		


		GameObject bar = HeadKnocked ();
		if (bar != null) {
			playSound(audioClips[1]);
			fullScreenFlashImage.GetComponent<Animator>().Play("fading",0);
			MyRigidBody.velocity = Vector3.zero;// -MyRigidBody.velocity * 0.1f;
			ChangeColor ((int)bar.GetComponent<BarController> ().getColor ());

			popUpText.Add("LIFE - 1");
			popUpScreenPos.Add( gameMgr.MainCam.WorldToScreenPoint(gameObject.transform.position + new Vector3(popUpTextOffset.x , popUpTextOffset.y * popUpScreenPos.Count,0)));
			popUpShowedTime.Add(Time.time);
			popUpGUIStyle.Add(popUpLifeGUIStyle);

			if( gameMgr.AddLife(-1) <= 0)
			{
				allowInput = false;
				gameObject.layer = LayerMask.NameToLayer("NoCollision");
			}
		}
	}

	void ChangeColor(int newColor = -1, bool playerChangeColor = false)
	{
		if (newColor < 0) {
			currentColor = currentColor + 1;
			if (currentColor == EObjectColor.MAXCOLORNUM)
				currentColor = 0;
		} else
			currentColor = (EObjectColor)newColor;

			gameObject.layer = 10 + (int)currentColor;
			spriteRenderer.color = getColorBuyColorEnum (currentColor);

		if (jumped && playerChangeColor) {
			airCombo += 1;
			if(airCombo >= 2)
			{
				if(airCombo > 2)
					CleanUpOtherComboText();

				popUpText.Add("COMBO X "+airCombo.ToString());
				popUpScreenPos.Add( gameMgr.MainCam.WorldToScreenPoint(gameObject.transform.position + new Vector3(popUpTextOffset.x , popUpTextOffset.y * popUpScreenPos.Count,0)));
				popUpShowedTime.Add(Time.time);
				popUpGUIStyle.Add(popUpComboGUIStyle);

			}
		}
		else
			airCombo = 0;
	}

	void CleanUpOtherComboText()
	{
		for(int i = 0; i < popUpText.Count; ++i)
		{ 
			if(popUpText[i][0] == 'C')//AIR COMBO
			{
				popUpText.RemoveAt(i);
				popUpScreenPos.RemoveAt(i);
				popUpGUIStyle.RemoveAt(i);
				popUpShowedTime.RemoveAt(i);
				
				i--;
			}
		}
	}

	Color getColorBuyColorEnum(EObjectColor oc)
	{
		Color rtColor = new Color();
		switch (oc) {
		case EObjectColor.RED:
			rtColor = RedColor;
			break;
		case EObjectColor.BLUE:
			rtColor = BlueColor;
			break;
		case EObjectColor.GREEN:
			rtColor = GreenColor;
			break;
		case EObjectColor.YELLOW:
			rtColor = YellowColor;
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
		Debug.DrawLine (myPos, myPos + Vector2.up * 0.15f,new Color(1,0,0,1));
		RaycastHit2D hitup = Physics2D.Raycast(myPos, Vector2.up, 0.1f,~(1 <<  (gameObject.layer)));
		if (hitup.collider != null && MyRigidBody.velocity.y > 0 )
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
		int realScore = s > 0? s + s * (Mathf.Max(0,airCombo - 1)) : s;
		gameMgr.AddScore (realScore);

		tempPlayerScore += realScore;
	 
		 
		popUpText.Add ("SCORE "+ (realScore>0?"+ " : "") + realScore.ToString ());
		popUpScreenPos.Add (gameMgr.MainCam.WorldToScreenPoint (gameObject.transform.position + getPopUpOffSetByString("S")));
		popUpShowedTime.Add (Time.time);
		popUpGUIStyle.Add (popUpScoreGUIStyle);
		 
		if (tempPlayerScore >= scoreToLife) {
			tempPlayerScore -= scoreToLife;
			gameMgr.AddLife (1);

			popUpText.Add ("LIFE X 1");
			popUpScreenPos.Add (gameMgr.MainCam.WorldToScreenPoint (gameObject.transform.position + getPopUpOffSetByString("L")));
			popUpShowedTime.Add (Time.time);
			popUpGUIStyle.Add (popUpLifeGUIStyle);
		}
	}

	public void playSound(AudioClip ac, bool loop = false)
	{
		audioSource.clip = ac;
		audioSource.loop = loop;
		audioSource.Play();
	}

	void OnGUI() {


		if (popUpText.Count > 0) {
			for(int i = 0; i < popUpText.Count; i++)
			{
				GUI.TextField(new Rect (popUpScreenPos[i].x, Screen.height - popUpScreenPos[i].y, 255, 20), popUpText[i],popUpGUIStyle[i]);
			}
		}

		if (popUpShowedTime.Count > 0) {
			for(int i = 0; i < popUpShowedTime.Count; ++i)
			{
				float DisplayTime = getPopUpDisplayTimeByString(popUpText[i]);
				if(Time.time - popUpShowedTime[i] > DisplayTime)
				{
					popUpText.RemoveAt(i);
					popUpScreenPos.RemoveAt(i);
					popUpGUIStyle.RemoveAt(i);
					popUpShowedTime.RemoveAt(i);

					i--;
				}
			}
		}
	}

	float getPopUpDisplayTimeByString(string s)
	{
		if (s [0] == 'L')//LIFE - 1
			return 1f;
		else if (s [0] == 'C')//AIR COMBO
			return 0.5f;
		else if (s [0] == 'S')
			return 0.5f;

		return 0.5f;
	}

	Vector3 getPopUpOffSetByString(string s)
	{
		if (s [0] == 'L')//LIFE - 1
			return new Vector3 (popUpTextOffset.x, popUpTextOffset.y * popUpScreenPos.Count, 0);
		else if (s [0] == 'C')//AIR COMBO
			return new Vector3 (popUpTextOffset.x, popUpTextOffset.y * popUpScreenPos.Count, 0);
		else if (s [0] == 'S')
			return new Vector3 (popUpTextOffset.x, popUpTextOffset.y * 0 , 0);
		
		return new Vector3 (popUpTextOffset.x, popUpTextOffset.y * popUpScreenPos.Count, 0);
	}

}

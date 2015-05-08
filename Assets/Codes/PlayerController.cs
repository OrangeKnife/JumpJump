using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EObjectColor
{
	RED = 0,
	GREEN = 1,
	BLUE = 2,
	YELLOW = 3,
	MAXCOLORNUM = 4,
	BLACK = 5	//this is for colliding all other colors
};

public class PlayerController : MonoBehaviour {

	public float jumpPower = 4;
	public int maxJumpCount = 1;
	int currentJumpCount = 1;//0;
	int combo = 0;//color combo
	public bool wantColorCombo = false;
	int jumpCombo = 0;
	public bool wantJumpCombo = true;
	bool isDead = false;
	bool allowInput = true;
	bool allowInput_jump = true;
	bool allowInput_color = true;

	GameSceneEvents eventHandler = null;
	GameManager gameMgr = null;
	Rigidbody2D MyRigidBody;
	SpriteRenderer spriteRenderer;

	public bool jumped {get; private set;}
	public Vector3 popUpComboTextOffset;
	public Vector3 popUpLifeTextOffset;
	public Vector3 popUpScoreTextOffset;
	public GUIStyle popUpComboGUIStyle;
	public GUIStyle popUpLifeGUIStyle;
	public GUIStyle popUpScoreGUIStyle;

	EObjectColor currentColor = EObjectColor.RED;
	List<AudioSource> audioSourceList;
	public List<AudioClip> audioClips = new List<AudioClip>();
	GameObject fullScreenFlashImage;

	static Color RedColor = new Color(234f/255f,46f/255f,73f/255f);
	static Color BlueColor = new Color(119f/255f,196f/255f,211f/255f);
	static Color GreenColor = new Color(102f/255f,196f/255f,50f/255f);
	static Color YellowColor = new Color(246f/255f,247f/255f,74f/255f);
	static Color BlackColor = new Color(0f,0f,0f);

	List<string> popUpText = new List<string>();
	List<Vector3> popUpScreenPos = new List<Vector3>();
	List<float> popUpShowedTime = new List<float>();
	List<GUIStyle> popUpGUIStyle = new List<GUIStyle>();

	int tempPlayerScore = 0;
	public int scoreToLife;
	public bool wantScoreToLife;
	bool bTimeSlowed;
	float slowTimeRecoverySpeed;
	int slowtimeAudioSourceIdx = -1;
	BarController lastBarStandOn = null;
	float lastStandTime = 0f;
	public float jumpComboThreshold;

	int maxBarNum = 0;
	public int minimumBarCountForHidingColorIndicatoin;
	void Awake()
	{
		
		audioSourceList = new List<AudioSource>();

		audioSourceList.Add( gameObject.AddComponent<AudioSource>() );
		audioSourceList [audioSourceList.Count - 1].volume = 0.2f;

	}
	void Start () 
	{
		eventHandler = GameObject.Find("eventHandler").GetComponent<GameSceneEvents>();

		gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();

		MyRigidBody = GetComponent<Rigidbody2D> ();

		spriteRenderer = GetComponent<SpriteRenderer>();


		fullScreenFlashImage = GameObject.Find ("FullScreenFlashImage");

		jumped = false;

		currentColor = (EObjectColor)Random.Range (0,(int)EObjectColor.MAXCOLORNUM);
		ChangeColor ();

	}


	void Die()
	{
		allowInput = false;
		gameObject.layer = LayerMask.NameToLayer("NoCollision");
		isDead = true;
		MyRigidBody.gravityScale = 0;
		MyRigidBody.velocity = Vector3.zero;
		spriteRenderer.enabled = false;

		if (gameMgr.currentLife < 1) {
			eventHandler.onPlayerDead ();
			gameMgr.EndGame ();

		} else if (gameMgr.AddLife (-1) >= 1) {
			allowInput = true;
			allowInput_jump = false;
			Invoke ("revive", 1f);
		} else {
			playSound (audioClips [3]);//die
			eventHandler.onPlayerDead ();
			gameMgr.EndGame ();
		}

	}

	void revive()
	{
		StartCoroutine ("DoRevive");
	}

	IEnumerator DoRevive()
	{
		gameMgr.barGen.ActiveAllSpawnedBars ();

		if (lastBarStandOn != null)
			gameObject.transform.position = lastBarStandOn.gameObject.transform.position + gameMgr.barGen.barHeight * 0.5f;
		else
			gameObject.transform.position = Vector3.zero;

		gameObject.layer = 10 + (int)currentColor;
		spriteRenderer.enabled = true;
		allowInput_jump = true;
		isDead = false;
		yield return new WaitForSeconds(1f);
		



 
		MyRigidBody.gravityScale = 1;

	}

	void Update () 
	{
		bool ButtonJumpDown, ButtonJumpHold, ButtonJumpUp;

		if (gameMgr.bGamePaused)
			return;
		//gameMgr.changeCameraBGColor(gameObject.transform.position.y);

		if (bTimeSlowed) {
			if (Time.timeScale < 1f)
				Time.timeScale += Time.deltaTime * slowTimeRecoverySpeed;
			else {
				Time.timeScale = 1;
				slowTimeFinished();
			}
		}

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
		if(allowInput_jump)
		{
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
		}

		if(allowInput_color)
		{
			if(Input.GetButtonDown("Fire1"))
			{
				playSound(audioClips[2]);
				ChangeColor(-1,true);
			}
		}
	

		#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

		if (Input.touchCount > 0 ) 
		{

			for (int i = 0; i < Input.touchCount; ++i)
			{
				Touch touch = Input.GetTouch(i);
				if(touch.position.y < Screen.height *0.8f)
				{
					if (touch.position.x <= Screen.width / 2 )
					{
						// jump
						if(allowInput_jump)
						{
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
					else
					{
						if (allowInput_color && touch.phase == TouchPhase.Began)
						{
							playSound(audioClips[2]);
							ChangeColor(-1,true);
						}
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
			EObjectColor barC = bar.GetComponent<BarController> ().getColor ();
			if(barC < EObjectColor.MAXCOLORNUM)
				ChangeColor ((int)barC);

			AddPopup("LIFE - 1", gameMgr.MainCam.WorldToScreenPoint(gameObject.transform.position + new Vector3(popUpLifeTextOffset.x , popUpLifeTextOffset.y * popUpScreenPos.Count,0)), Time.time, popUpLifeGUIStyle);


			if( gameMgr.AddLife(-1) < 1)
			{
				allowInput = false;
				gameObject.layer = LayerMask.NameToLayer("NoCollision");
				GetComponent<Animator>().Play("Spin");
				playSound (audioClips [3]);//die
			}
			else
			{
				ResetJumpCount (0, null);
			}
		}
	}

	public void AddPopup(string message, Vector3 loc, float popupTime, GUIStyle popupStyle)
	{
		popUpText.Add(message);
		popUpScreenPos.Add( loc);
		popUpShowedTime.Add(popupTime);
		popUpGUIStyle.Add(popupStyle);
	}

	void ChangeColor(int newColor = -1, bool playerChangeColor = false)
	{
		//Color lastColor  = getColorBuyColorEnum (currentColor);
		if (newColor < 0) {
			currentColor = currentColor + 1;
			if (currentColor == EObjectColor.MAXCOLORNUM)
				currentColor = 0;
		} else
			currentColor = (EObjectColor)newColor;

			gameObject.layer = 10 + (int)currentColor;
			spriteRenderer.color = getColorBuyColorEnum (currentColor);


		eventHandler.onPlayerColorChanged(currentColor);

		if (wantColorCombo && jumped && playerChangeColor) {
			combo += 1;
			if(combo >= 2)
			{
				if(combo > 2)
					CleanUpOtherComboText();

				AddPopup("COMBO x "+combo.ToString(), gameMgr.MainCam.WorldToScreenPoint(gameObject.transform.position + new Vector3(popUpComboTextOffset.x , popUpComboTextOffset.y * popUpScreenPos.Count,0)), Time.time, popUpComboGUIStyle);
			}
		}
		else
			combo = 0;

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

	public static Color getColorBuyColorEnum(EObjectColor oc)
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

	public void setJumpCombo(int i)
	{
		jumpCombo = i;
	}

	void checkJumpCombo(BarController lastBarStandOn)
	{
		if (Time.time - lastStandTime < jumpComboThreshold && !lastBarStandOn.isJumpedComboed) {
 			lastBarStandOn.isJumpedComboed = true;
			jumpCombo += 1;
			if(jumpCombo >= 2)
				AddPopup("COMBO x "+jumpCombo.ToString(), gameMgr.MainCam.WorldToScreenPoint(gameObject.transform.position + new Vector3(popUpComboTextOffset.x , popUpComboTextOffset.y * popUpScreenPos.Count,0)), Time.time, popUpComboGUIStyle);

		}
		else
			jumpCombo = 0;

	}

	void HandleInput(bool bButtonJumpDown, bool bButtonJumpHold, bool bButtonJumpUp)
	{
		if (bButtonJumpDown && maxJumpCount > currentJumpCount) {

			jumped = true;
			currentJumpCount += 1;
			MyRigidBody.velocity = Vector3.Min(Vector3.zero, Vector3.Max(new Vector3(0,-2f,0),MyRigidBody.velocity));
			MyRigidBody.AddForce(new Vector3(0,jumpPower * 100f,0));
			playSound(audioClips[0]); //jjump
			if(lastBarStandOn != null)
			{
				lastBarStandOn.onPlayerJumped();
				if(wantJumpCombo)
				{
					checkJumpCombo(lastBarStandOn);
				}

			}

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

	public float getHalfPlayerSizeY()
	{
		return gameObject.GetComponent<BoxCollider2D> ().size.y/2 * gameObject.transform.localScale.y;
	}


	GameObject HeadKnocked()
	{
		float halfPlayerSizeY = getHalfPlayerSizeY ();
		Vector2 myPos = new Vector2 (gameObject.transform.position.x, gameObject.transform.position.y + halfPlayerSizeY);
		Debug.DrawLine (myPos, myPos + Vector2.up * 0.15f,new Color(1,0,0,1));

		LayerMask lmask = (~(1 << (gameObject.layer))) - (1 <<  LayerMask.NameToLayer("NoCollision")) - (1 <<LayerMask.NameToLayer("Pickup"));

		RaycastHit2D hitup = Physics2D.Raycast(myPos, Vector2.up, 0.1f,lmask  );
		if (hitup.collider != null && MyRigidBody.velocity.y > 0 )
			return hitup.collider.gameObject;//knock into bar
		return null;
	}

	GameObject FootTouched()
	{
		float halfPlayerSizeY = getHalfPlayerSizeY ();
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
			combo = 0;
			if(barController != null)
			{
				lastBarStandOn = barController;
				lastStandTime = Time.time;
				maxBarNum = Mathf.Max(maxBarNum, lastBarStandOn.barNum);

				if(maxBarNum > minimumBarCountForHidingColorIndicatoin)
					gameMgr.SetColorIndication(false);// I am good enough to do this
			}
			else
			{
				jumpCombo = 0;
			}
		}


	}

	public void AddScore(int s)
	{
		int realScore = s > 0? s + s * (Mathf.Max(0,combo - 1)) + s * (Mathf.Max(0,jumpCombo - 1))  : s  ;
 		gameMgr.AddScore (realScore);

		tempPlayerScore += realScore;
	 
		string jumpComboString = jumpCombo >= 2 ? " x " + jumpCombo.ToString () : "";
		AddPopup ((realScore>0?"+ " : "") + s.ToString ()+ jumpComboString, gameMgr.MainCam.WorldToScreenPoint (gameObject.transform.position + getPopUpOffSetByString("S")), Time.time, popUpScoreGUIStyle);
				 
		if (wantScoreToLife && tempPlayerScore >= scoreToLife) {
			tempPlayerScore -= scoreToLife;
			gameMgr.AddLife (1);

			AddPopup (scoreToLife.ToString() + "SCORE:"+"LIFE + 1", gameMgr.MainCam.WorldToScreenPoint (gameObject.transform.position + getPopUpOffSetByString("L")), Time.time, popUpLifeGUIStyle);

		}
	}

	public void playSound( AudioClip ac, int AudioSourceIdx = 0, bool loop = false)
	{
		audioSourceList[AudioSourceIdx].clip = ac;
		audioSourceList[AudioSourceIdx].loop = loop;
		audioSourceList[AudioSourceIdx].Play();
	}

	public void stopSound(int AudioSourceIdx = 0)
	{
		audioSourceList [AudioSourceIdx].Stop ();
	}

	void OnGUI() {


		if (popUpText.Count > 0) {
			for(int i = 0; i < popUpText.Count; i++)
			{
				GUI.Label(new Rect (popUpScreenPos[i].x, Screen.height - popUpScreenPos[i].y, 255, 20), popUpText[i],popUpGUIStyle[i]);
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
			return 1.5f;
		else if (s [0] == 'C')//AIR COMBO
			return 0.5f;
		else if (s [0] == 'S')
			return 0.5f;

		return 1f;
	}

	Vector3 getPopUpOffSetByString(string s)
	{
		if (s [0] == 'L')//LIFE - 1
			return new Vector3 (popUpLifeTextOffset.x, popUpLifeTextOffset.y * popUpScreenPos.Count, 0);
		else if (s [0] == 'C')//AIR COMBO
			return new Vector3 (popUpComboTextOffset.x, popUpComboTextOffset.y * popUpScreenPos.Count, 0);
		else if (s [0] == 'S')
			return new Vector3 (popUpScoreTextOffset.x, popUpScoreTextOffset.y , 0);
		
		return new Vector3 (popUpScoreTextOffset.x, popUpScoreTextOffset.y * popUpScreenPos.Count, 0);
	}

	void slowTimeFinished()
	{
		bTimeSlowed = false;
		stopSound (slowtimeAudioSourceIdx);
		Destroy (audioSourceList [slowtimeAudioSourceIdx]);
		slowtimeAudioSourceIdx = -1;
	}

	void slowTimeStarted(float timescale, float recoverySpeed, AudioClip slowTimeLoopSound)
	{
		Time.timeScale = timescale;
		bTimeSlowed = true;
		slowTimeRecoverySpeed = recoverySpeed;

		if (slowtimeAudioSourceIdx == -1) {
			audioSourceList.Add (gameObject.AddComponent<AudioSource> ());
			slowtimeAudioSourceIdx = audioSourceList.Count - 1;
		}
		playSound (slowTimeLoopSound, slowtimeAudioSourceIdx, true);
	}

	public bool Pickup(Pickup something)
	{
		if (something.gainLife)
			gameMgr.AddLife (something.gainLifeNum);

		if (something.AddJump)
			maxJumpCount += something.AddJumpNum;

		if (something.slowTime) {
			slowTimeStarted(something.timescale, something.slowTimeRecoverySpeed, something.slowTimeLoopSound);
		}

		AddPopup(something.popupMessage,  gameMgr.MainCam.WorldToScreenPoint (gameObject.transform.position + something.popUpTextOffset), Time.time, something.popupStyle);


		return true;
	}
}

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
	public bool allowInput = true;
	bool allowInput_jump = true;
	bool allowInput_color = true;

	GameSceneEvents eventHandler = null;
	GameManager gameMgr = null;
	Rigidbody2D MyRigidBody;
	SpriteRenderer spriteRenderer,skinSpriteRenderer;



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

	public static Color RedColor = new Color(198f/255f,92f/255f,145f/255f);
	public static Color BlueColor = new Color(112f/255f,123f/255f,190f/255f);
	public static Color GreenColor = new Color(164f/255f,200f/255f,91f/255f);
	public static Color YellowColor = new Color(243f/255f,215f/255f,57f/255f);
	public static Color BlackColor = new Color(0f,0f,0f);

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
	BarController lastBarFallThrough = null;
	float lastStandTime = 0f;
	public float jumpComboThreshold;

	public int maxBarNum { get; private set; }
	public int minimumBarCountForHidingColorIndicatoin;

	public int maximumUnityAdsCanWatch = 1;
	int currentUnityAdsWatched = 0;
	bool deathSoundPlayed = false;

	float playerStartPlayTime;//track how long player survive
	Animator myAnimator,mySkinAnimator;
	public int totalJumpCount {get; private set;}
	public float defaultGravity;

	GameObject skinObject;
	PlayerSkin currentSkin = null;


	void Awake()
	{
		
		audioSourceList = new List<AudioSource>();

		audioSourceList.Add( gameObject.AddComponent<AudioSource>() );
		audioSourceList.Add( gameObject.AddComponent<AudioSource>() );

	}
	void Start () 
	{
		maxBarNum = 0;

		eventHandler = GameObject.Find("eventHandler").GetComponent<GameSceneEvents>();

		gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();

		MyRigidBody = GetComponent<Rigidbody2D> ();
		MyRigidBody.gravityScale = defaultGravity;

		spriteRenderer = GetComponent<SpriteRenderer>();

		myAnimator = GetComponent<Animator> ();

		fullScreenFlashImage = GameObject.Find ("FullScreenFlashImage");

		jumped = false;

		currentColor = (EObjectColor)Random.Range (0,(int)EObjectColor.MAXCOLORNUM);
		ChangeColor ();

		playerStartPlayTime = Time.time;
		totalJumpCount = 0;

		eventHandler.SetFloorText("FLOOR.0");
		eventHandler.SetJumpCountText ("JUMP "+getJumpXstring());

	}

	void AskUnityAdsQuestion()
	{
		StopRecording ();
		CleanUpAllPopup ();

		if (currentUnityAdsWatched < maximumUnityAdsCanWatch && eventHandler.IsUnityAdsReady()
		    && (Time.time - playerStartPlayTime > 20f && gameMgr.currentScore > 5 || gameMgr.currentScore > 25) )
		{
			MyRigidBody.velocity = Vector2.zero;
			MyRigidBody.gravityScale = 0;
			currentUnityAdsWatched++;

			eventHandler.onAdsQuestionPopup ();
		} else {
			DoDeath();

		}
	}

	public void DoDeath()
	{
		eventHandler.onPlayerDead ();
		gameMgr.EndGame ();
		gameMgr.AddDeathCount (1);
	}

	
	public void AfterWatchAds()
	{
		gameMgr.AddLife (gameMgr.getPlayerLifeByMode(gameMgr.gameMode));
		allowInput = true;
		allowInput_jump = false;
		Invoke ("revive", 1f);
	}

	public void AttachSkin(GameObject skinTemp)
	{
		if (skinObject != null)
			GameObject.Destroy (skinObject);

		if (skinTemp != null) {
			skinObject = GameObject.Instantiate (skinTemp);
			skinObject.transform.SetParent (gameObject.transform, false);
			currentSkin = skinObject.GetComponent<PlayerSkin> ();
			skinSpriteRenderer = skinObject.GetComponent<SpriteRenderer> ();
			mySkinAnimator = skinObject.GetComponent<Animator> ();
		}
	 	 
	}

	void Die()
	{
		allowInput = false;
		gameObject.layer = LayerMask.NameToLayer("NoCollision");
		isDead = true;
		MyRigidBody.gravityScale = 0;
		MyRigidBody.velocity = Vector2.zero;
		spriteRenderer.enabled = false;
		if(skinSpriteRenderer != null)
			skinSpriteRenderer.enabled = false;

		if (gameMgr.currentLife < 1) {
			eventHandler.SetPauseButton(false);
			Invoke("AskUnityAdsQuestion",1f);

		} else if (gameMgr.AddLife (-1) >= 1) {
			allowInput = true;
			allowInput_jump = false;
			playSound(audioClips[5]);
			Invoke ("revive", 1f);
		} else {
			if(!deathSoundPlayed)
			{
				playSound (audioClips [3]);//die cuz fall through
				deathSoundPlayed = true;
			}

			eventHandler.SetPauseButton(false);
			Invoke("AskUnityAdsQuestion",1f);
		}

	}

	void StopRecording()
	{
		if (gameMgr.readyForRecording) {

			gameMgr.recorded = true;
		}
	}


	void revive()
	{
		StartCoroutine (DoRevive ());
	}

	IEnumerator DoRevive()
	{
		gameMgr.barGen.ActiveAllSpawnedBars ();

		if (lastBarFallThrough != null)
			gameObject.transform.position = lastBarFallThrough.gameObject.transform.position + gameMgr.barGen.barHeight * 0.5f;
		else {
			gameObject.transform.position = Vector3.zero;
			gameMgr.MainCam.gameObject.GetComponent<CameraController> ().ResetCamera (gameObject);
		}
		myAnimator.Play("Regular");
		gameObject.layer = 10 + (int)currentColor;
		spriteRenderer.enabled = true;
		if(skinSpriteRenderer != null)
			skinSpriteRenderer.enabled = true;
		allowInput_jump = true;
		isDead = false;
		deathSoundPlayed = false;

		playSound (audioClips [4]);

		yield return new WaitForSeconds(1f);
		



 
		MyRigidBody.gravityScale = defaultGravity;

	}

	void performJump()
	{

	}

	void Update () 
	{
		bool ButtonJumpDown, ButtonJumpHold, ButtonJumpUp;
		bool ButtonChangeColorDown = false;

		//Utils.addLog ("velocity y=" + MyRigidBody.velocity.y);


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
				ButtonChangeColorDown = true;
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
					if (gameMgr.mysave.currentJumpType == 0)
					{
						// left jump
						if(touch.position.x <= Screen.width / 2 )
						{
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
								ButtonChangeColorDown = true;
							}
						}
					}
					else//right jump
					{
						// left jump
						if(touch.position.x <= Screen.width / 2 )
						{	
							if (allowInput_color && touch.phase == TouchPhase.Began)
							{
								ButtonChangeColorDown = true;
							}
						}
						else
						{
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
					}

				}
				 
			}
		}
		
		#endif

		 
			if (ButtonChangeColorDown) {
				playSound (audioClips [2], 1);
				ChangeColor (-1, true);
			}

			HandleInput (ButtonJumpDown, ButtonJumpHold, ButtonJumpUp);
	 

		GameObject obj_foot = FootTouched ();
		if (obj_foot != null) {
			BarController bc = obj_foot.GetComponent<BarController> ();

			if(lastBarFallThrough == null)
				lastBarFallThrough = bc;
			else if(bc != null)
			{
				if(bc.gameObject.transform.position.y > lastBarFallThrough.gameObject.transform.position.y)
				{
					lastBarFallThrough = bc;
					//Utils.addLog("lastBarFallThrough changed");
				}
			}
		}


		GameObject bar = HeadKnocked ();
		if (bar != null) {
			//Utils.addLog("knock into bar: " + bar.GetComponent<BarController>().getColor());
			playSound(audioClips[1]);
			PlayScreenFlash();

			MyRigidBody.velocity = Vector2.zero;// -MyRigidBody.velocity * 0.1f;
			BarController bc = bar.GetComponent<BarController> ();
			EObjectColor barC = bc.getColor ();
			if(barC < EObjectColor.MAXCOLORNUM && gameMgr.currentLife > 1)
			{
				myAnimator.Play("FastFlashing");
				if(mySkinAnimator != null)
					mySkinAnimator.Play ("PlayerSkinHurt");
				//ChangeColor ((int)barC);

			}

			if(bc.barNum > 1)
			{
				AddPopup("LIFE - 1", gameMgr.MainCam.WorldToScreenPoint(gameObject.transform.position + new Vector3(popUpLifeTextOffset.x , popUpLifeTextOffset.y * popUpScreenPos.Count,0)), Time.time, popUpLifeGUIStyle);
				gameMgr.AddLife(-1);
			}

			if( gameMgr.currentLife < 1)
			{
				allowInput = false;
				gameObject.layer = LayerMask.NameToLayer("NoCollision");
				myAnimator.Play("Spin");
				
				if(mySkinAnimator != null)
					mySkinAnimator.Play ("PlayerSkinDeath");

				if(!deathSoundPlayed)
				{
					playSound (audioClips [3]);//die
					deathSoundPlayed = true;
				}
			}
			else
			{
				ResetJumpCount (0, null);
			}
		}
	}

	public void PlayScreenFlash(string animName = "fading")
	{
		fullScreenFlashImage.GetComponent<Animator>().Play(animName);
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
					CleanUpPopupStartWith('C');

				AddPopup("COMBO x "+combo.ToString(), gameMgr.MainCam.WorldToScreenPoint(gameObject.transform.position + new Vector3(popUpComboTextOffset.x , popUpComboTextOffset.y * popUpScreenPos.Count,0)), Time.time, popUpComboGUIStyle);
			}
		}
		else
			combo = 0;

	}

	void CleanUpPopupStartWith(char firstchar)
	{
		for(int i = 0; i < popUpText.Count; ++i)
		{ 
			if(popUpText[i][0] == firstchar)//AIR COMBO
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
		if(jumpCombo <= 1)
			eventHandler.SetExtraInfoText("NO BONUS");
		else
			eventHandler.SetExtraInfoText("SCORE x "+jumpCombo);//combo bonus
	}

	void checkStandTime()
	{
		setJumpCombo (0);
	}

	void checkJumpCombo(BarController lastBarStandOn)
	{
		if (Time.time - lastStandTime < jumpComboThreshold && !lastBarStandOn.isJumpedComboed) {
 			lastBarStandOn.isJumpedComboed = true;
			setJumpCombo(jumpCombo + 1);
			if(jumpCombo >= 2)
				AddPopup("COMBO x "+jumpCombo.ToString(), gameMgr.MainCam.WorldToScreenPoint(gameObject.transform.position + new Vector3(popUpComboTextOffset.x , popUpComboTextOffset.y * popUpScreenPos.Count,0)), Time.time, popUpComboGUIStyle);

		}
		else
			setJumpCombo(0);

	}
	string getJumpXstring()
	{
		string Xstring = "";
		if (maxJumpCount - currentJumpCount < 3) 
		{
			for (int i = 0; i < maxJumpCount - currentJumpCount; ++i) {
				Xstring += "X";
			}
		} else
			Xstring = "X " + (maxJumpCount - currentJumpCount).ToString ();

		return Xstring;
	}
	void HandleInput(bool bButtonJumpDown, bool bButtonJumpHold, bool bButtonJumpUp)
	{
		if (bButtonJumpDown && maxJumpCount > currentJumpCount) {

			if(totalJumpCount < 1)
				eventHandler.hideScorePanelShopAndGiftButton(); //only show you are at 0 floor
			
			if(mySkinAnimator != null)
				mySkinAnimator.Play ("PlayerSkinJump");

			totalJumpCount += 1;
			jumped = true;
			currentJumpCount += 1;
			eventHandler.SetJumpCountText("JUMP "+getJumpXstring() );
			MyRigidBody.velocity = Vector2.Min(Vector2.zero, Vector2.Max(new Vector2(0,-2f),MyRigidBody.velocity));//avoid crazy current vel !!!
			MyRigidBody.AddForce(new Vector3(0,jumpPower * 100f,0));
			playSound(audioClips[0],0,false,0.1f); //jjump
			if(lastBarStandOn != null)
			{
				CancelInvoke("checkStandTime");
				lastBarStandOn.onPlayerJumped();
				if(wantJumpCombo)
				{
					checkJumpCombo(lastBarStandOn);
				}

			}

		}
		/*
		if (bButtonJumpHold) {

		}

		if (bButtonJumpUp && maxJumpCount > currentJumpCount) {

		}
*/


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
		//Debug.DrawLine (myPos, myPos + Vector2.up * 0.15f,new Color(1,0,0,1));

		LayerMask lmask = (~(1 << (gameObject.layer))) - (1 <<  LayerMask.NameToLayer("NoCollision")) - (1 <<LayerMask.NameToLayer("Pickup"));

		RaycastHit2D hitup = Physics2D.Raycast(myPos, Vector2.up, 0.1f,lmask  );
		if (hitup.collider != null && MyRigidBody.velocity.y > 0 )
			return hitup.collider.gameObject;//knock into bar
		return null;
	}

	GameObject FootTouched()
	{
		if (MyRigidBody.velocity.y >= 0)
			return null;

		float halfPlayerSizeY = getHalfPlayerSizeY ();
		Vector2 myPos = new Vector2 (gameObject.transform.position.x, gameObject.transform.position.y - halfPlayerSizeY - 0.05f);
		
		RaycastHit2D hitDown = Physics2D.Raycast (myPos, -Vector2.up, 0.1f);
		if (hitDown.collider != null && hitDown.collider.gameObject != gameObject )
			return hitDown.collider.gameObject;//foot touch into bar
		return null;
	}

	public void SetJumpCountZero()
	{
		currentJumpCount = 0;
		eventHandler.SetJumpCountText("JUMP "+getJumpXstring());
		jumped = false;
	}

	public void ResetJumpCount(int num, BarController barController)
	{
		if (!isDead) {
			currentJumpCount = Mathf.Min (Mathf.Max (0, num), maxJumpCount);
			eventHandler.SetJumpCountText("JUMP "+getJumpXstring());
			jumped = false;
			combo = 0;
			if(barController != null)
			{
				lastBarStandOn = barController;
				lastStandTime = Time.time;
				maxBarNum = Mathf.Max(maxBarNum, lastBarStandOn.barNum);

				//Utils.addLog("stand on bar:" + barController.getColor());

				Invoke("checkStandTime",jumpComboThreshold);

				if(maxBarNum > minimumBarCountForHidingColorIndicatoin)
					gameMgr.SetColorIndication(false);// I am good enough to do this

				eventHandler.SetFloorText("FLOOR."+lastBarStandOn.barNum.ToString());
			}
			else
			{
				setJumpCombo(0);
			}
		}


	}

	public void AddScore(int s)
	{
		int realScore = s > 0? s + s * (Mathf.Max(0,combo - 1)) + s * (Mathf.Max(0,jumpCombo - 1))  : s  ;
 		gameMgr.AddScore (realScore);

		tempPlayerScore += realScore;
	 
		string jumpComboString = jumpCombo >= 2 ? " x " + jumpCombo.ToString () : "";

		CleanUpPopupStartWith ('#');//hack for score :D
		AddPopup ((realScore>0?"#" : "") + s.ToString ()+ jumpComboString, gameMgr.MainCam.WorldToScreenPoint (gameObject.transform.position + getPopUpOffSetByString("#")), Time.time, popUpScoreGUIStyle);
				 
		if (wantScoreToLife && tempPlayerScore >= scoreToLife) {
			tempPlayerScore -= scoreToLife;
			gameMgr.AddLife (1);

			AddPopup (scoreToLife.ToString() + "SCORE:"+"LIFE x 1", gameMgr.MainCam.WorldToScreenPoint (gameObject.transform.position + getPopUpOffSetByString("L")), Time.time, popUpLifeGUIStyle);

		}
	}

	public void playSound( AudioClip ac, int AudioSourceIdx = 0, bool loop = false , float overwriteVolume = 0.2f)
	{
		audioSourceList[AudioSourceIdx].clip = ac;
		audioSourceList[AudioSourceIdx].loop = loop;

		audioSourceList [AudioSourceIdx].volume = overwriteVolume;
		audioSourceList[AudioSourceIdx].Play();
	}

	bool IsPlaying(AudioClip ac, int AudioSourceIdx = 0)
	{
		return audioSourceList [AudioSourceIdx].clip == ac && audioSourceList [AudioSourceIdx].isPlaying;
	}

	public void stopSound(int AudioSourceIdx = 0)
	{
		audioSourceList [AudioSourceIdx].Stop ();
	}

	void CleanUpAllPopup()
	{
		 
		popUpText.Clear ();
		popUpScreenPos.Clear ();
		popUpGUIStyle.Clear ();
		popUpShowedTime.Clear ();
		 
	}

	void OnGUI() {


		if (popUpText.Count > 0) {
			for(int i = 0; i < popUpText.Count; i++)
			{
				string myText = popUpText[i];
				if(myText[0] == '#')//Score
					myText = myText.TrimStart('#');
				GUI.Label(new Rect (popUpScreenPos[i].x, Screen.height - popUpScreenPos[i].y, 1, 1), myText ,popUpGUIStyle[i]);
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
		else if (s [0] == 'C')//COMBO
			return 1f;
		else if (s [0] == '#')//SCORE
			return 1.25f;

		return 2f;//other pick ups
	}

	Vector3 getPopUpOffSetByString(string s)
	{
		if (s [0] == 'L')//LIFE - 1
			return new Vector3 (popUpLifeTextOffset.x, popUpLifeTextOffset.y * popUpScreenPos.Count, 0);
		else if (s [0] == 'C')//AIR COMBO
			return new Vector3 (popUpComboTextOffset.x, popUpComboTextOffset.y * popUpScreenPos.Count, 0);
		else if (s [0] == '#')//for score
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
		playSound (slowTimeLoopSound, slowtimeAudioSourceIdx, true,1f);
	}

	public bool Pickup(Pickup something)
	{
		if(something.UnlockHardMode)
			gameMgr.unlockHardcoreMode(true);

		if (something.gainLife)
			gameMgr.AddLife (something.gainLifeNum);

		if (something.AddJump)
			maxJumpCount += something.AddJumpNum;

		if (something.slowTime) {
			slowTimeStarted(something.timescale, something.slowTimeRecoverySpeed, something.slowTimeLoopSound);
		}

		if (something.GiveToken) {
			gameMgr.AddFreeGiftToken(something.GiveTokenNum);
		}

		AddPopup(something.popupMessage,  gameMgr.MainCam.WorldToScreenPoint (gameObject.transform.position + something.popUpTextOffset), Time.time, something.popupStyle);


		return true;
	}
}

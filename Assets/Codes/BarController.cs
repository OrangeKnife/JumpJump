using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BarController : MonoBehaviour
{
	public int score;
	SpriteRenderer spriteRenderer;
	EObjectColor barColor;
	AudioSource audioSource;

	public List<AudioClip> audioClips = new List<AudioClip>();
	

	public bool minusScore;
	public bool Flashing;
	public float FlashingInterVal;
	public float FlashingDuration,DisappearDurationAfterFlashing;
	public Material FlashingMaterial;

	public bool ScrollingColor;
	public float ScrollingColorSpeed;

	public bool fadingAfterPlayerJumped;
	public bool faded = false;

	float lastTimeFlashCheck = 0f;
	bool bIsDoingFlash = false;

	public bool isJumpedComboed = false;

	public float beforeShakingTime,shakingDuration;
	bool shaking = false;

	public GameObject barNumObj;
	public int barNum {get; private set;}

	Rigidbody2D shakingRigidBody;
	Vector3 savedShakingPos;
	GameManager gameMgr;
	void Awake ()
	{
		barColor = (EObjectColor)Random.Range (0,(int)EObjectColor.MAXCOLORNUM);
		//Utils.addLog (((int)barColor).ToString());
		gameObject.layer = 10 + (int)barColor;


		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();

		audioSource = GetComponent<AudioSource> ();
		//audioSource.volume = 0.2f;
		ChangeColor ();

		gameMgr = GameObject.Find ("GameManager").GetComponent<GameManager>();
	}

	bool shallWeDisplayBarNum(int n)
	{
		return n % 10 == 0;
	}

	public void setBarNum(int n)
	{
		barNum = n;
		if(shallWeDisplayBarNum(n))
			barNumObj.GetComponent<TextMesh>().text = n.ToString();

		score *= (n + 10) / 10;
	}

	public void onPlayerJumped()
	{
		if (fadingAfterPlayerJumped) {
			gameObject.SetActive(false);
			faded = true;
		}

		if (shaking) {
			gameObject.GetComponent<Animator> ().Play ("Regular");
			gameObject.transform.rotation = Quaternion.identity; //remove shaking effect
			CancelInvoke("ShakingThenDisappear");
			CancelInvoke("DoShaking");

		}
	}

	public void enableScrollingColor(bool en)
	{
		if (en) {
			ScrollingColor = en;
			GetComponent<MaterialScrollingController> ().enabled = true;
			GetComponent<MaterialScrollingController> ().SetScrollingSpeed(ScrollingColorSpeed);
		}
	}

	public void enableFlashing(bool en)
	{
		if (en) {
			Flashing = en;
			barColor = EObjectColor.BLACK;
			gameObject.layer = LayerMask.NameToLayer("CollideAll");
			spriteRenderer.material = FlashingMaterial;
			spriteRenderer.material.SetColor("_Color",getColorByColorEnum (barColor));
		}
	}

	public void enableShaking(bool en)
	{
		shaking = en;
	}

	public EObjectColor getColor()
	{
		return barColor;
	}

	public void FlashingOut()
	{
		gameObject.layer = LayerMask.NameToLayer("NoCollision");
		spriteRenderer.enabled = false;
	}


	public void stopFlashing()
	{
		bIsDoingFlash = false;
		gameObject.layer = LayerMask.NameToLayer("CollideAll");
		gameObject.GetComponent<Animator> ().Play ("Regular");
		spriteRenderer.enabled = true;
	}


	public void SetBarColorAndLayerByColor(EObjectColor c)
	{
		if (c < EObjectColor.MAXCOLORNUM) {
			barColor = c;
			gameObject.layer = 10 + (int)barColor;
		}

		//Debug.Log ("SetLayerByColor " + c.ToString ());
	}

	public void ChangeColor()
	{
		barColor = barColor + 1;
		if (barColor == EObjectColor.MAXCOLORNUM)
			barColor = 0;
				
		gameObject.layer = 10 + (int)barColor;
		//spriteRenderer.color = getColorBuyColorEnum (barColor);

		spriteRenderer.material.SetTextureOffset("_MainTex",getColorTextureOffsetByColorEnum (barColor));

		if(Flashing)
			gameObject.GetComponent<Animator> ().Play ("Regular");

	}
	
	Color getColorByColorEnum(EObjectColor oc)
	{
		Color rtColor = new Color();
		switch (oc) {
		case EObjectColor.RED:
			rtColor = PlayerController.RedColor;
			break;
		case EObjectColor.BLUE:
			rtColor = PlayerController.BlueColor;
			break;
		case EObjectColor.GREEN:
			rtColor = PlayerController.GreenColor;
			break;
		case EObjectColor.YELLOW:
			rtColor = PlayerController.YellowColor;
			break;
		case EObjectColor.BLACK:
			rtColor = PlayerController.BlackColor;
			break;
		}
		
		return rtColor;
	}

	Vector2 getColorTextureOffsetByColorEnum(EObjectColor oc)
	{
		Vector2 rtColorOffset = new Vector2();
		switch (oc) {
		case EObjectColor.RED:
			rtColorOffset =  new Vector2(-7f/16f,0f);
			break;
		case EObjectColor.BLUE:
			rtColorOffset = new Vector2(-5f/16f,0f);
			break;
		case EObjectColor.GREEN:
			rtColorOffset = new Vector2(-3f/16f,0f);
			break;
		case EObjectColor.YELLOW:
			rtColorOffset = new Vector2(-1f/16f,0f);;
			break;
		}
		
		return rtColorOffset;
	}

	// Update is called once per frame
	void Update ()
	{
		if (Flashing && !faded) {
			if(Time.time - lastTimeFlashCheck > FlashingInterVal && !bIsDoingFlash)
			{
				Invoke("DoFlashing", Random.Range(0f,1f));//just have some random

			}
		}
	}

	void DoFlashing()
	{
		lastTimeFlashCheck = Time.time;
		bIsDoingFlash = true;
		gameObject.GetComponent<Animator> ().Play ("FlashingBarAnimation");

		//gameObject.layer = LayerMask.NameToLayer("NoCollision");
		Invoke("FlashingOut",FlashingDuration);//disappear
		 
		Invoke("stopFlashing",FlashingDuration + DisappearDurationAfterFlashing);//come back !
	}


	public void ResetBar()
	{
		faded = false;
		CancelInvoke ();
		if (Flashing) {
			stopFlashing();
		}

		if (shaking) {
			if(shakingRigidBody != null)
			{
				gameObject.GetComponent<Animator> ().Play ("Regular");
				gameObject.layer = 10 + (int)barColor;
				gameObject.transform.position = savedShakingPos;
				GameObject.Destroy(shakingRigidBody);
			}
		}

	}

	void OnTriggerExit2D(Collider2D  other) 
	{
		if (other.gameObject.tag == "Player") 
		{
			if (shaking)
			{
				if (other.gameObject.GetComponent<Rigidbody2D> ().velocity.y < 0)
				{
					//Utils.addLog ("player fall through one bar");

					gameObject.GetComponent<Animator> ().Play ("Regular");
					gameObject.transform.rotation = Quaternion.identity; //remove shaking effect
					CancelInvoke("ShakingThenDisappear");
					CancelInvoke("DoShaking");
				}
			}
		}
	}

	/*	void OnTriggerEnter2D(Collider2D  other) 
	{

		if (other.gameObject.tag == "Player") {
			if(other.gameObject.GetComponent<Rigidbody2D>().velocity.y > 0)
				return;

			float halfPlayerSize = other.gameObject.GetComponent<PlayerController>().getHalfPlayerSizeY();

			if(other.gameObject.transform.position.y  < gameObject.transform.position.y )
				return;
 
			other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

			if(minusScore && score < 0 || score > 0)
				other.gameObject.GetComponent<PlayerController> ().AddScore (score);

			if (score > 0) {
				score = -1;
				audioSource.clip = audioClips [0];
				audioSource.Play ();
			} else {
				score --;
				audioSource.clip = audioClips [1];
				audioSource.Play ();

			}

			other.gameObject.GetComponent<PlayerController> ().ResetJumpCount (0, this);
		}


	}	 */

	public void DoStandOnLogic(PlayerController pc)
	{
 
		if(pc.gameObject.transform.position.y < gameObject.GetComponent<BoxCollider2D>().transform.position.y)
			return;

 
		if(pc.gameObject.GetComponent<Rigidbody2D>().velocity.y > 0 )
			return;
			
		if(pc.gameObject.transform.position.y  < gameObject.transform.position.y )
			return;
		
		//other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

		if(minusScore && score < 0 || score > 0)
		{
			pc.AddScore (score);
		}
		
		if (score > 0) {
			score = -1;
			
			
			if(true)//!audioSource.isPlaying)
			{
				if(shallWeDisplayBarNum(barNum))
				{
					audioSource.clip = audioClips [3];//speical sfx for 10,20,30....
					audioSource.volume = 0.4f;
					
					//lets change BG
					gameMgr.ChangeRandomBG();
				}
				else
				{
					audioSource.clip = audioClips [0];
					audioSource.volume = 0.7f;
				}
				audioSource.Play ();
			}
			
			
		} else {
			score --;
			
			if(!audioSource.isPlaying)
			{
				audioSource.clip = audioClips [1];
				audioSource.volume = 0.05f;
				audioSource.Play ();
			}
			
		}
		
		pc.ResetJumpCount (0, this);
		
		
		if(shaking)
		{
			CancelInvoke("DoShaking");
			CancelInvoke("ShakingThenDisappear");
			Invoke("DoShaking",beforeShakingTime);
			//Utils.addLog("Start check shaking");
			GetComponent<Animator>().Play("oneTimeShaking");
		}
	}

//	void OnCollisionEnter2D(Collision2D other)
//	{
//		if (other.gameObject.tag == "Player") {
//			if(other.gameObject.transform.position.y < gameObject.GetComponent<BoxCollider2D>().transform.position.y)
//				return;
//
//			//other.gameObject.GetComponent<PlayerController> ().SetJumpCountZero ();//hack
//			/*
//			if(!audioSource.isPlaying)//hack
//			{
//				audioSource.clip = audioClips [1];
//				audioSource.volume = 0.03f;
//				audioSource.Play ();
//			}*/
//
//			
//			if(other.gameObject.GetComponent<Rigidbody2D>().velocity.y > 0 )
//				return;
//				
//			if(other.gameObject.transform.position.y  < gameObject.transform.position.y )
//				return;
//			
//			//other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
//			
//			PlayerController pc = other.gameObject.GetComponent<PlayerController>();
//			DoStandOnLogic(pc);
//		}
//	}

	void DoShaking()
	{
		//Utils.addLog("now is shaking");
		gameObject.GetComponent<Animator> ().Play ("shaking");
		Invoke ("ShakingThenDisappear", shakingDuration);
	}

	void ShakingThenDisappear()
	{
		//Utils.addLog("ShakingThenDisappear");
		gameObject.transform.rotation = Quaternion.identity; //remove shaking effect
		//gameObject.SetActive (false);
		faded = true;

		savedShakingPos = gameObject.transform.position;
		shakingRigidBody = gameObject.AddComponent<Rigidbody2D>();
		gameObject.layer = LayerMask.NameToLayer("NoCollision");
	}


	public void NotifyBarColorChanged (Vector2 offset)
	{
		EObjectColor c = getColorByTextureOffset (offset);
		if (c < EObjectColor.MAXCOLORNUM && barColor != c)
			SetBarColorAndLayerByColor (c);


	}

	EObjectColor getColorByTextureOffset(Vector2 offset)
	{
		/*if ((0 - offset.x) < 0.001f && (0 - offset.x) > -0.001f)
			return EObjectColor.RED;
		if ((-6f/16f - offset.x)  < 0.001f && (-6f/16f - offset.x) > -0.001f)	 
			return EObjectColor.BLUE;
		if ((-4f/16f - offset.x)  < 0.001f && (-4f/16f - offset.x) > -0.001f)	 
			return EObjectColor.GREEN;
		if ((-2f/16f - offset.x) < 0.001f && (-2f/16f - offset.x) > -0.001f)	 
			return EObjectColor.YELLOW;
*/

		if (offset.x > 0 && offset.x < 1f / 8f || offset.x > -4f/8f && offset.x < -3f/8f)
			return EObjectColor.RED;
		if (offset.x > -3f/8f && offset.x < -2f/8f)
			return EObjectColor.BLUE;
		if (offset.x > -2f/8f && offset.x < -1f/8f) 
			return EObjectColor.GREEN;
		if (offset.x > -1f/8f && offset.x < 0)  
			return EObjectColor.YELLOW;
		 

		Utils.addLog ("Color is wrong!");

		return EObjectColor.MAXCOLORNUM;
	}

}


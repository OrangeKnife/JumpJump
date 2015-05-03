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

	static Color RedColor = new Color(234f/255f,46f/255f,73f/255f);
	static Color BlueColor = new Color(119f/255f,196f/255f,211f/255f);
	static Color GreenColor = new Color(102f/255f,196f/255f,50f/255f);
	static Color YellowColor = new Color(246f/255f,247f/255f,74f/255f);
	static Color BlackColor = new Color(0f,0f,0f);//collide all colors

	public bool minusScore;
	public bool Flashing;
	public float FlashingInterVal;
	public Material FlashingMaterial;

	public bool ScrollingColor;
	public float ScrollingColorSpeed;


	float lastTimeTick = 0f;
	bool bIsDoingFlash = false;
	void Awake ()
	{
		barColor = (EObjectColor)Random.Range (0,(int)EObjectColor.MAXCOLORNUM);
		gameObject.layer = 10 + (int)barColor;


		spriteRenderer = GetComponent<SpriteRenderer> ();

		audioSource = GetComponent<AudioSource> ();
		ChangeColor ();

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

	public EObjectColor getColor()
	{
		return barColor;
	}

	public void StopFlashing()
	{
		bIsDoingFlash = false;
		gameObject.layer = LayerMask.NameToLayer("CollideAll");
		gameObject.GetComponent<Animator> ().Play ("Regular");
	}


	public void SetBarColorAndLayerByColor(EObjectColor c)
	{
		if (c < EObjectColor.MAXCOLORNUM) {
			barColor = c;
			gameObject.layer = 10 + (int)barColor;
		}

		Debug.Log ("SetLayerByColor " + c.ToString ());
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
		case EObjectColor.BLACK:
			rtColor = BlackColor;
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
		if (Flashing) {
			if(Time.time - lastTimeTick > FlashingInterVal && !bIsDoingFlash)
			{
				DoFlashing();
				lastTimeTick = Time.time;
			}
			 
		}
	}

	void DoFlashing()
	{
		bIsDoingFlash = true;
		gameObject.GetComponent<Animator> ().Play ("FlashingBarAnimation");
		gameObject.layer = LayerMask.NameToLayer("NoCollision");
		Invoke("StopFlashing",1.5f);
	}


	void OnTriggerEnter2D(Collider2D  other) 
	{
		if (other.gameObject.tag == "Player") {
			if(other.gameObject.GetComponent<Rigidbody2D>().velocity.y > 0)
				return;

			if(other.gameObject.transform.position.y < gameObject.transform.position.y )
				return;

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

	}

	public void NotifyBarColorChanged (Vector2 offset)
	{
		EObjectColor c = getColorByTextureOffset (offset);
		if (c < EObjectColor.MAXCOLORNUM && barColor != c)
			SetBarColorAndLayerByColor (c);

	}

	EObjectColor getColorByTextureOffset(Vector2 offset)
	{
		if ((0 - offset.x) < 0.001f && (0 - offset.x) > -0.001f)
			return EObjectColor.RED;
		if ((-6f/16f - offset.x)  < 0.001f && (-6f/16f - offset.x) > -0.001f)	 
			return EObjectColor.BLUE;
		if ((-4f/16f - offset.x)  < 0.001f && (-4f/16f - offset.x) > -0.001f)	 
			return EObjectColor.GREEN;
		if ((-2f/16f - offset.x) < 0.001f && (-2f/16f - offset.x) > -0.001f)	 
			return EObjectColor.YELLOW;
		/*
		if ((offset - new Vector2 (0.3f, 0)).sqrMagnitude < 0.0001f)
			return EObjectColor.RED;
		if ((offset - new Vector2 (-0.3f, 0)).sqrMagnitude < 0.0001f)	 
			return EObjectColor.BLUE;
		if ((offset - new Vector2(-0.1f,0)).sqrMagnitude < 0.0001f)	 
			return EObjectColor.GREEN;
		if ((offset - new Vector2 (0.1f, 0)).sqrMagnitude < 0.0001f)	 
			return EObjectColor.YELLOW;
*/
		
		return EObjectColor.MAXCOLORNUM;
	}

}


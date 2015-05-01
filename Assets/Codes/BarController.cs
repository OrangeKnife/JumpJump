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

	public bool minusScore;
	public bool TickingColor;
	public float TickingColorInterVal;
	float lastTimeTick = 0f;
	void Awake ()
	{
		barColor = (EObjectColor)Random.Range (0,(int)EObjectColor.MAXCOLORNUM);
		gameObject.layer = 10 + (int)barColor;


		spriteRenderer = GetComponent<SpriteRenderer> ();

		audioSource = GetComponent<AudioSource> ();
		ChangeColor ();
	}

	public EObjectColor getColor()
	{
		return barColor;
	}


	public void ChangeColor()
	{
		barColor = barColor + 1;
		if (barColor == EObjectColor.MAXCOLORNUM)
			barColor = 0;
				
		gameObject.layer = 10 + (int)barColor;
		spriteRenderer.color = getColorBuyColorEnum (barColor);

		if(TickingColor)
			gameObject.GetComponent<Animator> ().Play ("Regular");

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

	// Update is called once per frame
	void Update ()
	{
		if (TickingColor) {
			if(Time.time - lastTimeTick > TickingColorInterVal)
			{
				FlashingColor();
				lastTimeTick = Time.time;
			}
			 
		}
	}

	void FlashingColor()
	{
		gameObject.GetComponent<Animator> ().Play ("FlashingBarAnimation");
		Invoke("ChangeColor",1.5f);
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

 


}


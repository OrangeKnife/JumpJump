using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BarController : MonoBehaviour
{
	public bool TimedBroken = false;
	int score = 1;
	SpriteRenderer spriteRenderer;
	EObjectColor barColor;
	AudioSource audioSource;

	public List<AudioClip> audioClips = new List<AudioClip>();

	static Color RedColor = new Color(234f/255f,46f/255f,73f/255f);
	static Color BlueColor = new Color(119f/255f,196f/255f,211f/255f);
	static Color GreenColor = new Color(102f/255f,196f/255f,50f/255f);
	static Color YellowColor = new Color(246f/255f,247f/255f,146f/255f);

	void Start ()
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


	void ChangeColor()
	{
		barColor = barColor + 1;
		if (barColor == EObjectColor.MAXCOLORNUM)
			barColor = 0;
				
		gameObject.layer = 10 + (int)barColor;
		spriteRenderer.color = getColorBuyColorEnum (barColor);
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
	
	}


	void OnTriggerEnter2D(Collider2D  other) 
	{
		//add score first cuz we have combo
		if (score > 0) {
			other.gameObject.GetComponent<PlayerController> ().AddScore (score);
			score = 0;
			audioSource.clip = audioClips [0];
			audioSource.Play ();
		} else {
			audioSource.clip = audioClips[1];
			audioSource.Play();

		}

		if (other.gameObject.tag == "Player")
			other.gameObject.GetComponent<PlayerController> ().ResetJumpCount (0, this);


	}


}


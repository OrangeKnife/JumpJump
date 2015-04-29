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


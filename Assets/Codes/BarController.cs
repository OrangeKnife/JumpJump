using UnityEngine;
using System.Collections;

public class BarController : MonoBehaviour
{
	public bool TimedBroken = false;

	SpriteRenderer spriteRenderer;
	EObjectColor barColor;

	void Start ()
	{
		barColor = (EObjectColor)Random.Range (0,(int)EObjectColor.MAXCOLORNUM);
		gameObject.layer = 10 + (int)barColor;


		spriteRenderer = GetComponent<SpriteRenderer> ();


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

	public void onPlayerHold()
	{
		print ("I am hold");
	}

	void OnTriggerEnter2D(Collider2D  other) 
	{
		if (other.gameObject.tag == "Player")
			other.gameObject.GetComponent<PlayerController> ().ResetJumpCount (0);
	}


}


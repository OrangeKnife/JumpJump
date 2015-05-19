using UnityEngine;
using System.Collections;
using Soomla.Store;
public class FreeGiftController : MonoBehaviour
{
	GameManager gameMgr;
	UnityEngine.UI.Text FreeTokenInfoText;
	Animator textAnimator;
	float lastTimeTick;
	int currentTextAnim = -1;
	void Start ()
	{
		FreeTokenInfoText = gameObject.GetComponent<UnityEngine.UI.Text> ();
		gameMgr = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		textAnimator = gameObject.GetComponent<Animator> ();
		lastTimeTick = Time.realtimeSinceStartup;
	}

	void Update()
	{
		if (Time.realtimeSinceStartup - lastTimeTick > 1f)
			OneSecondTick ();
	}


	void OneSecondTick ()
	{ 
		if (!gameMgr.bGameStarted) {
			int freeGiftTimeLeft = gameMgr.getFreeGiftGiveAwayTimeLeft ();
			if (freeGiftTimeLeft > 0)
			{
				FreeTokenInfoText.text = "FREE  TOKENS  IN  " + getTimeStringFromSeconds (freeGiftTimeLeft);
				if (currentTextAnim != 0)
				{
					textAnimator.Play ("FlashText"); 
					currentTextAnim = 0;
				}
			}
			else if (freeGiftTimeLeft == 0) {
				FreeTokenInfoText.text = "FREE  TOKENS  READY!  CHECK  OUT  YOUR  GIFT!";
				if (currentTextAnim != 1)
				{
					textAnimator.Play ("FlashingTextOneSecondAnimation"); 
					currentTextAnim = 1;
				}
			} else
				FreeTokenInfoText.text = "";

			Utils.addLog ("time left:" + freeGiftTimeLeft.ToString ());
		} 

		lastTimeTick = Time.realtimeSinceStartup;
	}

	public string getTimeStringFromSeconds(int seconds)
	{
		int h = seconds / 3600;
		int m = (seconds - h * 3600 + 59) / 60;
		//int s = seconds - h * 3600 - (m - 1) * 60;
		//Utils.addLog ("seconds="+seconds.ToString()+",h=" + h.ToString () + ",m=" + m.ToString () + ",s=" + s.ToString ());
		return (h > 0? h.ToString()+" H ":"") + m.ToString() + " M";
	}
}


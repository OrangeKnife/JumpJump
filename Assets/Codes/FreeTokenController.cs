using UnityEngine;
using System.Collections;
using Soomla.Store;
public class FreeTokenController : MonoBehaviour
{
	GameManager gameMgr;
	UnityEngine.UI.Text InfoText;
	Animator textAnimator;
	float lastTimeTick;
	int currentTextAnim = -1;
	void Start ()
	{
		InfoText = gameObject.GetComponent<UnityEngine.UI.Text> ();
		gameMgr = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		textAnimator = gameObject.GetComponent<Animator> ();
		lastTimeTick = Time.realtimeSinceStartup;
	}

	void Update()
	{
		if (gameMgr.syncTimeSuccess &&  !gameMgr.bGameStarted && Time.realtimeSinceStartup - lastTimeTick > 2f)
			CheckFreeTokenReady ();
	}

	public void ForceDisplayNonSynchronizedInfo()
	{
		//InfoText.text = "CONNECT  TO  INTERNET  AND  GET  FREE  TOKENS !";
		InfoText.text = "WATCH  A  VIDEO  AND  GET  FREE  TOKENS !";
	}


	void CheckFreeTokenReady ()
	{ 
		if (true) {
			int freeTokenTime = gameMgr.getFreeTokenGiveAwayTime ();
			int myCurrentSyncTimeMins = gameMgr.synchronizedMinutes + (int)gameMgr.GetTimeElaspeSinceSyncTimeSuccessMins();

			if(myCurrentSyncTimeMins < freeTokenTime)
			{
				InfoText.text = "FREE  TOKENS  IN  " +  (freeTokenTime - myCurrentSyncTimeMins).ToString() + " MIN";
				if (currentTextAnim != 0)
				{
					textAnimator.Play ("FlashText"); 
					currentTextAnim = 0;
				}
			}
			else
			{
				InfoText.text = "FREE  TOKENS  READY!  CHECK  OUT  YOUR  GIFT!";
				if (currentTextAnim != 1)
				{
					textAnimator.Play ("FlashingTextOneSecondAnimation"); 
					currentTextAnim = 1;
				}
			} 

			//Utils.addLog ("time left [min]:" + (freeTokenTime - myCurrentSyncTimeMins).ToString());
		} 

		lastTimeTick = Time.realtimeSinceStartup;
	}
	
}


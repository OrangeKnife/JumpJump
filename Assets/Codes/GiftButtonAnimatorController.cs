using UnityEngine;
using System.Collections;

public class GiftButtonAnimatorController : MonoBehaviour {

	GameManager gameMgr;
	// Use this for initialization
	void Start () {
		gameMgr = GameObject.Find ("GameManager").GetComponent<GameManager> ();
		if (gameMgr.IsFreeTokenReady ())
			gameObject.GetComponent<Animator> ().Play ("FlashingTextOneSecondAnimation");
	}
 
}

using UnityEngine;
using System.Collections;

public class TransitionController : MonoBehaviour {

	public delegate void myTransitionDelegate();
	public myTransitionDelegate transitionDelegates;

	public bool doingTransition { get; private set; }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void startTransition()
	{
		doingTransition = true;
		gameObject.SetActive (true);
		gameObject.GetComponent<Animator> ().Play ("TransitionAnimation");
	}
	public void TransitionFinished()
	{
		gameObject.SetActive (false);
	}

	public void DoTransition()
	{
		Utils.addLog ("DoTransition");
		transitionDelegates ();
		transitionDelegates = null;
		doingTransition = false;
	}
}

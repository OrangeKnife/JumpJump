using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BarGenerator : MonoBehaviour {

	private int howMany = 8;
	public GameObject barTemmplate;

	private GameObject player;

	private LinkedList<GameObject> SpawnedBarsList;

	public Vector3 firstBarLocation,barHeight;
	public float flashingBarChance,scrollingColorChance;
	public int minimumBarCountForFlashingBar,minimumBarCountForScrollingColor,minimumBarCountForFadingBar;
	public int minimumBarCountHavingAdjacentSameColorBar,minimumBarCountForHavingAdjacentScrollingBar;
	Vector3 lastBarLocation = Vector3.zero;
	GameManager gameMgr;
	int barCount = 0;
	int pickupCount = 1;
	public int barCountPerPickUp = 25;
	public float pickupChance;
	bool lastBarFlashing = false;
	List<GameObject> PickupList;
	void Start () 
	{
		if (gameMgr == null)
			gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();

		
		if (SpawnedBarsList == null)
			SpawnedBarsList = new LinkedList<GameObject>();

		if (PickupList == null)
			PickupList = new List<GameObject> ();

	}

	public void onGameStarted()
	{
		foreach (GameObject obj in SpawnedBarsList) {
			Destroy (obj);
		}
		SpawnedBarsList.Clear ();

		foreach (GameObject obj in PickupList) {
			Destroy (obj);
		}
		PickupList.Clear ();

		player = gameMgr.GetCurrentPlayer();

		
		lastBarLocation = firstBarLocation - barHeight;

		barCount = 0;

		pickupCount = 1;
	}
	
	public void ActiveAllSpawnedBars(bool wantToActive)
	{
		foreach (GameObject go in SpawnedBarsList) {
			go.SetActive(wantToActive);
		}
	}

	void Update () 
	{
		if (gameMgr.bGameStarted) {
			if (lastBarLocation.y - player.transform.position.y < 5 * barHeight.y)
				SpawnBar (barTemmplate);

			if (SpawnedBarsList!=null && SpawnedBarsList.First != null && player.transform.position.y - SpawnedBarsList.First.Value.transform.position.y > 5 * barHeight.y)
			{
				Destroy (SpawnedBarsList.First.Value);
				SpawnedBarsList.RemoveFirst ();
			}
		}
	}

	public Vector3 GetFirstBarLocation()
	{
		if (SpawnedBarsList.First != null)
			return SpawnedBarsList.First.Value.transform.position;
		else
			return firstBarLocation;
	}


	void SpawnBar(GameObject template)
	{
		EObjectColor lastBarColor = EObjectColor.MAXCOLORNUM;
		BarController lastBarController = null;

		for (int i = 0; i < howMany; i++) {
			GameObject newBar = Instantiate (template);

			BarController bc = newBar.GetComponent<BarController>();
			if(bc.getColor() == lastBarColor && barCount <= minimumBarCountHavingAdjacentSameColorBar)
				bc.ChangeColor();

			lastBarColor = bc.getColor();

			if(barCount > minimumBarCountForScrollingColor && Random.Range(0f,1f) < scrollingColorChance  
			   && (barCount > minimumBarCountForHavingAdjacentScrollingBar || (lastBarController == null || (lastBarController != null && !lastBarController.ScrollingColor))))
			{
				bc.enableScrollingColor(true);
			}
			else if(!lastBarFlashing && barCount > minimumBarCountForFlashingBar && Random.Range(0f,1f) < flashingBarChance)
				bc.enableFlashing(true);

			if(barCount > minimumBarCountForFadingBar)
				bc.fadingAfterPlayerJumped = true;

			lastBarFlashing = bc.Flashing;

			newBar.transform.position = lastBarLocation + barHeight;
			lastBarLocation = newBar.transform.position;
			SpawnedBarsList.AddLast(newBar);
			barCount++;
			lastBarController = newBar.GetComponent<BarController>();


			if(barCount > pickupCount * barCountPerPickUp && Random.Range(0f,1f) < pickupChance)
			{
				SpawnPickup( gameMgr.PickupTemplates[ Random.Range(0,gameMgr.PickupTemplates.Count)] , lastBarLocation);
				pickupCount++;
			}

		}
	}

	void SpawnPickup(GameObject pickupTemplate, Vector3 barloc)
	{
		GameObject go = GameObject.Instantiate (pickupTemplate);
		PickupList.Add (go);
		go.transform.position = barloc + new Vector3(barloc.x, Random.Range(barHeight.y*0.3f,barHeight.y * 0.6f),barloc.z);
	}

}

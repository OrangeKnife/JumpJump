using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BarGenerator : MonoBehaviour {

	private int howMany = 8;
	public GameObject barTemmplate;

	private GameObject player;
	private Transform playerTransform;
	private LinkedList<GameObject> SpawnedBarsList;

	public Vector3 firstBarLocation,barHeight;
	Vector3 lastBarLocation = Vector3.zero;
	GameManager gameMgr;
	// Use this for initialization
	void Start () 
	{
		if (gameMgr == null)
			gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();

		
		if (SpawnedBarsList == null)
			SpawnedBarsList = new LinkedList<GameObject>();

	}

	public void onGameStarted()
	{
		foreach (GameObject obj in SpawnedBarsList) {
			Destroy (obj);
		}
		SpawnedBarsList.Clear ();

		player = gameMgr.GetCurrentPlayer();
		playerTransform = player.transform;
		
		lastBarLocation = firstBarLocation - barHeight;
	}
	


	void Update () 
	{
		if (gameMgr.bGameStarted) {
			if (lastBarLocation.y - player.transform.position.y < 5 * barHeight.y)
				SpawnBar (barTemmplate);

			if (SpawnedBarsList.First != null && player.transform.position.y - SpawnedBarsList.First.Value.transform.position.y > 5 * barHeight.y)
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
		for (int i = 0; i < howMany; i++) {
			GameObject newBar = Instantiate (template);
			newBar.transform.position = lastBarLocation + barHeight;
			lastBarLocation = newBar.transform.position;
			SpawnedBarsList.AddLast(newBar);
		}
	}

}

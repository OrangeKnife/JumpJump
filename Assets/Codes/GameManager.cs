using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GameManager : MonoBehaviour {
	public List<GameObject> PlayerTemplates;
	private GameObject CurrentPlayer;

	bool bGameStarted = false;
	GameSceneEvents eventHandler = null;
	GameObject CurrentPlayerTemplate;
	public GameObject MainCam { get; private set;}

	void Start () {
		SetCurrentPlayerTemplateByIdx (0);
		MainCam = GameObject.Find ("Main Camera");
	}

	public void StartGame()
	{
		bGameStarted = true;
	
		RespawnPlayer ();

	}

	public void EndGame()
	{
		bGameStarted = false;
		GetComponent<SectionGenerator> ().enabled = bGameStarted;
	}

	public void SetAudioAvailable(bool bAvailable)
	{
	


	}

	public void PlayBackground() {

	}


	public bool SetCurrentPlayerTemplateByIdx(int idx)
	{
		if (PlayerTemplates.Count > 0) {
			CurrentPlayerTemplate = PlayerTemplates [idx];
			return true;
		} else 
			return false;
	}

	void Update () {

	}

	void Awake(){
		System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");

		DontDestroyOnLoad(gameObject);
	}

	public void RespawnPlayer()
	{
		if (CurrentPlayer != null)
		{
			Destroy(CurrentPlayer);
		}

		if(eventHandler == null)
			eventHandler = GameObject.Find ("eventHandler").GetComponent<GameSceneEvents>();

		CurrentPlayer = Instantiate(CurrentPlayerTemplate);
		MainCam.GetComponent<CameraController> ().ResetCamera (CurrentPlayer);

		if(eventHandler)
			eventHandler.onPlayerRespawn ();
	}

	public GameObject GetCurrentPlayer()
	{
		return CurrentPlayer;
	}


}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;

public class GameManager : MonoBehaviour {
	public List<GameObject> PlayerTemplates;
	private GameObject CurrentPlayer;

	public bool bGameStarted { get; private set; }
	public Camera MainCam { get; private set;}
	public BarGenerator barGen { get; private set;}

	GameSceneEvents eventHandler = null;
	GameObject CurrentPlayerTemplate;

	public int currentScore {get; private set;}
	public int currentLife { get; private set; }
	public int bestScore { get; private set; }

	SaveObject mysave;
	void Start () {

		SetCurrentPlayerTemplateByIdx (0);
		MainCam = GameObject.Find ("Main Camera").GetComponent<Camera>();

		barGen = GetComponent<BarGenerator> ();


		if (GameFile.Load ("save.data", ref mysave))
			bestScore = mysave.bestScore;

		//leaderboard
#if UNITY_ANDROID || UNITY_IOS

		Social.localUser.Authenticate (success => {
			if (success) {
				Debug.Log ("Authentication successful");
				string userInfo = "Username: " + Social.localUser.userName + 
					"\nUser ID: " + Social.localUser.id + 
						"\nIsUnderage: " + Social.localUser.underage;
				Debug.Log (userInfo);

				ILeaderboard leaderboard = Social.CreateLeaderboard();
				leaderboard.id = "Leaderboard001";
				leaderboard.LoadScores(result =>
				                       {
					Debug.Log("Received " + leaderboard.scores.Length + " scores");
					foreach (IScore score in leaderboard.scores)
						Debug.Log(score);
				});

			}
			else
				Debug.Log ("Authentication failed");
		});


#endif

	}

	

	public void StartGame()
	{
		bGameStarted = true;
		currentScore = 0;
		currentLife = 3;
		eventHandler.UpdateUILife (currentLife);

		RespawnPlayer ();

		barGen.onGameStarted ();
	
		eventHandler.onGameStarted ();


	}

	public void EndGame()
	{
		bGameStarted = false;
		if (currentScore > bestScore) {
			bestScore = currentScore;
			mysave.bestScore = bestScore;
			GameFile.Save("save.data",mysave);

			//leaderboard
			ILeaderboard leaderboard = Social.CreateLeaderboard();
			leaderboard.id = "Leaderboard1";
			leaderboard.LoadScores(result =>
			                       {
				Debug.Log("Received " + leaderboard.scores.Length + " scores");
				foreach (IScore score in leaderboard.scores)
					Debug.Log(score);
			});

		}
		eventHandler.onGameEnded ();
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
		MainCam.gameObject.GetComponent<CameraController> ().ResetCamera (CurrentPlayer);

		if(eventHandler)
			eventHandler.onPlayerRespawn ();
	}

	public GameObject GetCurrentPlayer()
	{
		return CurrentPlayer;
	}


	public int AddScore(int s)
	{
		currentScore += s;
		eventHandler.UpdateUISocre (currentScore);
		return currentScore;
	}

	public int AddLife(int l)
	{
		currentLife += l;
		eventHandler.UpdateUILife (currentLife);
		return currentLife;
	}
}

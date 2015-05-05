﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;

public class GameManager : MonoBehaviour {
	public List<GameObject> PlayerTemplates;
	private GameObject CurrentPlayer;

	public List<GameObject> PickupTemplates;

	public bool bGameStarted { get; private set; }
	public Camera MainCam { get; private set;}
	public BarGenerator barGen { get; private set;}

	GameSceneEvents eventHandler = null;
	GameObject CurrentPlayerTemplate;

	public int currentScore {get; private set;}
	public int currentLife { get; private set; }
	public int bestScore { get; private set; }

	public List<AudioClip> backgroundMusic;
	int currentBGMindex = 0;
	AudioSource audiosource;

	SaveObject mysave;

	int gameMode = 0;

	public Color fromCameraColor;//218,237,226
	public Color towardsCameraColor;
	void Start () {

		SetCurrentPlayerTemplateByIdx (0);
		MainCam = GameObject.Find ("Main Camera").GetComponent<Camera>();

		barGen = GetComponent<BarGenerator> ();


		if (GameFile.Load ("save.data", ref mysave))
			bestScore = mysave.bestScore;

		//leaderboard
#if UNITY_IOS

		Social.localUser.Authenticate (success => {
			if (success) {
				Debug.Log ("Authentication successful");
				string userInfo = "Username: " + Social.localUser.userName + 
					"\nUser ID: " + Social.localUser.id + 
						"\nIsUnderage: " + Social.localUser.underage;
				Debug.Log (userInfo);

				ILeaderboard leaderboard = Social.CreateLeaderboard();
				leaderboard.id = "ColorJumpScore";
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

		audiosource = GetComponent<AudioSource> ();

	}

	void PlayBGM()
	{
		if (audiosource && backgroundMusic.Count > 0) {
			audiosource.clip = backgroundMusic [currentBGMindex];
			audiosource.Play ();
			currentBGMindex++;
			currentBGMindex =  currentBGMindex == backgroundMusic.Count ? 0 : currentBGMindex;
		}
	}

	public void StartGame()
	{
		bGameStarted = true;
		currentScore = 0;
		currentLife = 10;
		eventHandler.UpdateUILife (currentLife);

		RespawnPlayer ();

		barGen.onGameStarted ();
	
		eventHandler.onGameStarted ();

		MainCam.backgroundColor = fromCameraColor;
	}

	public void changeCameraBGColor(float playerHeight)
	{
		if(playerHeight <= 300f)
			MainCam.backgroundColor = fromCameraColor + playerHeight / 300f * (towardsCameraColor - fromCameraColor);
	}

	public void HighScoreCheck(bool result) {
		if(result)
			Debug.Log("score submission successful");
		else
			Debug.Log("score submission failed");
	}

	public void EndGame()
	{
		bGameStarted = false;
		if (currentScore > bestScore) {

			Social.ReportScore(currentScore,"ColorJumpScore",HighScoreCheck);

			bestScore = currentScore;
			mysave.bestScore = bestScore;
			GameFile.Save("save.data",mysave);
#if UNITY_IOS
			//leaderboard
			ILeaderboard leaderboard = Social.CreateLeaderboard();
			leaderboard.id = "Leaderboard1";
			leaderboard.LoadScores(result =>
			                       {
				Debug.Log("Received " + leaderboard.scores.Length + " scores");
				foreach (IScore score in leaderboard.scores)
					Debug.Log(score);
			});
#endif
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

		if (!audiosource.isPlaying)
			PlayBGM ();
	}

	void Awake(){
#if UNITY_ANDROID || UNITY_IOS
		System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#endif
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

		Time.timeScale = 1f;
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

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;
#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif
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
	public bool bGamePaused { get; private set; }

	public List<AudioClip> backgroundMusic;
	int currentBGMindex = 0;
	AudioSource audiosource;

	SaveObject mysave;

	int gameMode = 0;
	float savedTimeScale = 1f;

	public Color fromCameraColor;//218,237,226
	public Color towardsCameraColor;

	static string leaderboardId = "";
	ILeaderboard leaderboard;
	public int playerLife;

	public void login()
	{
		#if UNITY_ANDROID
		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
		/*	
		// enables saving game progress.
			.EnableSavedGames()
				// registers a callback to handle game invitations received while the game is not running.
				.WithInvitationDelegate(<callback method>)
				// registers a callback for turn based match notifications received while the
				// game is not running.
				.WithMatchDelegate(<callback method>)
				.Build();
		*/
		PlayGamesPlatform.InitializeInstance(config);
		// recommended for debugging:
		PlayGamesPlatform.DebugLogEnabled = true;
		// Activate the Google Play Games platform
		PlayGamesPlatform.Activate();
		#endif
		//leaderboard
		
		#if UNITY_IOS
		leaderboardId = "ColorJumpScore";
		#elif UNITY_ANDROID
		leaderboardId = "CgkI_ab0x7wJEAIQAA";
		#endif
		Social.localUser.Authenticate (success => {
			if (success) {
				Utils.addLog ("Authentication successful");
				string userInfo = "Username: " + Social.localUser.userName + 
					"\nUser ID: " + Social.localUser.id + 
						"\nIsUnderage: " + Social.localUser.underage;
				Utils.addLog (userInfo);
				
				leaderboard = Social.CreateLeaderboard();
				leaderboard.id = leaderboardId;
				#if UNITY_IOS
				string[] userfilterstrings = new string[1];
				userfilterstrings[0] = Social.localUser.id;
				if(userfilterstrings[0] != "")
					leaderboard.SetUserFilter(userfilterstrings);

				leaderboard.LoadScores(result =>
			     {
					Utils.addLog("Received " + leaderboard.scores.Length + " scores");
					foreach (IScore score in leaderboard.scores)
						Utils.addLog(score.ToString());

					CheckLeaderboardsScore();
				});
				#endif
				
				
			}
			else
				Utils.addLog("Authentication failed");
		});
		

	}

	void Start () {

		SetCurrentPlayerTemplateByIdx (0);
		MainCam = GameObject.Find ("Main Camera").GetComponent<Camera>();

		barGen = GetComponent<BarGenerator> ();


		if (GameFile.Load ("save.data", ref mysave))
			bestScore = mysave.bestScore;

		login ();
 

		audiosource = GetComponent<AudioSource> ();

	}

	void CheckLeaderboardsScore()
	{
#if UNITY_IOS
		if (leaderboard.id != "" && bestScore > (int)leaderboard.localUserScore.value) {
			Utils.addLog ("new score sent to leaderboard " + bestScore.ToString());
			Social.ReportScore (bestScore, leaderboardId, ScoreReported);
		} else {
			Utils.addLog ("current leaderboard socre is higher " + leaderboard.localUserScore.value.ToString());
		}
#elif UNITY_ANDROID
		if (currentScore >= bestScore) {
			Utils.addLog ("new score sent to leaderboard " + currentScore.ToString());
			Social.ReportScore (currentScore, leaderboardId, ScoreReported);
		}
#endif
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
		currentLife = playerLife;
		eventHandler.UpdateUILife (currentLife);

		RespawnPlayer ();

		barGen.onGameStarted ();
	
		eventHandler.onGameStarted ();

		MainCam.backgroundColor = fromCameraColor;
	}

	public void changeCameraBGColor(float playerHeight)
	{
		if(playerHeight <= 80f)
			MainCam.backgroundColor = fromCameraColor + playerHeight / 300f * (towardsCameraColor - fromCameraColor);
	}

	public void ScoreReported(bool result) {
		if(result)
			Utils.addLog("score submission successful");
		else
			Utils.addLog("score submission failed");
	}

	public void BackToMainMenu()
	{
		if (CurrentPlayer != null)
			GameObject.Destroy(CurrentPlayer);

		barGen.DestoryAllBarsAndPickups ();

		eventHandler.showTutorial (false);
	}

	public void EndGame()
	{
		bGameStarted = false;
		if (currentScore > bestScore) {
			CheckLeaderboardsScore();

			bestScore = currentScore;



			mysave.bestScore = bestScore;
			GameFile.Save("save.data",mysave);

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

	public void PauseGame()
	{
		savedTimeScale = Time.timeScale;
		Time.timeScale = 0;
		bGamePaused = true;
	}

	public void UnPauseGame()
	{
		Time.timeScale = savedTimeScale;
		savedTimeScale = 1f;
		bGamePaused = false;
	}
}

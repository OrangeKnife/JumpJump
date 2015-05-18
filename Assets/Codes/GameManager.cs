using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.Cloud.Analytics;
using Soomla.Store;

#if UNITY_ANDROID && !UNITY_EDITOR
using GooglePlayGames;
using GooglePlayGames.BasicApi;
#endif
public class GameManager : MonoBehaviour {
	public List<GameObject> PlayerTemplates;
	private GameObject CurrentPlayer;

	
	public List<GameObject> ForegroundFloatingObjectList;
	public List<GameObject> PickupTemplates;
	public GameObject UnlockHardcoreModePickupTemplate;

	public bool bGameStarted { get; private set; }
	public Camera MainCam { get; private set;}
	public BarGenerator barGen { get; private set;}

	GameSceneEvents eventHandler = null;
	GameObject CurrentPlayerTemplate;

	public int currentScore {get; private set;}
	public int currentLife { get; private set; }
	public int bestScore { get; private set; }
	public int bestScore_hardcore { get; private set; }
	public bool bGamePaused { get; private set; }

	public List<AudioClip> backgroundMusic;
	int currentBGMindex = 0;
	AudioSource audiosource;

	[System.NonSerialized]
	public SaveObject mysave;

	public int gameMode { get; private set;}//0 normal, 1 hard core
	float savedTimeScale = 1f;

	//public Color fromCameraColor;//218,237,226
	//public Color towardsCameraColor;

	public static string leaderboardId = "";
	public static string leaderboardId_hardcore = "";
	ILeaderboard leaderboard;
	ILeaderboard leaderboard_hardcore;
	public int playerLife,hardCoreLife;

	bool bColorIndication = true;

	public bool NoAds { get; private set; }

	ColorJumpShopEventHandler shopHandler ;

	public bool hardCoreUnlocked { get; private set; }
	public int hardCoreUnlockCount;

	//public List<Sprite> backgroundSprites;
	//public List<Sprite> backgroundSprites_hardcore;

	public List<Color> backgroundColors,backgroundColors_hardcore;

	SpriteRenderer BGSpriteRenderer;

	public List<string> notificationTextList;

	public bool readyForRecording { get; private set; }

	public bool recorded = false;
	float gameStartTime;

	public List<GameObject> SkinTemplates;
	List<GameObject> ownedSkins;
	GameObject currentSelectedSkinTemplate;
	int currentSkinTemplateIdx = 0;//random skin

	public List<int> freeTokenGiveAwayTime;

	int gameActiveTime = 0;
	float lastTimeCheckGameActiveTime = 0f;
	float lastTimeSaveGameActiveTime = 0f;
	float lastTimeDoOneSecondTick = 0f;

	int freeGiftCounterBalance;
	public void login()
	{

		//leaderboard
		
		Invoke ("DoLoginAuthenticate",0.1f);
		

	}

	void DoLoginAuthenticate()
	{
		#if UNITY_IOS && !UNITY_EDITOR
		leaderboardId = "ColorJumpScore";
		leaderboardId_hardcore = "ColorJumpScore_HardCore";
		#elif UNITY_ANDROID && !UNITY_EDITOR
		leaderboardId = "CgkI_ab0x7wJEAIQAA";
		leaderboardId_hardcore = "CgkI_ab0x7wJEAIQBw";
		#endif
		
		Social.localUser.Authenticate (success => {
			if (success) {
				Utils.addLog ("Authentication successful");
				string userInfo = "Username: " + Social.localUser.userName + 
					"\nUser ID: " + Social.localUser.id + 
						"\nIsUnderage: " + Social.localUser.underage;
				Utils.addLog (userInfo);	
			}
			else
				Utils.addLog("Authentication failed");
		});

	}

	public void SendPhoneNotification() {
#if UNITY_ANDROID && !UNITY_EDITOR
		MyLocalNotification.SendNotification(1, 3600*24, "Color Jump", "Ah! I am a Colorful Jumpy game! o(∩_∩)o", new Color32(0xff, 0x44, 0x44, 255));
		MyLocalNotification.SendNotification(2, 3600*24*3, "Color Jump", "Ah! I am a Colorful Jumpy game! o(∩_∩)o", new Color32(0xff, 0x44, 0x44, 255));
		MyLocalNotification.SendNotification(3, 3600*24*7, "Color Jump", "Ah! I am a Colorful Jumpy game! o(∩_∩)o", new Color32(0xff, 0x44, 0x44, 255));
		MyLocalNotification.SendNotification(4, 3600*24*14, "Color Jump", "Ah! I am a Colorful Jumpy game! o(∩_∩)o", new Color32(0xff, 0x44, 0x44, 255));
		MyLocalNotification.SendNotification(5, 3600*24*30, "Color Jump", "Ah! I am a Colorful Jumpy game! o(∩_∩)o", new Color32(0xff, 0x44, 0x44, 255));
#endif

#if UNITY_IOS && !UNITY_EDITOR
		//ios is damn stupid, doesn't support custom interval like android :(, hack it here 2 notifications a day
		MyLocalNotification.SendNotification(1, 3600*24, "Color Jump", "Ah! I am a Colorful Jumpy game! o(∩_∩)o", new Color32(0xff, 0x44, 0x44, 255));
		MyLocalNotification.SendNotification(2, 3600*24*3, "Color Jump", "Ah! I am a Colorful Jumpy game! o(∩_∩)o", new Color32(0xff, 0x44, 0x44, 255));
		MyLocalNotification.SendNotification(3, 3600*24*7, "Color Jump", "Ah! I am a Colorful Jumpy game! o(∩_∩)o", new Color32(0xff, 0x44, 0x44, 255));
		MyLocalNotification.SendNotification(4, 3600*24*14, "Color Jump", "Ah! I am a Colorful Jumpy game! o(∩_∩)o", new Color32(0xff, 0x44, 0x44, 255));
		MyLocalNotification.SendNotification(5, 3600*24*30, "Color Jump", "Ah! I am a Colorful Jumpy game! o(∩_∩)o", new Color32(0xff, 0x44, 0x44, 255));
#endif

	}
	void Start () {
		//inti google play

		#if UNITY_ANDROID && !UNITY_EDITOR
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


		SetCurrentPlayerTemplateByIdx (0);
		MainCam = GameObject.Find ("Main Camera").GetComponent<Camera>();
		BGSpriteRenderer = MainCam.GetComponentInChildren<SpriteRenderer>();
		barGen = GetComponent<BarGenerator> ();

		//PlayerPrefs.DeleteAll ();
		//GameFile.Save ("save.data", new SaveObject (true));

		if (GameFile.Load ("save.data", ref mysave)) {
			bestScore = mysave.bestScore;
			bestScore_hardcore = mysave.bestScore_hardcore;
			hardCoreUnlocked = mysave.unlockedHardCore;
		}

		login ();
 



		eventHandler = GameObject.Find ("eventHandler").GetComponent<GameSceneEvents>();

		if(!SoomlaStore.Initialized)
			SoomlaStore.Initialize(new ColorJumpStoreAssets());

		NoAds = StoreInventory.GetItemBalance (ColorJumpStoreAssets.NO_ADS_LTVG.ItemId) > 0;
		Utils.addLog ("balance = " + StoreInventory.GetItemBalance (ColorJumpStoreAssets.NO_ADS_LTVG.ItemId));

		if (!NoAds)
			eventHandler.InitAds ();

		shopHandler = new ColorJumpShopEventHandler ();
		shopHandler.setUpGameMgr (this);
#if UNITY_IOS && !UNITY_EDITOR
		MyLocalNotification.CancelNotification (0);
#endif
		SendPhoneNotification ();//haha

		ownedSkins = new List<GameObject> ();
		checkOwnedSkins ();

		gameActiveTime = StoreInventory.GetItemBalance (ColorJumpStoreAssets.ACCUMULATED_ACTIVETIME.ItemId);
		freeGiftCounterBalance = StoreInventory.GetItemBalance (ColorJumpStoreAssets.FREEGIFT_COUNTER.ItemId);
		lastTimeDoOneSecondTick = Time.realtimeSinceStartup;
	}

	void checkOwnedSkins()
	{
		for(int i = 1; i < SkinTemplates.Count; ++i)
		{
			PlayerSkin ps = SkinTemplates[i].GetComponent<PlayerSkin>();
			if(ps != null)
			{
				if(ps.freeToUse || ps.purchasable && ps.skinId != "" && StoreInventory.GetItemBalance (ps.skinId) > 0)
					ownedSkins.Add (SkinTemplates[i]);
			}
		}
	}

	public void RecordingStartedDelegate() {
		Utils.addLog("Recording was started");
		/* The recording is now started, show the red "REC" in the upper hand corner */
		//MyGameEngine.ShowRecordingIndicator();
	}
	
	public void RecordingStoppedDelegate() {
		Utils.addLog("Recording ended");
		/* Remove visual indicator from the user */
		//MyGameEngine.RemoveRecordingIndicator();
	}
	
	public void ThumbnailReadyAtFilePathDelegate(string path) {
		Utils.addLog("Thumbnail ready: "+path);
		//this.thumbnailPath = path;
	}

	public void onReadyForRecording(bool enabled)
	{
		if (enabled) {
			Utils.addLog("Ready for recording!");
			readyForRecording = enabled;

		}
	}

	public string getCurrentLeaderBoardId()
	{
		if (gameMode == 0)
			return leaderboardId;
		else if (gameMode == 1)
			return leaderboardId_hardcore;

		return leaderboardId;
	}

	void CheckLeaderboardsScore()
	{
 
		if(gameMode == 0)
		{
			if (currentScore >= getBestScore()) {
				Utils.addLog ("new score sent to leaderboard " + currentScore.ToString());
			 	Social.ReportScore (currentScore, leaderboardId, ScoreReported);
			}
		}
		else if(gameMode == 1)
		{
			if (currentScore >= getBestScore()) {
				Utils.addLog ("new score sent to leaderboard_hardcore" + currentScore.ToString());
				Social.ReportScore (currentScore, leaderboardId_hardcore, ScoreReported);
			}
		}
 
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

	public int getPlayerLifeByMode(int mode)
	{
		switch (mode) {
		case 0:
			return playerLife;
		case 1:
			return hardCoreLife;
		case 2:
			return 30;//???
		}

		return 3;
	}

	public void StartGame(int mode)
	{
		if(mode >= 0)
			gameMode = mode;
		/*
#if(UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
		if (gameMode == 0) {
			leaderboard = Social.CreateLeaderboard ();
			leaderboard.id = leaderboardId;
		} else if (gameMode == 1) {
			leaderboard_hardcore = Social.CreateLeaderboard();
			leaderboard_hardcore.id = leaderboardId_hardcore;
		}
#endif
*/

		eventHandler.isTitle = false;

		bGameStarted = true;
		currentScore = 0;
		currentLife = getPlayerLifeByMode(gameMode);
		eventHandler.UpdateUILife (currentLife);
		
		RespawnPlayer ();

		barGen.onGameStarted ();
	
		eventHandler.onGameStarted ();

		//MainCam.backgroundColor = fromCameraColor;

		gameStartTime = Time.time;


	}

	/*public void changeCameraBGColor(float playerHeight)
	{
		if(playerHeight <= 80f)
			MainCam.backgroundColor = fromCameraColor + playerHeight / 300f * (towardsCameraColor - fromCameraColor);
	}*/

	public void ScoreReported(bool result) {
		if(result)
			Utils.addLog("score submission successful");
		else
			Utils.addLog("score submission failed");
	}

	public void BackToMainMenu()
	{
		bGameStarted = false;

		CurrentPlayer.GetComponent<PlayerController>().allowInput = false;

		eventHandler.DoTransition(BackToMainMenuFromGame);


	}


	void BackToMainMenuFromGame()
	{
		if (CurrentPlayer != null)
			GameObject.Destroy(CurrentPlayer);
		
		 barGen.DestoryAllBarsAndPickups ();

		MainCam.GetComponent<CameraController> ().ResetCamera (null,true);
		
		eventHandler.showTutorial (false);
		eventHandler.SetPausePanel (false);
		eventHandler.SetScorePanel (false);
		eventHandler.SetDeathPanel (false);
		eventHandler.SetStartPanel (true);
		eventHandler.ShowOneOfTheBannerViews(true);

		eventHandler.isTitle = true;

	}

	void setBestScore(int score)
	{
		if (gameMode == 0) {
			bestScore = score;
			mysave.bestScore = bestScore;
		} else if (gameMode == 1) {
			bestScore_hardcore = score;
			mysave.bestScore_hardcore = bestScore_hardcore;
		}
	}

	public void EndGame()
	{
		Utils.clearLog ();

		bGameStarted = false;
		if (currentScore > getBestScore()) {
			CheckLeaderboardsScore();

			setBestScore(currentScore);

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

	void OneSecondTick ()
	{
		 
		gameActiveTime += 1;
		float currentTimeSinceStartup = Time.realtimeSinceStartup;
		if (currentTimeSinceStartup - lastTimeCheckGameActiveTime > 5)
			gameActiveTime += (int)(currentTimeSinceStartup - lastTimeCheckGameActiveTime);

		lastTimeCheckGameActiveTime = Time.realtimeSinceStartup;

		if (lastTimeCheckGameActiveTime - lastTimeSaveGameActiveTime > 30) {
			int savedGameActiveTime = StoreInventory.GetItemBalance (ColorJumpStoreAssets.ACCUMULATED_ACTIVETIME.ItemId);
			StoreInventory.GiveItem (ColorJumpStoreAssets.ACCUMULATED_ACTIVETIME.ItemId, gameActiveTime - savedGameActiveTime);//now it's the save
			lastTimeSaveGameActiveTime = lastTimeCheckGameActiveTime;
		}

		Utils.addLog ("GameManagerlog: (int)Time.deltaTime = " + (gameActiveTime).ToString ());
		 
		lastTimeDoOneSecondTick = Time.realtimeSinceStartup;
	}

	void Update () {

		if (Input.GetKeyDown(KeyCode.Escape)) 
		{
			if(eventHandler.isTitle)
				Application.Quit(); 
			else if(!bGamePaused)
				eventHandler.onPauseButtonClicked();

		}

		if (!audiosource.isPlaying)
			PlayBGM ();

		if (Time.realtimeSinceStartup - lastTimeDoOneSecondTick > 1f)
			OneSecondTick ();

	}
	

	void Awake(){
#if UNITY_ANDROID || UNITY_IOS
		System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#endif


		DontDestroyOnLoad(gameObject);

		audiosource = GetComponent<AudioSource> ();

		try{
			if(!GameFile.Load ("save.data", ref mysave))
			{
				mysave = new SaveObject(true);
				GameFile.Save("save.data",mysave);

				UnityAnalytics.CustomEvent("GameSaveDataCreated",new Dictionary<string, object>{
					{ "GameSaveData", 1 }
				} );
			}
			
		}
		catch(System.Exception)
		{
			Debug.Log ("save.data loading error");
		}
	}

	public void ApplySkinSetting()
	{
	 
		if (CurrentPlayer != null)
			CurrentPlayer.GetComponent<PlayerController> ().AttachSkin (currentSelectedSkinTemplate);
	}

	public void RespawnPlayer()
	{
		if (CurrentPlayer != null)
		{
			Destroy(CurrentPlayer);
		}

		CurrentPlayer = Instantiate(CurrentPlayerTemplate);

		if (currentSkinTemplateIdx == 0)
			UseSkin (currentSkinTemplateIdx); // do random every game if choose 0

		ApplySkinSetting ();

		MainCam.gameObject.GetComponent<CameraController> ().ResetCamera (CurrentPlayer);

		if(eventHandler)
			eventHandler.onPlayerRespawn ();

		Time.timeScale = 1f;

		if (!mysave.rated)
		{
			if(mysave.deathCount == mysave.rateLaterDeathCount) {
				eventHandler.setRateQuestionPanel(true);
			}
		}

		//BGSpriteRenderer.sprite = getRandomBG (gameMode);
		MainCam.backgroundColor = getRandomBGColor (gameMode);

	}

	public void ChangeRandomBG()
	{
		MainCam.backgroundColor = getRandomBGColor (gameMode);
	}

	/*Sprite getRandomBG(int mode)
	{
		if (mode == 0 && backgroundSprites.Count > 0)
			return backgroundSprites [Random.Range (0, backgroundSprites.Count)];
		else if(mode == 1 && backgroundSprites_hardcore.Count > 0)
			return backgroundSprites_hardcore [Random.Range (0, backgroundSprites_hardcore.Count)];

		return null;
	}*/

	Color getRandomBGColor(int mode)
	{
		if (mode == 0 && backgroundColors.Count > 0)
			return backgroundColors [Random.Range (0, backgroundColors.Count)];
		else if(mode == 1 && backgroundColors_hardcore.Count > 0)
			return backgroundColors_hardcore [Random.Range (0, backgroundColors_hardcore.Count)];
		
		return Color.white;
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
		if(savedTimeScale != 0)
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

	public void SetColorIndication(bool visiable)
	{
		if (!bColorIndication && visiable) {
			bColorIndication = visiable;
			eventHandler.SetColorIndicationPanel (true);
		} else if (bColorIndication && !visiable) {
			bColorIndication = visiable;
			eventHandler.SetColorIndicationPanel (false);
		}
	}

	public int getBestScore ()
	{
		if (gameMode == 0)
			return bestScore;
		else if (gameMode == 1)
			return bestScore_hardcore;

		return bestScore;
	}

	public void onMarketPurchase(string itemId)
	{
		switch(itemId)
		{
		case "no_ads":
				NoAds = true;
				Utils.addLog ("new NoAds = " + NoAds.ToString ());
				eventHandler.DestoryAllAds ();
			break;
		case "colorjumpId_smileface":
		case "colorjumpId_sadface":
				GameObject skinJustBought = getSkinBySkinId(itemId);
				if(skinJustBought != null)
					ownedSkins.Add(skinJustBought);
				else
					Utils.addLog("WTF, CANNOT FIND SKIN YOU JUST BOUGHT");

				eventHandler.onMarketPurchase (itemId);
				break;
		}
	}

	GameObject getSkinBySkinId(string skId)
	{
		foreach (GameObject sktemp in SkinTemplates) {
			if(sktemp.GetComponent<PlayerSkin>().skinId == skId)
				return sktemp;
		}

		return null;
	}

	public void onRestoreTransactionsStarted() {
		eventHandler.onRestoreTransactionsStarted ();
	}

	public void onRestoreTransactionsFinished(bool success) {
		eventHandler.onRestoreTransactionsFinished (success);
	}

	public void onPurchaseStarted(string itemId)
	{
		eventHandler.onPurchaseStarted (itemId);
	}
	

	public void onPurchaseCancelled(string itemId)
	{
		eventHandler.onPurchaseCancelled(itemId);
	}

	public void RemoveAds()
	{
		try{
			StoreInventory.BuyItem(ColorJumpStoreAssets.NO_ADS_LTVG.ItemId);
		} catch (System.Exception e) {
			Utils.addLog ("SOOMLA/UNITY " + e.Message);
		}
	}

	public void BuySkin(string soomlaId)
	{
		try{
			StoreInventory.BuyItem(soomlaId);
		} catch (System.Exception e) {
			Utils.addLog ("SOOMLA/UNITY " + e.Message);
		}
	}

	GameObject getRandomUseableSkin()
	{
		GameObject rtObj = null;
		int RandomNum = 0;
		Random.seed = (int)Time.time;
		while (rtObj == null)
		{
			RandomNum = Random.Range(0,ownedSkins.Count);//at least you will get no skin in owned skins
			rtObj = ownedSkins[RandomNum];
		}
		return rtObj;
	}

	public void UseSkin(int skinTemplateIdx)
	{
		currentSkinTemplateIdx = skinTemplateIdx;
		if (skinTemplateIdx == 0) //force random a skin
		{
			currentSelectedSkinTemplate = getRandomUseableSkin();
		}
		else if (skinTemplateIdx == 1)//force no skin
			currentSelectedSkinTemplate = null;
		else 
			currentSelectedSkinTemplate = SkinTemplates [skinTemplateIdx];
	}

	public void AddDeathCount(int d)
	{
		mysave.deathCount += d;
		GameFile.Save ("save.data",mysave);
	}

	public void unlockHardcoreMode(bool wantToUnlock)
	{
		hardCoreUnlocked = wantToUnlock;
		mysave.unlockedHardCore = hardCoreUnlocked;
		GameFile.Save ("save.data",mysave);
	}

	public void ratedGame()
	{
		mysave.rated = true;
		GameFile.Save ("save.data",mysave);
	}

	public void rateLater()
	{
		mysave.rateLaterDeathCount *= 5;
		GameFile.Save ("save.data",mysave);
	}

	public void PlayLastMoment()
	{
	}

	public float getPlayTime()
	{
		return Time.time - gameStartTime;
	}

	public int AddFreeGiftToken(int num = 1)
	{
		StoreInventory.GiveItem (ColorJumpStoreAssets.ONE_FREEGIFT_TOKEN.ItemId,num);
		StoreInventory.GiveItem (ColorJumpStoreAssets.FREEGIFT_COUNTER.ItemId,1);
		freeGiftCounterBalance += 1;
		
		int savedGameActiveTime = StoreInventory.GetItemBalance(ColorJumpStoreAssets.ACCUMULATED_ACTIVETIME.ItemId);
		StoreInventory.TakeItem(ColorJumpStoreAssets.ACCUMULATED_ACTIVETIME.ItemId,savedGameActiveTime);//now it's the save
		gameActiveTime = 0;

		return num;
	}
	
	public int GetTokenNum()
	{
		return StoreInventory.GetItemBalance (ColorJumpStoreAssets.FREEGIFT_TOKEN_ITEM_ID);
	}

	public bool consumeToken(int i)
	{
		if (i > GetTokenNum ())
			return false;

		StoreInventory.TakeItem (ColorJumpStoreAssets.FREEGIFT_TOKEN_ITEM_ID, i);
		return true;
	}

	public PlayerSkin GivePlayerRandomSkin()
	{
		GameObject rtObj = null;
		int RandomNum = 1;
		Random.seed = (int)Time.time;
		 
		RandomNum = Random.Range(1,SkinTemplates.Count);//no skin to the last skin
		rtObj = SkinTemplates[RandomNum];
		 
 		if (ownedSkins.IndexOf (rtObj) == -1) {
			StoreInventory.GiveItem(rtObj.GetComponent<PlayerSkin>().skinId,1);
			ownedSkins.Add(rtObj);
			return rtObj.GetComponent<PlayerSkin> ();
		}
		else
			return null;//nothing haha
	}

	public List<GameObject> getOwnedSkins()
	{
		return ownedSkins;
	}

	public int getNextTimeFreeTokenSeconds()
	{
		if (freeGiftCounterBalance < 5)
			return freeTokenGiveAwayTime [freeGiftCounterBalance];
		else
			return freeTokenGiveAwayTime [4];
	}

	public int getFreeGiftGiveAwayTimeLeft()
	{	
		int nextTimeGiveAwayTokens = getNextTimeFreeTokenSeconds ();
		if (nextTimeGiveAwayTokens == -1)
			return nextTimeGiveAwayTokens;
		else 
			return Mathf.Max(0,nextTimeGiveAwayTokens - gameActiveTime);
	}

}

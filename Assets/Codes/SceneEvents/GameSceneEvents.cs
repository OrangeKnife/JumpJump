using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Advertisements;
using System.IO;

using GoogleMobileAds.Api;
#if UNITY_ANDROID
using GooglePlayGames;
#endif
#if UNITY_IOS && !UNITY_EDITOR
using UnityEngine.SocialPlatforms.GameCenter;
#endif
public class GameSceneEvents : MonoBehaviour {

	//[SerializeField]
	//GameObject yellowboardsButton = null;
	//[SerializeField]
	//GameObject UI_SmallLeaderBoardsPanel = null;

	[SerializeField]
	UnityEngine.UI.Text AutoMessageText = null;
	[SerializeField]
	GameObject UI_AutoMessage  = null;
	[SerializeField]
	GameObject UI_RateQuestion = null;
	[SerializeField]
	GameObject UI_UnityAdsQuestion = null;
	[SerializeField]
	GameObject UI_PausePanel = null;
	[SerializeField]
	GameObject UI_DeathPanel = null;
	[SerializeField]
	GameObject UI_ScorePanel = null;
	[SerializeField]
	GameObject UI_ScoreText = null;
	[SerializeField]
	GameObject UI_StartPanel = null;
	[SerializeField]
	GameObject UI_LifeText = null;

	[SerializeField]
	UnityEngine.UI.Text yourScore = null; 
	[SerializeField]
	UnityEngine.UI.Text yourBest = null; 

	[SerializeField]
	GameObject tutorialLeft = null; 
	[SerializeField]
	GameObject tutorialRight = null; 

	[SerializeField]
	UnityEngine.UI.Image color1 = null; 
	[SerializeField]
	UnityEngine.UI.Image color2 = null; 
	[SerializeField]
	UnityEngine.UI.Image color3 = null; 
	[SerializeField]
	UnityEngine.UI.Image color4 = null; 
	[SerializeField]
	GameObject DimImage = null; 


	[SerializeField]
	GameObject RemoveAdsButton = null; 

	[SerializeField]
	GameObject UI_ColorIndicationPanel = null; 
	[SerializeField]
	GameObject UI_NoColorIndicationText = null;

	[SerializeField]
	UnityEngine.UI.Text ExtraInfoText = null;
	[SerializeField]
	UnityEngine.UI.Text JumpCountText = null;
	[SerializeField]
	UnityEngine.UI.Text FloorText = null;

	[SerializeField]
	UnityEngine.UI.Text UnityAdsYesNumText = null;

	[SerializeField]
	GameObject EndOfGameObj = null;
	[SerializeField]
	UnityEngine.UI.Image ScreenShotImg = null;

	int UnityAdsYesNum;


	GameObject Player;


	GameManager gameMgr;
	SaveObject mysave;
	


	BannerView bannerView,bannerViewBottom;
	AdRequest request;

	[SerializeField]
	GameObject transitionImg = null; 

	public AudioClip menuClickedSound;

	
	AudioSource audioSource;
	Sprite screenShotSprite;
	string ScreenShotPath;

	public void DestoryAllAds()
	{
		if (bannerView != null)
			bannerView.Destroy ();

		if (bannerViewBottom != null)
			bannerViewBottom.Destroy ();

		RemoveAdsButton.SetActive (false);
	}

	public void InitAds()
	{
		RemoveAdsButton.SetActive (true);
		//init ads
		string bannerAdsId = "";
		#if UNITY_IOS && !UNITY_EDITOR
		bannerAdsId = "ca-app-pub-7183026460514946/4970464516";
		#endif
		#if UNITY_ANDROID && !UNITY_EDITOR
		bannerAdsId = "ca-app-pub-7183026460514946/9400664114";
		#endif
		
		if (bannerView == null) {
			bannerView = new BannerView (
				bannerAdsId, AdSize.SmartBanner, AdPosition.Top);
			
			
			
			
			
			bannerViewBottom = new BannerView (
				bannerAdsId, AdSize.SmartBanner, AdPosition.Bottom);
			
			ShowOneOfTheBannerViews (true);
			
		}
	}

	void Start () {

		audioSource = GetComponent<AudioSource> ();


		if (gameMgr == null)
			InitGameMgr ();

		StartCoroutine(initUnityAds());
	}

	public void playMenuClickedSound()
	{
		audioSource.clip = menuClickedSound;
		audioSource.Play ();
	}

	IEnumerator initUnityAds()
	{
		if (Advertisement.isSupported && Application.internetReachability != NetworkReachability.NotReachable )
		{
			Advertisement.allowPrecache = true;
			string UnityAdsId="";
			#if UNITY_IOS && !UNITY_EDITOR
			UnityAdsId = "37628";
			#endif
			#if UNITY_ANDROID && !UNITY_EDITOR
			UnityAdsId = "37626";
			#endif
			

			Advertisement.Initialize(UnityAdsId);
			Utils.addLog("UnityAds initialized = " + Advertisement.isInitialized);
		} else {
			Utils.addLog("no internet or platform not supported");
		}


		yield return new WaitForSeconds(2f);


		if (!Advertisement.isInitialized)
			StartCoroutine (initUnityAds());
	}

	void requestNewBannerAds(BannerView aBannerView)
	{
		request = new AdRequest.Builder ().Build ();
		// Load the banner with the request.
		aBannerView.LoadAd (request);
	}

	public void ShowOneOfTheBannerViews(bool forceTop = false)
	{
		HideAllBannerViews();

		if (UnityEngine.Random.Range (0, 2) == 0 || forceTop) {
			if(bannerView != null)
			{
				requestNewBannerAds (bannerView);
				bannerView.Show ();
			}
		} else {
			if(bannerViewBottom != null)
			{
				requestNewBannerAds (bannerViewBottom);
				bannerViewBottom.Show ();
			}
		}
	}

	void HideAllBannerViews()
	{
		if(bannerView != null)
			bannerView.Hide ();
		if(bannerViewBottom != null)
			bannerViewBottom.Hide ();
	}
		
	void Awake() {





		try{

			if(!GameFile.Load ("save.data", ref mysave))
			{
				mysave = new SaveObject(true);
				GameFile.Save("save.data",mysave);
			}

		}
		catch(System.Exception)
		{
			Debug.Log ("save.data loading error");
		}
	}

	void InitGameMgr()
	{
		gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();

	}

	public void onPlayerDead() 
	{
	}

	void SetEndOfGameMark(bool bActive)
	{
		EndOfGameObj.SetActive (bActive);

		if (bActive) {
			int maxBarNum = gameMgr.GetCurrentPlayer ().GetComponent<PlayerController> ().maxBarNum;
			 
				string EOGText = "<color=#FF00FFFF>FLOOR.</color>  " + maxBarNum.ToString () + "\n<color=#FF00FFFF>SCORE.</color>  " + gameMgr.currentScore.ToString () + 
				"\n<color=#FF00FFFF>JUMP.</color>  " + (gameMgr.GetCurrentPlayer().GetComponent<PlayerController>().totalJumpCount).ToString () +
				"\n<color=#FF00FFFF>TIME.</color>  " + ((int)gameMgr.getPlayTime ()).ToString ();
			 
				EndOfGameObj.GetComponentInChildren<TextMesh> ().text = EOGText;
				EndOfGameObj.transform.position = new Vector3(0,gameMgr.MainCam.transform.position.y,-1);//lastBar.gameObject.transform.position;
				EndOfGameObj.GetComponent<Animator>().Play("ScalePopInAnimation");
				EndOfGameObj.GetComponent<AudioSource>().Play();

			}
 



	}

	public void onPlayerRespawn()
	{
		UpdateUISocre (0);
		showTutorial (true);
		SetColorIndicationPanel (true);//auto show
	}

	void TakeAScreenShotAndShowDeathPanel()
	{
		StartCoroutine(DoTakingScreenShot());
	}

	void ShowDeathPanel()
	{	
		SetDeathPanel (true);
		yourScore.text = gameMgr.currentScore.ToString ();
		yourBest.text = gameMgr.getBestScore ().ToString();

		ShowOneOfTheBannerViews ();

	}

	 
	

	
	public void OnTryAgainButtonClicked()
	{
		playMenuClickedSound ();
		
		HideAllBannerViews ();
		
		SetDeathPanel (false);
		
		gameMgr.StartGame (-1);
		
		
		
		
	}
	
	public void UpdateUISocre(int newScore)
	{
		UI_ScoreText.GetComponent<UnityEngine.UI.Text>().text = newScore.ToString();
	}

	public void UpdateUILife(int newLife)
	{
		UI_LifeText.GetComponent<UnityEngine.UI.Text> ().text = newLife.ToString ();
	}




	public void DoTransition(TransitionController.myTransitionDelegate func)
	{
		TransitionController tc = transitionImg.GetComponent<TransitionController> ();
		if (!tc.doingTransition) {
			tc.startTransition();
			tc.transitionDelegates += func;		
		}
	}



	public void ShowAds()
	{
		/*
		if(Advertisement.isReady())
		{ 
			UI_ScoreText.GetComponent<UnityEngine.UI.Text>().text = "ready";

			Advertisement.Show(null, new ShowOptions{ pause = true,
				resultCallback = ShowResult =>{
				 //do something
				}
			});
		}
		else
		{

			UI_ScoreText.GetComponent<UnityEngine.UI.Text>().text = "not ready";
		}
*/
	}

	public void onGameStarted()
	{
		UI_ScorePanel.SetActive (true);
		UpdateUISocre (gameMgr.currentScore);
	}

	public void onGameEnded()
	{
		SetEndOfGameMark (true);
		Invoke ("TakeAScreenShotAndShowDeathPanel", 2f);
	}

	public void addLog(string logstring)
	{
		GameObject.Find ("LogText").GetComponent<UnityEngine.UI.Text> ().text += logstring;
	}

	public void showTutorial(bool wantToShow)
	{
		tutorialLeft.SetActive(wantToShow);
		tutorialRight.SetActive(wantToShow);
	}

	public void onPlayerColorChanged(EObjectColor currentColor)
	{
		color1.color = PlayerController.getColorBuyColorEnum (currentColor);
		
		EObjectColor next = currentColor;
		next = next + 1;
		next = next == EObjectColor.MAXCOLORNUM ? 0 : next;
		color2.color = PlayerController.getColorBuyColorEnum (next);
		next = next + 1;
		next = next == EObjectColor.MAXCOLORNUM ? 0 : next;
		color3.color = PlayerController.getColorBuyColorEnum (next);
		next = next + 1;
		next = next == EObjectColor.MAXCOLORNUM ? 0 : next;
		color4.color = PlayerController.getColorBuyColorEnum (next);

		//lifeImg.color = currentColor;
	}

	public void onLeaderboardsButtonCilicked()
	{
		if (!Social.localUser.authenticated) {
			Utils.addLog("authenticated = " + Social.localUser.authenticated.ToString());
			gameMgr.login ();
		}
		#if UNITY_ANDROID && !UNITY_EDITOR
		Social.ShowLeaderboardUI();
		#elif UNITY_IOS && !UNITY_EDITOR
		GameCenterPlatform.ShowLeaderboardUI(gameMgr.getCurrentLeaderBoardId(),UnityEngine.SocialPlatforms.TimeScope.AllTime);
		#endif

	}

	public void onLeaderboardsButton_mainMenu_Clicked()
	{
		playMenuClickedSound ();
		if (!Social.localUser.authenticated) {
			Utils.addLog("authenticated = " + Social.localUser.authenticated.ToString());
			gameMgr.login ();
		}
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		Social.ShowLeaderboardUI();
		#elif UNITY_IOS && !UNITY_EDITOR
		GameCenterPlatform.ShowLeaderboardUI("",UnityEngine.SocialPlatforms.TimeScope.AllTime);

		//UI_SmallLeaderBoardsPanel.SetActive (true);
		//yellowboardsButton.SetActive (false);
		#endif


	}

	public void onLeaderboardButton_normalClicked()
	{
		playMenuClickedSound ();
		#if UNITY_IOS && !UNITY_EDITOR
		if (Social.localUser.authenticated)
			GameCenterPlatform.ShowLeaderboardUI(GameManager.leaderboardId,UnityEngine.SocialPlatforms.TimeScope.AllTime);
		#endif
	}

	public void onLeaderboardButton_hardlClicked()
	{
		playMenuClickedSound ();
		#if UNITY_IOS && !UNITY_EDITOR
		if (Social.localUser.authenticated)
			GameCenterPlatform.ShowLeaderboardUI(GameManager.leaderboardId_hardcore,UnityEngine.SocialPlatforms.TimeScope.AllTime);
		#endif
	}

	public void onPauseButtonClicked()
	{
		playMenuClickedSound ();
		SetPausePanel (true);

		ShowOneOfTheBannerViews();
		gameMgr.PauseGame ();

	}

	public void onRestartButtonClicked()
	{
		playMenuClickedSound ();
		onResumebuttonClicked ();
		OnTryAgainButtonClicked ();
	}

	public void onResumebuttonClicked()
	{
		playMenuClickedSound ();
		SetPausePanel (false);
		UI_ScorePanel.SetActive (true);
		HideAllBannerViews();
		gameMgr.UnPauseGame ();
	}

	public void onBackButtonClicked()
	{
		//back to main menu
		playMenuClickedSound ();
		gameMgr.BackToMainMenu ();


		gameMgr.UnPauseGame ();
	}

	public void onNoAdsButtonClicked()
	{
	/*	#if UNITY_EDITOR
		PlayerPrefs.DeleteAll ();
		#endif*/
		playMenuClickedSound ();
		gameMgr.RemoveAds ();

	}

	public void onRateButtonClicked()
	{
		playMenuClickedSound ();
		gameMgr.ratedGame ();
		Utils.rateGame ();
	}

	public void onHardcoreClicked()
	{
		playMenuClickedSound ();


		if(gameMgr.hardCoreUnlocked)
			DoTransition (DoHardCoreButton);
		else
			ShowAutoMessage("HARDCORE  MODE\n  CAN  BE  UNLOCKED\n  BY  BEATING THE\n NORMAL MODE");
	}

	public void OnStartButtonClicked()
	{
		playMenuClickedSound ();
		DoTransition (DoStartButton);
	}

	void DoHardCoreButton()
	{
		UI_StartPanel.SetActive (false);
		
		gameMgr.StartGame (1);
		
		HideAllBannerViews ();
	}
	
	void DoStartButton()
	{
		UI_StartPanel.SetActive (false);
		
		gameMgr.StartGame (0);
		
		HideAllBannerViews ();
		
		
		
	}

	public void SetPausePanel(bool bActive)
	{
		UI_PausePanel.SetActive (bActive);
		SetDimImage (bActive);

		if(bActive)
			UI_PausePanel.GetComponent<Animator> ().Play ("PausePanelOpened");
	}

	public void SetScorePanel(bool bActive)
	{
		UI_ScorePanel.SetActive (bActive);
	}

	public void SetDeathPanel(bool bActive)
	{
		UI_DeathPanel.SetActive (bActive);
		SetDimImage (bActive);
		if(bActive)
			UI_DeathPanel.GetComponent<Animator> ().Play ("GenericMenuOpenedAnimation");
	}

	public void SetStartPanel(bool bActive)
	{
		UI_StartPanel.SetActive (bActive);
	}

	public void SetColorIndicationPanel(bool bActive)
	{
		UI_ColorIndicationPanel.SetActive (bActive);
		UI_NoColorIndicationText.SetActive (!bActive);
	}

	public void SetExtraInfoText(string ExtraInfoStr)
	{
		ExtraInfoText.text = ExtraInfoStr;
	}

	public void SetJumpCountText(string jcountStr)
	{
		JumpCountText.text = jcountStr;
	}

	public void SetFloorText(string floorTextStr)
	{
		FloorText.text = floorTextStr;
	}
	
	public bool IsUnityAdsReady()
	{
		return Advertisement.isReady ();
	}

	public void SetUnityAdsQuestion(bool bActive)
	{
		UI_UnityAdsQuestion.SetActive (bActive);
		SetDimImage (bActive);
	}

	public void onAdsQuestionPopup()
	{
		if (Advertisement.isReady ()) {
			SetUnityAdsQuestion (true);
			SetDimImage (true);
			UI_UnityAdsQuestion.GetComponent<Animator> ().Play ("GenericMenuOpenedAnimation");

			UnityAdsYesNum = 10;
			UnityAdsYesNumText.GetComponent<Animator> ().Play ("FlashingTextOneSecondAnimation");
			TickingUnityAdsYesButton ();
		} else {
			Utils.addLog("ads not ready!");
			NoAdsContinueDie();
		}
	}

	public void UnityAdsYesButtonClicked()
	{
#if UNITY_EDITOR
		gameMgr.GetCurrentPlayer ().GetComponent<PlayerController> ().AfterWatchAds();
		SetUnityAdsQuestion (false);
		return;
#elif UNITY_ANDROID || UNITY_IOS

		CancelInvoke ("TickingUnityAdsYesButton");
		//UnityADS
		if(Advertisement.isReady())
		{ 
			ShowOptions options = new ShowOptions();
			options.pause = true;                        // Pauses game while ads are shown
			options.resultCallback = HandleShowResult;   // Triggered when the ad is closed
			Advertisement.Show(null,options);
		}
		else
		{
			SetUnityAdsQuestion (false);
			gameMgr.GetCurrentPlayer ().GetComponent<PlayerController> ().DoDeath ();
		 
		}
#endif
	}

	public void HandleShowResult (ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			gameMgr.GetCurrentPlayer ().GetComponent<PlayerController> ().AfterWatchAds();
			Utils.addLog("The ad was successfully shown.");
			break;
		case ShowResult.Skipped:
			gameMgr.GetCurrentPlayer ().GetComponent<PlayerController> ().AfterWatchAds();
			Utils.addLog("The ad was skipped before reaching the end.");
			break;
		case ShowResult.Failed:
			SetUnityAdsQuestion (false);
			gameMgr.GetCurrentPlayer ().GetComponent<PlayerController> ().DoDeath ();
			Utils.addLog("The ad failed to be shown.");
			break;
		}

		SetUnityAdsQuestion (false);
	}
	
	public void UnityAdsNoButtonClicked()
	{
		playMenuClickedSound ();
		NoAdsContinueDie ();
	}

	void NoAdsContinueDie()
	{
		CancelInvoke ("TickingUnityAdsYesButton");
		SetUnityAdsQuestion (false);
		gameMgr.GetCurrentPlayer ().GetComponent<PlayerController> ().DoDeath ();
	}

	void TickingUnityAdsYesButton()
	{
		//playMenuClickedSound ();
		UnityAdsYesNumText.text = (UnityAdsYesNum).ToString();
		UnityAdsYesNum -= 1;

		if (UnityAdsYesNum >= 0)
			Invoke ("TickingUnityAdsYesButton", 1f);
		else {
			SetUnityAdsQuestion (false);
			gameMgr.GetCurrentPlayer ().GetComponent<PlayerController> ().DoDeath ();
		}
	}

	public void onRateQuestionYesClicked()
	{
		onRateButtonClicked ();
		setRateQuestionPanel (false);
		gameMgr.UnPauseGame ();
	}

	public void onRateQuestionNoClicked()
	{
		playMenuClickedSound ();
		setRateQuestionPanel (false);
		gameMgr.rateLater ();
		gameMgr.UnPauseGame ();
	}

	public void setRateQuestionPanel(bool bActive)
	{
		UI_RateQuestion.SetActive (bActive);
		SetDimImage (bActive);

		if (bActive) {
			gameMgr.PauseGame ();
			UI_RateQuestion.GetComponent<Animator> ().Play ("GenericMenuOpenedAnimation");
		}



	}

	public void RemoveLocalSave()
	{
		GameFile.Save("save.data", new SaveObject(true));
	}

	public void ShowAutoMessage(string message)
	{
		bool bActive = message.Length > 0;
		UI_AutoMessage.SetActive (bActive);
		SetDimImage (bActive);
		if(bActive)
		{
			UI_AutoMessage.GetComponent<Animator> ().Play ("GenericMenuOpenedAnimation");
			AutoMessageText.text = message;
		}
	}

	public void onCreditButtonClicked()
	{
		playMenuClickedSound ();
		ShowAutoMessage("DEVELOPED  BY\n JUNSHENG YAO\n MUSIC  BY\n SHADY DAVE");
	}

	public void ToggleDebug()
	{
		Utils.bDebug = !Utils.bDebug;
	}

	public void onAutoMessageOKButtonClicked()
	{
		playMenuClickedSound ();
		UI_AutoMessage.SetActive (false);
		SetDimImage (false);
	}

	public void SetDimImage(bool bActive)
	{
		DimImage.SetActive (bActive);
	}

	public void OnShareButtonClicked()
	{
		ShareScreenshot ();
	}

	IEnumerator DoTakingScreenShot()
	{
		yield return new WaitForEndOfFrame();


		// create the texture
		Texture2D aTex = new Texture2D(Screen.width, Screen.height,TextureFormat.RGB24,true);
		aTex.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height),0,0);
		aTex.Apply();
		/*
		RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 24);
		gameMgr.MainCam.targetTexture = rt;
		Texture2D aTex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		gameMgr.MainCam.Render();
		RenderTexture.active = rt;
		aTex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		aTex.Apply ();
		gameMgr.MainCam.targetTexture = null;
		RenderTexture.active = null;
		Destroy(rt);
		*/
		byte[] dataToSave = aTex.EncodeToPNG();
		ScreenShotPath = Path.Combine(Application.persistentDataPath,System.DateTime.Now.ToString("yyyy-MM-dd-HH") + ".png");
		File.WriteAllBytes(ScreenShotPath, dataToSave);
		
		screenShotSprite = Sprite.Create(aTex,new Rect(0,0,Screen.width,Screen.height),new Vector2(0,0));
		ScreenShotImg.sprite = screenShotSprite;
		
		SetEndOfGameMark (false);
		UI_ScorePanel.SetActive (false);//gameended

		yield return new WaitForSeconds(0.5f);
		ShowDeathPanel ();
	}

	void ShareScreenshot()
	{
#if UNITY_ANDROID && !UNITY_EDITOR  

		// block to open the file and share it ------------START
		AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
		AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
		intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
		AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
		AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse","file://" + ScreenShotPath);
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), "Check out this colorful jumpy game!");
		intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), "Color Jump");
		intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
		AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
		
		// option one WITHOUT chooser:
		currentActivity.Call("startActivity", intentObject);
		
		// option two WITH chooser:
		//AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "HEY! WANNA SHARE?");
		//currentActivity.Call("startActivity", jChooser);
		
		// block to open the file and share it ------------END
			
		 
#endif 	
		#if UNITY_IOS && !UNITY_EDITOR
		 MyiOSSharing.iOSShareImgAndMessage(ScreenShotPath,"Check out this colorful jumpy game!");
		#endif 
	}

}

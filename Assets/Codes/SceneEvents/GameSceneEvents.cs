using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Advertisements;
using System.IO;
using UnityEngine.Cloud.Analytics;
using GoogleMobileAds.Api;
using Soomla.Store;

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
	GameObject FreeTokenButton = null;
	[SerializeField]
	GameObject RecordVideoButton = null;
	[SerializeField]
	GameObject RecordVideoButtonOnPausePanel = null;
	[SerializeField]
	Sprite noGiftImg = null;
	[SerializeField]
	Sprite giftTokenImg = null;
	[SerializeField]
	GameObject shopPreviousButton = null;
	[SerializeField]
	GameObject shopNextButton = null;
	[SerializeField]
	UnityEngine.UI.Text FreeTokenInfoText = null;
	[SerializeField]
	UnityEngine.UI.Text FreeTokensIndicationText = null;
	[SerializeField]
	UnityEngine.UI.Text MyTokenBalanceText = null;
	[SerializeField]
	UnityEngine.UI.Image autoMessageImg = null;
	[SerializeField]
	UnityEngine.UI.Image autoMessageImgBG = null; 
	[SerializeField]
	GameObject GiftImage = null;
	[SerializeField]
	UnityEngine.UI.Text OpenGiftBoxButtonText = null;
	[SerializeField]
	GameObject UI_GiftPanel = null;
	[SerializeField]
	GameObject ExtraButton = null;
	/*[SerializeField]
	GameObject ScorePanelShopButton = null;*/
	[SerializeField]
	GameObject ScorePanelGiftButtonTextObj = null;
	[SerializeField]
	GameObject GiftButtonTextObj = null;
	[SerializeField]
	UnityEngine.UI.Text purchaseButtonText = null;
	[SerializeField]
	GameObject currentShopItemPriceBG = null;
	[SerializeField]
	UnityEngine.UI.Text currentShopItemPriceText = null;
	[SerializeField]
	UnityEngine.UI.Text currentShopItemName = null;
	[SerializeField]
	UnityEngine.UI.Image currentShopItemImage = null;
	[SerializeField]
	GameObject UI_ShopPanel = null;
	[SerializeField]
	UnityEngine.UI.Text CountingText = null;
	[SerializeField]
	GameObject UI_CountingPanel = null;
	[SerializeField]
	GameObject countingPanelCancelButton = null;
	[SerializeField]
	UnityEngine.UI.Text SwitchJumpLeftRightText = null;
	[SerializeField]
	GameObject UI_OptionPanel = null;
	[SerializeField]
	UnityEngine.UI.Text AutoMessageText = null;
	[SerializeField]
	GameObject AutoMessageOKButton = null;
	[SerializeField]
	GameObject UI_AutoMessage  = null;
	[SerializeField]
	GameObject UI_CreditPanel  = null;
	[SerializeField]
	GameObject UI_RateQuestion = null;
	[SerializeField]
	GameObject UI_FreeSkinTrialQuestion = null;
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
	GameObject DimImageForAutoMessage = null; 


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
	[SerializeField]
	GameObject ScreenShotFrame = null;


	[SerializeField]
	GameObject RecordButton = null;

	int UnityAdsYesNum;


	GameObject Player;


	GameManager gameMgr;


	BannerView bannerView,bannerViewBottom;
	AdRequest request;

	[SerializeField]
	GameObject transitionImg = null; 

	public AudioClip menuClickedSound,screenShotSound,menuButtonDownSound,giveFreeTokenSound,noGiftSound,openGiftSound;

	
	AudioSource audioSource;
	Sprite screenShotSprite;
	string ScreenShotPath;

	bool bPauseButtonDisabled = false;
	float countingTime;//counting Time

	public delegate void AutoMessageOKButtonDelegate();
	AutoMessageOKButtonDelegate currentMessageOkButtonDelegate;

	IEnumerator TickingCountCoroutine;
	bool bDidRecordingVideo = false;

	public bool isTitle = true;

	int currentShopItemDisplayIndex = 0;

	string currentShopItemId;
	IEnumerator ButtonHoldLoopCoroutine;

	float lastTimePlayTitleJumpAnim;
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
			bannerView = new BannerView (bannerAdsId, AdSize.SmartBanner, AdPosition.Top);
		}

		if (bannerViewBottom == null) {
			bannerViewBottom = new BannerView (bannerAdsId, AdSize.SmartBanner, AdPosition.Bottom);
		}	

		ShowOneOfTheBannerViews (true);
			

	}

	void Start () {

		audioSource = GetComponent<AudioSource> ();


		if (gameMgr == null)
			InitGameMgr ();

		StartCoroutine(initUnityAds());

		if(Everyplay.IsSupported())
			Everyplay.ReadyForRecording += OnReadyForRecording;

		UI_StartPanel.GetComponent<Animator>().Play("TitleSlideAnimation");
	}

	void Update(){
		if (UI_StartPanel.activeSelf && Time.realtimeSinceStartup - lastTimePlayTitleJumpAnim > 10f) {
			lastTimePlayTitleJumpAnim = Time.realtimeSinceStartup;
			UI_StartPanel.GetComponent<Animator> ().Play ("TitleJumpAnimation");
		}
	}

	public void playGiveFreeTokenSound()
	{
		audioSource.volume = 0.4f;
		audioSource.clip = giveFreeTokenSound;
		audioSource.Play ();
	}

	public void playNoGiftSound()
	{
		audioSource.volume = 0.4f;
		audioSource.clip = noGiftSound;
		audioSource.Play ();
	}

	public void playOpenGiftSound()
	{
		audioSource.volume = 1f;
		audioSource.clip = openGiftSound;
		audioSource.Play ();
	}

	public void playMenuClickedSound()
	{
		audioSource.volume = 0.07f;
		audioSource.clip = menuClickedSound;
		audioSource.Play ();
	}

	public void playMenuButtonDownSound()
	{
		audioSource.volume = 0.07f;
		audioSource.clip = menuButtonDownSound;
		audioSource.Play ();
	}


	public void playScreenShotSound()
	{
		gameMgr.GetCurrentPlayer ().GetComponent<PlayerController> ().PlayScreenFlash ("ScreenShotFlash");

		audioSource.volume = 0.3f;
		audioSource.clip = screenShotSound;
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
			 
			string EOGText = "<color=#19AB21>FLOOR.</color>  " + maxBarNum.ToString () + "\n<color=#19AB21>SCORE.</color>  " + gameMgr.currentScore.ToString () + 
				"\n<color=#19AB21>JUMP.</color>  " + (gameMgr.GetCurrentPlayer().GetComponent<PlayerController>().totalJumpCount).ToString () +
					"\n<color=#19AB21>TIME.</color>  " + ((int)gameMgr.getPlayTime ()).ToString ();
			 
				EndOfGameObj.GetComponentInChildren<TextMesh> ().text = EOGText;
				EndOfGameObj.transform.position = new Vector3(0,gameMgr.MainCam.transform.position.y + 0.92f,-1);//hack 0.92f offset for skin display//lastBar.gameObject.transform.position;
				EndOfGameObj.GetComponent<Animator>().Play("ScalePopInAnimation");
				EndOfGameObj.GetComponent<AudioSource>().Play();

			PlayerController pc = gameMgr.GetCurrentPlayer().GetComponent<PlayerController>();
			pc.SetGravityScale(0);
			pc.SetPlayerRenderer(true);
			pc.GetComponent<PlayerController>().StopCurrentAnim();
			pc.gameObject.transform.localScale *= 2;
			pc.gameObject.transform.position = new Vector3(0,gameMgr.MainCam.transform.position.y -1.5f,-1);


			UnityAnalytics.CustomEvent("GameEnded",new Dictionary<string, object>{
				{ "FLOOR", maxBarNum },
				{ "SCORE", gameMgr.currentScore },
				{ "TIME", (int)gameMgr.getPlayTime () }
			} );
		
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


		FreeTokensIndicationText.enabled = gameMgr.getFreeTokenGiveAwayTime () == 0;


	}

	 
	public void SetPauseButton(bool bActive)
	{
		bPauseButtonDisabled = !bActive;
	}

	
	public void OnTryAgainButtonClicked()
	{
		//playMenuClickedSound ();
		
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
		SetScorePanel (true);
		UpdateUISocre (gameMgr.currentScore);

		bool isTokenReadynow = gameMgr.IsFreeTokenReady ();
		if (gameMgr.getOwnedSkins ().Count > 1 || gameMgr.GetTokenNum () > 0 || isTokenReadynow) {
			UI_ScorePanel.GetComponent<Animator> ().Play ("ScorePanelShopAndGiftSlideIn");
		}
	}

	public void onGameEnded()
	{
		SetEndOfGameMark (true);
		Invoke ("playScreenShotSound",1.2f);
		Invoke ("TakeAScreenShotAndShowDeathPanel", 2f);
	}


	public void showTutorial(bool wantToShow)
	{

		tutorialLeft.GetComponent<TextMesh>().text = "TAP LEFT\n" +( gameMgr.mysave.currentJumpType == 0 ? "JUMP" : "COLOR");
		tutorialRight.GetComponent<TextMesh>().text = "TAP RIGHT\n" +( gameMgr.mysave.currentJumpType == 0 ? "COLOR" : "JUMP");

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
		#if UNITY_ANDROID && !UNITY_EDITOR
		if (!Social.localUser.authenticated) {
			Utils.addLog ("authenticated = " + Social.localUser.authenticated.ToString ());
			gameMgr.login ();
		} else {

		Social.ShowLeaderboardUI();
		

		}
		#elif UNITY_IOS && !UNITY_EDITOR
		GameCenterPlatform.ShowLeaderboardUI(gameMgr.getCurrentLeaderBoardId(),UnityEngine.SocialPlatforms.TimeScope.AllTime);
		#endif



	}

	public void onLeaderboardsButton_mainMenu_Clicked()
	{

		#if UNITY_ANDROID && !UNITY_EDITOR
		if (!Social.localUser.authenticated) {
			Utils.addLog ("authenticated = " + Social.localUser.authenticated.ToString ());
			gameMgr.login ();
		} else {
			
			Social.ShowLeaderboardUI();
			
			
		}
		#elif UNITY_IOS && !UNITY_EDITOR
		GameCenterPlatform.ShowLeaderboardUI(gameMgr.getCurrentLeaderBoardId(),UnityEngine.SocialPlatforms.TimeScope.AllTime);
		#endif

	}

	public void onLeaderboardButton_normalClicked()
	{
		//playMenuClickedSound ();
		#if UNITY_IOS && !UNITY_EDITOR
		if (Social.localUser.authenticated)
			GameCenterPlatform.ShowLeaderboardUI(GameManager.leaderboardId,UnityEngine.SocialPlatforms.TimeScope.AllTime);
		#endif
	}

	public void onLeaderboardButton_hardlClicked()
	{
		//playMenuClickedSound ();
		#if UNITY_IOS && !UNITY_EDITOR
		if (Social.localUser.authenticated)
			GameCenterPlatform.ShowLeaderboardUI(GameManager.leaderboardId_hardcore,UnityEngine.SocialPlatforms.TimeScope.AllTime);
		#endif
	}

	public void onPauseButtonClicked()
	{
		if (bPauseButtonDisabled)
			return;
		
		//playMenuClickedSound ();
		SetPausePanel (true);

		ShowOneOfTheBannerViews();
		gameMgr.PauseGame ();

	}

	public void onRestartButtonClicked()
	{
		//playMenuClickedSound ();
		ResumeGame ();
		OnTryAgainButtonClicked ();
	}

	void ResumeGame()
	{
		SetPausePanel (false);
		SetScorePanel (true);
		HideAllBannerViews();
		gameMgr.UnPauseGame ();
	}
	public void onResumebuttonClicked()
	{
		//playMenuClickedSound ();

		setCountingPanel (true,false);
		countingTime = 3f;
		TickingCountCoroutine = TickingCountingForResume ();
		StartCoroutine (TickingCountCoroutine);

	}


	IEnumerator TickingCountingForResume()
	{
		//hide ui first
		SetPausePanel (false);
		SetScorePanel (true);
		HideAllBannerViews();

		CountingText.text = ((int)countingTime).ToString();
		
		while (countingTime > 0) {
			yield return StartCoroutine (CoroutineUtil.WaitForRealSeconds (1f));
			countingTime -= 1f;
			CountingText.text = ((int)countingTime).ToString();
		}
		
		setCountingPanel (false);

		gameMgr.UnPauseGame ();
		TickingCountCoroutine = null;
		yield return null;
	}

	public void onBackButtonClicked()//pause panel
	{
		//back to main menu
		//playMenuClickedSound ();

		Animator animator = ScreenShotFrame.GetComponent<Animator> ();
		if(!animator.GetCurrentAnimatorStateInfo(0).IsName("ScreenShotImgAnimation"))
			animator.Play ("ScaleDown");

		gameMgr.UnPauseGame ();

		gameMgr.BackToMainMenu ();
	}

	public void onNoAdsButtonClicked()
	{
	/*	#if UNITY_EDITOR
		PlayerPrefs.DeleteAll ();
		#endif*/
		//playMenuClickedSound ();
		gameMgr.RemoveAds ();

		UnityAnalytics.CustomEvent("NoAdsButtonClicked",new Dictionary<string, object>{
			{ "NoAdsButtonClicked", 1 }
		} );
		
	}
	
	public void onRateButtonClicked()
	{
		//playMenuClickedSound ();
		gameMgr.ratedGame ();
		Utils.rateGame ();

		UnityAnalytics.CustomEvent("RateButtonClicked",new Dictionary<string, object>{
			{ "RateButtonClicked", 1 }
		} );
	}

	public void onHardcoreClicked()
	{
		//playMenuClickedSound ();

		if(ExtraButton.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("MovingUp"))
			ExtraButton.GetComponent<Animator>().Play("MovingDown");
		
		if(gameMgr.hardCoreUnlocked)
			DoTransition (DoHardCoreButton);
		else
			ShowAutoMessage("HARDCORE  MODE  CAN  BE  UNLOCKED  BY  REACHING FLOOR.100  IN  NORMAL  MODE");
	}

	public void OnStartButtonClicked()
	{
		//playMenuClickedSound ();

		if(ExtraButton.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("MovingUp"))
			ExtraButton.GetComponent<Animator>().Play("MovingDown");


		
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

		if (bActive) {
			UI_PausePanel.GetComponent<Animator> ().Play ("GenericMenuOpenedAnimation");

			RecordVideoButtonOnPausePanel.GetComponent<UnityEngine.UI.Button>().interactable = !Everyplay.IsRecording();
		}
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

		if (!bActive)
			bPauseButtonDisabled = false; // enable pause button 
	}

	public void SetStartPanel(bool bActive, bool wantTileSlideInAnim = true)
	{

		UI_StartPanel.SetActive (bActive);

		//only when back from game will call this
		if(wantTileSlideInAnim)
			UI_StartPanel.GetComponent<Animator>().Play("TitleSlideAnimation");
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

			UnityAnalytics.CustomEvent("UnityAdsPopup",new Dictionary<string, object>{
				{ "UnityAdsNum", 1 }
			} );
			
		} else {
			Utils.addLog("ads not ready!");
			NoAdsContinueDie();
		}
	}

	public void UnityAdsYesButtonClicked()
	{
		SetPauseButton(true);
#if UNITY_EDITOR
		gameMgr.GetCurrentPlayer ().GetComponent<PlayerController> ().AfterWatchAds();
		SetUnityAdsQuestion (false);
		return;
#elif UNITY_ANDROID || UNITY_IOS
		
		if(Everyplay.IsRecording())
			Everyplay.PauseRecording();

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
		if(Everyplay.IsPaused())
			Everyplay.ResumeRecording();
	}
	
	public void UnityAdsNoButtonClicked()
	{
		//playMenuClickedSound ();
		SetPauseButton(true);
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
		////playMenuClickedSound ();
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
		//playMenuClickedSound ();
		setRateQuestionPanel (false);
		gameMgr.rateLater ();
		gameMgr.UnPauseGame ();
	}

	public void setFreeSkinTrialPanel(bool bActive)
	{
		UI_FreeSkinTrialQuestion.SetActive (bActive);
		SetDimImage (bActive);

		
		if (gameMgr.GetCurrentPlayer () != null)
			gameMgr.GetCurrentPlayer ().GetComponent<PlayerController> ().allowInput = !bActive;

		if (bActive) {
			UI_FreeSkinTrialQuestion.GetComponent<Animator> ().Play ("GenericMenuOpenedAnimation");
		}

	}

	public void onFreeSkinTrialPanelSureButtonClicked()
	{
		setFreeSkinTrialPanel (false);
		GameObject freeSkinTrialTemplate = gameMgr.FreeSkinTrial ();
		if (freeSkinTrialTemplate != null) {
			PlayerSkin ps = freeSkinTrialTemplate.GetComponent<PlayerSkin>();
			if(ps != null && ps.skinId != "")
			{
				ShowAutoMessage("!! "+ps.skinName + " !!",turnOnPlayerInput,true,ps.ShopIcon, false,true,true,true);
			}
		}
	}

	void turnOnPlayerInput()
	{
		if (gameMgr.GetCurrentPlayer () != null)
			gameMgr.GetCurrentPlayer ().GetComponent<PlayerController> ().allowInput = true;
	}

	public void onFreeSkinTrialPanelNahButtonClicked()
	{
		setFreeSkinTrialPanel (false);
	}

	public void setRateQuestionPanel(bool bActive)
	{
		UI_RateQuestion.SetActive (bActive);
		SetDimImage (bActive);

		
		if (gameMgr.GetCurrentPlayer () != null)
			gameMgr.GetCurrentPlayer ().GetComponent<PlayerController> ().allowInput = !bActive;

		if (bActive) {
			gameMgr.PauseGame ();
			UI_RateQuestion.GetComponent<Animator> ().Play ("GenericMenuOpenedAnimation");
		}



	}

	public void RemoveLocalSave()
	{
		GameFile.Save("save.data", new SaveObject(true));

		#if UNITY_EDITOR
		PlayerPrefs.DeleteAll ();
		#endif
	}

	public void SetCreditPanel(bool bActive)
	{
		//playMenuClickedSound ();
		UI_CreditPanel.SetActive (bActive);
		SetDimImage (bActive);
		
		if (bActive) {
			UI_CreditPanel.GetComponent<Animator> ().Play ("GenericMenuOpenedAnimation");
		}

	}

	public void ShowAutoMessage(string message, AutoMessageOKButtonDelegate messageOkDelegate = null, bool wantOKButton = true, Sprite img = null, bool wantCenter = true, bool wantAnim = true, bool wantImgBG = false, bool wantDisableInput = false)
	{
		currentMessageOkButtonDelegate = messageOkDelegate;
		bool bActive = message.Length > 0;
		UI_AutoMessage.SetActive (bActive);
		SetDimImage (bActive, true);

		autoMessageImg.enabled = false;


		if(bActive)
		{
			if(wantAnim)
				UI_AutoMessage.GetComponent<Animator> ().Play ("GenericMenuOpenedAnimation");

			AutoMessageText.text = message;
			if(wantCenter)
				AutoMessageText.alignment = TextAnchor.MiddleCenter;
			else
				AutoMessageText.alignment = TextAnchor.UpperCenter;

			if(img != null)
			{
				autoMessageImg.sprite = img;
				autoMessageImg.enabled = true;
			}
			else
				autoMessageImg.enabled = false;

			autoMessageImgBG.enabled = wantImgBG;

			if(wantDisableInput && gameMgr.GetCurrentPlayer () != null)
			   gameMgr.GetCurrentPlayer ().GetComponent<PlayerController> ().allowInput = false;//disable it !

		}




		AutoMessageOKButton.SetActive(wantOKButton);
	}

	public void SetOptionPanel(bool bActive)
	{
		UI_OptionPanel.SetActive (bActive);
		SetDimImage (bActive);

		if(bActive)
		{
			UI_OptionPanel.GetComponent<Animator> ().Play ("GenericMenuOpenedAnimation");

			SwitchJumpLeftRightText.text = gameMgr.mysave.currentJumpType == 0 ? "SET RIGHT JUMP" : "SET LEFT JUMP";

			RecordVideoButton.GetComponent<UnityEngine.UI.Button>().interactable = !Everyplay.IsRecording();
		}
	}

	public void SetShopPanel(bool bActive)
	{
		UI_ShopPanel.SetActive (bActive);
		SetDimImage (bActive);
		if(bActive)
		{
			currentShopItemDisplayIndex = gameMgr.currentSkinTemplateIdx;
			UI_ShopPanel.GetComponent<Animator> ().Play ("GenericMenuOpenedAnimation");
			DisplayShopItem(currentShopItemDisplayIndex);
		}
	}

	public void onShopPreviousButtonDown()
	{
		ButtonHoldLoopCoroutine = previousButtonLoop (0.3f);
		StartCoroutine (ButtonHoldLoopCoroutine);
	}

	public void onShopPreviousButtonUp()
	{
		if (ButtonHoldLoopCoroutine != null) {
			StopCoroutine (ButtonHoldLoopCoroutine);
			ButtonHoldLoopCoroutine = null;
		}
	}

	IEnumerator previousButtonLoop(float loopinterval)
	{
		while(true)
		{
			onShopPreviousButtonClicked ();
			yield return new WaitForSeconds(loopinterval);
			loopinterval -= 0.01f;
			loopinterval = Mathf.Max(0.2f,loopinterval);
		}
	}

	public void onShopNextButtonDown ()
	{
		ButtonHoldLoopCoroutine = nextButtonLoop (0.3f);
		StartCoroutine (ButtonHoldLoopCoroutine);
	}

	public void onShopNextButtonUp()
	{
		if(ButtonHoldLoopCoroutine != null)
			StopCoroutine (ButtonHoldLoopCoroutine);
	}

	IEnumerator nextButtonLoop(float loopinterval)
	{
		while(true)
		{
			onShopNextButtonClicked ();
			yield return new WaitForSeconds(loopinterval);
			loopinterval -= 0.01f;
			loopinterval = Mathf.Max(0.2f,loopinterval);
		}
	}

	public void onShopPreviousButtonClicked()
	{
		if(currentShopItemDisplayIndex > 0)
			currentShopItemDisplayIndex--;

		DisplayShopItem (currentShopItemDisplayIndex);
	}
	public void onShopNextButtonClicked ()
	{
		if (currentShopItemDisplayIndex < gameMgr.SkinTemplates.Count - 1)
			currentShopItemDisplayIndex++;
		DisplayShopItem (currentShopItemDisplayIndex);
	}

	void refreshCurrentShopItem()
	{
		if(UI_ShopPanel.activeSelf)
			DisplayShopItem (currentShopItemDisplayIndex);
	}

	public void onMarketPurchase(string itemId)
	{
		CancelInvoke ("ConnectToAppStoreTimeOut");
		ShowAutoMessage ("");
		Invoke ("refreshCurrentShopItem", 0.5f);
	}

	public void onPurchaseStarted(string itemId)
	{
		ShowAutoMessage("PROCESSING...\nPLEASE  WAIT",null,false,null,true,false);
		CancelInvoke ("ConnectToAppStoreTimeOut");
		Invoke("ConnectToAppStoreTimeOut",90f);
	}
	
	public void onPurchaseCancelled(string itemId)
	{
		CancelInvoke ("ConnectToAppStoreTimeOut");
		ShowAutoMessage ("CANCELLED!");
	}

	public void onRestorePurchaseButtonClicked()
	{
#if UNITY_IOS && !UNITY_EDITOR
		SoomlaStore.RestoreTransactions ();
		UnityAnalytics.CustomEvent("IOSRestorePurchaseButtonClicked",new Dictionary<string, object>{
			{ "IOSRestorePurchaseButtonClicked", 1 }
		} );
#endif
#if UNITY_ANDROID
		ShowAutoMessage("ANDROID  USES  DONT  HAVE  TO  RESTORE!");
#endif
	}

	void DisplayShopItem(int idxOfgoodies)
	{
		GameObject objTemp = gameMgr.SkinTemplates [idxOfgoodies];
		PlayerSkin ps = objTemp.GetComponent<PlayerSkin> ();
		if (ps != null) {
			//show icon
			currentShopItemImage.sprite = ps.ShopIcon;
			currentShopItemName.text = ps.skinName;

			if(ps.purchasable)
			{
				VirtualGood vg = (VirtualGood)StoreInfo.GetItemByItemId(ps.skinId);
				string strfromStore = ((PurchaseWithMarket)vg.PurchaseType).MarketItem.MarketPriceAndCurrency;
				currentShopItemPriceText.text = strfromStore;
				Debug.Log("str from store:" + strfromStore);
			}
			currentShopItemId = ps.skinId;


			if(ps.freeToUse || ps.purchasable && currentShopItemId != "" && StoreInventory.GetItemBalance (currentShopItemId) > 0 )
			{
				//refresh UI , already owned
				currentShopItemPriceBG.SetActive(false);
				purchaseButtonText.text = "CHOOSE";
			}else
			{
				currentShopItemPriceBG.SetActive(true); 
				purchaseButtonText.text = "I  WANT  IT !";
				//currentShopItemPriceBG.SetActive(false);//
				//purchaseButtonText.text = ps.skinPrice.ToString("0.00");
			}

		}

		shopPreviousButton.GetComponent<UnityEngine.UI.Button>().interactable = currentShopItemDisplayIndex > 0;
		shopNextButton.GetComponent<UnityEngine.UI.Button>().interactable = currentShopItemDisplayIndex < gameMgr.SkinTemplates.Count - 1;
	}

	public void onRestoreTransactionsStarted() {
#if UNITY_IOS && !UNITY_EDITOR
		ShowAutoMessage("PROCESSING...\nPLEASE  WAIT",null,false);
		CancelInvoke ("ConnectToAppStoreTimeOut");
		Invoke("ConnectToAppStoreTimeOut",90f);
#endif
	}
	
	public void onRestoreTransactionsFinished(bool success) {

#if UNITY_IOS && !UNITY_EDITOR
		CancelInvoke ("ConnectToAppStoreTimeOut");
		if(success)
			ShowAutoMessage ("FINISHED !");
		else
			ShowAutoMessage ("FAILED !");
#endif
	}

	public void PurchaseCurrentSelectedSkin()
	{
		//playMenuClickedSound ();
		Utils.addLog("PurchaseCurrentSelectedSkin");
		PlayerSkin ps = gameMgr.SkinTemplates [currentShopItemDisplayIndex].GetComponent<PlayerSkin> ();
		if(ps != null)
		{
			if (currentShopItemId != "" && StoreInventory.GetItemBalance (currentShopItemId) == 0) {
				gameMgr.BuySkin (currentShopItemId);

			}
			else {
				 
					gameMgr.UseSkin (currentShopItemDisplayIndex);
					gameMgr.ApplySkinSetting();
					SetShopPanel(false);
					if (gameMgr.GetCurrentPlayer () != null)
						gameMgr.GetCurrentPlayer ().GetComponent<PlayerController> ().allowInput = true;
				}
		}


	}

	void ConnectToAppStoreTimeOut()
	{
		ShowAutoMessage("TIMEOUT\nCANNOT  CONNECT\nTO  APP  STORE");
	}

	public void onShopButtonClicked()
	{
		//playMenuClickedSound ();
		SetShopPanel (true);
	}

	public void onShopBackButtonClicked()
	{
		//playMenuClickedSound ();
		SetShopPanel (false);

		if (gameMgr.GetCurrentPlayer () != null)
			gameMgr.GetCurrentPlayer ().GetComponent<PlayerController> ().allowInput = true;

	}

	public void onOptionButtonClicked()
	{
		//playMenuClickedSound ();
		SetOptionPanel (true);
	}

	public void onCreditButtonClicked()
	{
		//playMenuClickedSound ();
		//ShowAutoMessage("COLOR  JUMP\n\nDEVELOPED  BY\nJUNSHENG  YAO\n MUSIC  BY\n SHADY  DAVE\nSFX  BY  FREESOUND.ORG\nART BY  YANG  DU");
		SetCreditPanel (true);
	}

	public void ToggleDebug()
	{
		Utils.bDebug = !Utils.bDebug;
	}

	public void onCreditPanelOKButtonClicked()
	{
		//playMenuClickedSound ();
		UI_CreditPanel.SetActive (false);
		SetDimImage (false);
	}

	public void onAutoMessageOKButtonClicked()
	{
		//playMenuClickedSound ();
		
		if(currentMessageOkButtonDelegate != null)
			currentMessageOkButtonDelegate ();

		ShowAutoMessage ("");
	}

	public void SetDimImage(bool bActive, bool forAutoMessage = false)
	{
		DimImage.SetActive (bActive);

		if (forAutoMessage)
			DimImageForAutoMessage.SetActive (bActive);
	}

	public void OnShareButtonClicked()
	{
		Animator animator = ScreenShotFrame.GetComponent<Animator> ();
		if (!animator.GetCurrentAnimatorStateInfo (0).IsName ("ScaleUp"))
			animator.Play ("ScaleUp");

		Invoke ("ShareScreenshot", 1f);
	}

	IEnumerator DoTakingScreenShot()
	{


		yield return new WaitForEndOfFrame ();

		//resize frame

		float newFrameWidth = ScreenShotFrame.GetComponent<RectTransform>().rect.height *  ((float)Screen.width)   / Screen.height;
		 
		ScreenShotFrame.GetComponent<RectTransform> ().SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, newFrameWidth);
		// create the texture
		Texture2D aTex = new Texture2D (Screen.width, Screen.height, TextureFormat.RGB24, false);
		aTex.ReadPixels (new Rect (0f, 0f, Screen.width, Screen.height), 0, 0);
		aTex.Apply ();
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
		byte[] dataToSave = aTex.EncodeToPNG ();
		ScreenShotPath = Path.Combine (Application.persistentDataPath, System.DateTime.Now.ToString ("yyyy-MM-dd-HH") + ".png");
		//File.WriteAllBytes (ScreenShotPath, dataToSave);//don't save it now
		
		screenShotSprite = Sprite.Create (aTex, new Rect (0, 0, Screen.width, Screen.height), new Vector2 (0, 0));
		ScreenShotImg.sprite = screenShotSprite;

		int shotWidth = Screen.width;
		int shotHeight = Screen.height;
		if (shotWidth > 640) {
			shotWidth = 640;
			shotHeight = (int)(((float)Screen.height) / Screen.width * 640);
			Texture2D ScaledTex = Utils.ScaleTexture (aTex, shotWidth, shotHeight);
			File.WriteAllBytes (ScreenShotPath, ScaledTex.EncodeToPNG ());
			Destroy (ScaledTex);
		} else {
			File.WriteAllBytes (ScreenShotPath, dataToSave);
		}



		ShowOneOfTheBannerViews ();
		yield return new WaitForSeconds (2f);

		SetEndOfGameMark (false);
		//UI_ScorePanel.SetActive (false);//gameended

		StopRecording ();

		ShowDeathPanel ();
	}

	public void StopRecording ()
	{
		if (Everyplay.IsRecording ()) {
			Everyplay.StopRecording ();
			Everyplay.SetMetadata ("FloorNum", gameMgr.GetCurrentPlayer ().GetComponent<PlayerController> ().maxBarNum);
			Everyplay.SetMetadata ("Score", gameMgr.currentScore);
			bDidRecordingVideo = true;
		}

		if (bDidRecordingVideo)
		{
			ShowAutoMessage ("VIDEO RECORDING ENDED!", playLastRecording);
			bDidRecordingVideo = false;
		}
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

	public void onPausePanelRecordVideoButtonClicked()
	{
		//playMenuClickedSound ();
		setCountingPanel (true);
		countingTime = 3f;
		TickingCountCoroutine = TickingCountingForRecordingOnPausePanel ();
		StartCoroutine (TickingCountCoroutine);

	}

	public void onRecordVideoButtonClicked()
	{
		SetOptionPanel (false);
		setCountingPanel (true);
		countingTime = 3f;
		TickingCountCoroutine = TickingCountingForRecording ();
		StartCoroutine (TickingCountCoroutine);
	}

	public void OnReadyForRecording(bool enabled)
	{
		RecordButton.GetComponent<UnityEngine.UI.Button>().interactable = enabled;
	}

	public void startRecording()
	{

		if (Everyplay.IsSupported () && !Everyplay.IsRecording ())
			Everyplay.StartRecording ();

	}

	public void endRecording()
	{
		////playMenuClickedSound ();
	}

	public void playLastRecording()
	{
		//playMenuClickedSound ();
		Everyplay.PlayLastRecording ();
	}

	public void SwitchLeftRightJump()
	{
		//playMenuClickedSound ();
		gameMgr.mysave.currentJumpType = gameMgr.mysave.currentJumpType == 0 ? 1 : 0;
		GameFile.Save ("save.data", gameMgr.mysave);
		SwitchJumpLeftRightText.text = gameMgr.mysave.currentJumpType == 0 ? "SET RIGHT JUMP" : "SET LEFT JUMP";
	}

	public void onOptionBackButtonClicked()
	{
		//playMenuClickedSound ();
		SetOptionPanel (false);
	}

	public void setCountingPanel(bool bActive, bool wantCancelButton = true)
	{
		UI_CountingPanel.SetActive(bActive);
		if (bActive) {
			UI_CountingPanel.GetComponent<Animator> ().Play ("GenericMenuOpenedAnimation");

			countingPanelCancelButton.SetActive(wantCancelButton);
		}
	}

	public void onCountingCanceled()
	{
		//playMenuClickedSound ();
		setCountingPanel (false);
		if(TickingCountCoroutine != null)
			StopCoroutine (TickingCountCoroutine);
	}


	IEnumerator TickingCountingForRecordingOnPausePanel()
	{
		CountingText.text = ((int)countingTime).ToString();

		while (countingTime > 0) {

			yield return StartCoroutine (CoroutineUtil.WaitForRealSeconds (1f));

			countingTime -= 1f;
			CountingText.text = ((int)countingTime).ToString();
		}


		setCountingPanel (false);
		onResumebuttonClicked ();//resume game
		startRecording ();
		yield return null;
	}
	
	/*IEnumerator TickingCountingForRecordingOnPausePanel()
	{
		CountingText.text = ((int)countingTime).ToString();
		
		while (countingTime > 0) {
			yield return new WaitForSeconds (1f);
			countingTime -= 1f;
			CountingText.text = ((int)countingTime).ToString();
		}
		
		setCountingPanel (false);
		onResumebuttonClicked ();//resume game
		startRecording ();
		yield return null;
	}*/

	IEnumerator TickingCountingForRecording()
	{
		CountingText.text = ((int)countingTime).ToString();

		while (countingTime > 0) {
			yield return new WaitForSeconds (1f);
			countingTime -= 1f;
			CountingText.text = ((int)countingTime).ToString();
		}

		setCountingPanel (false);
		startRecording ();
		yield return null;
	}

	public void  onScorePanelShopORGiftButtonDown()
	{
		if(gameMgr.GetCurrentPlayer () != null)
			gameMgr.GetCurrentPlayer ().GetComponent<PlayerController> ().allowInput = false;
	}

	public void hideScorePanelShopAndGiftButton ()
	{
		Animator animator = UI_ScorePanel.GetComponent<Animator> ();
		if(animator.GetCurrentAnimatorStateInfo (0).IsName("ScorePanelShopAndGiftSlideIn"))
			animator.Play ("ScorePanelShopAndGiftSlideOut");
	}
	
	public void onExtraButtonClicked()
	{
		//Utils.forceAddToken (100);
		UnityEngine.AnimatorStateInfo animstateinfo = ExtraButton.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
		if (animstateinfo.IsName ("MovingUp"))
			ExtraButton.GetComponent<Animator> ().Play ("MovingDown");
		else {
			ExtraButton.GetComponent<Animator> ().Play ("MovingUp");

		}
	}

	public void onGiftButtonClicked()
	{
		//playMenuClickedSound ();
		SetGiftPanel (true);
	}

	public void SetGiftPanel(bool bActive)
	{
		UI_GiftPanel.SetActive(bActive);
		SetDimImage (bActive);
		
		if(bActive)
		{
			GiftImage.GetComponent<Animator>().Play("Regular");
			UI_GiftPanel.GetComponent<Animator> ().Play ("GenericMenuOpenedAnimation");
			updateMyTokenBalance();
		

			FreeTokenButton.SetActive( gameMgr.IsFreeTokenReady() );

			//Invoke("GiveFreeTokens",1f);


 
		}
	}

	public void GiveFreeTokens()
	{
		if (gameMgr.IsFreeTokenReady()) {
			playGiveFreeTokenSound();
			int howManyToken = 0;
			if(UnityEngine.Random.Range(0,10)<=1)// 20% get 10 tokens
			{
				howManyToken = gameMgr.AddFreeGiftToken (10);
			}
			else
			{
				howManyToken = gameMgr.AddFreeGiftToken (UnityEngine.Random.Range(2,6));
				
			}
			
			ShowAutoMessage("YOU  GOT  "+howManyToken.ToString() + "  TOKENS!",updateMyTokenBalance,true,giftTokenImg,false,true,true);
			FreeTokenButton.SetActive( gameMgr.IsFreeTokenReady() );

			
			UnityAnalytics.CustomEvent("GetFreeTokens",new Dictionary<string, object>{
				{ "GetFreeTokens", howManyToken }
			} );
		}

		if (GiftButtonTextObj.activeSelf) {
			Animator animator = GiftButtonTextObj.GetComponent<Animator> ();
			if (animator != null)
				animator.Play ("FlashText");
		}

		if (ScorePanelGiftButtonTextObj.activeSelf) {
			Animator animator = ScorePanelGiftButtonTextObj.GetComponent<Animator> ();
			if (animator != null)
				animator.Play ("FlashText");
		}
	}
	
	public void onGiftPanelBackButtonClicked()
	{
		//playMenuClickedSound ();
		SetGiftPanel (false);
		
		if (gameMgr.GetCurrentPlayer () != null)
			gameMgr.GetCurrentPlayer ().GetComponent<PlayerController> ().allowInput = true;
	}

	public void onOpenGiftBoxButtonClicked()
	{

		if (gameMgr.getOwnedSkins ().Count == gameMgr.SkinTemplates.Count - 1) {
			ShowAutoMessage ("!! WOW !!\nYOU  ALREADY  COLLECT  ALL  THE  SKINS !  MORE  FUN  SKINS  ARE  COMING  SOON !");
			return;
		}

		int mytokenNum = gameMgr.GetTokenNum ();

		if (mytokenNum >= gameMgr.TokenNumToOpenAGiftBox) {
			CancelInvoke ("stopOpeningGift");
			OpenGiftBoxButtonText.text = "OPENING...";
			GiftImage.GetComponent<Animator> ().Play ("OpenGiftAnimation");
			playOpenGiftSound ();
			Invoke ("stopOpeningGift", 3f);


		} else {
			ShowAutoMessage("YOU  CAN  COLLECT  TOKENS  BY  PLAYING  THE  GAME  OR  COME  BACK  EVERY  FEW  MINUTUES !");
		}
	}

	void updateMyTokenBalance()
	{
		int mytokenNum = gameMgr.GetTokenNum ();
		OpenGiftBoxButtonText.text = mytokenNum < 10 ? (10 - mytokenNum).ToString () + "  TO  GO" : "TRY  MY  LUCK";
		string tokencolorstr = mytokenNum < 10 ? "fa4a37" : "3afa37";//red and green
		MyTokenBalanceText.text = "I  HAVE  <color=#"+tokencolorstr+">" +mytokenNum.ToString()+"</color> TOKENS !!";
	}

	void stopOpeningGift()
	{
		if ( gameMgr.consumeToken (10)) {


			if(UnityEngine.Random.Range(0,3) == 0)
			{
				PlayerSkin ps = gameMgr.GivePlayerRandomSkin();
				if(ps != null)
				{
					playGiveFreeTokenSound();
					ShowAutoMessage("!! "+ps.skinName + " !!",updateMyTokenBalance,true,ps.ShopIcon, false,true,true);
				}
				else if (UnityEngine.Random.Range(0,10) <= 6)
				{
					int howManyToken = 0;
					if(UnityEngine.Random.Range(0,15)<=1)// chance to get more tokens
					{
						howManyToken = gameMgr.AddFreeGiftToken (15,false);
					}
					else
					{
						howManyToken = gameMgr.AddFreeGiftToken (UnityEngine.Random.Range(2,6),false);
						
					}
					playGiveFreeTokenSound();
					ShowAutoMessage("YOU  GOT  "+howManyToken.ToString() + "  TOKENS!",updateMyTokenBalance,true,giftTokenImg,false,true,true);

					UnityAnalytics.CustomEvent("RandomTokensFromGiftBox",new Dictionary<string, object>{
						{ "RandomTokensFromGiftBox", howManyToken }
					} );
				}
				else
				{
					playNoGiftSound();
					ShowAutoMessage("!! OH  NO !!",updateMyTokenBalance,true,noGiftImg,false,true,true);
				}
			}
			else if (UnityEngine.Random.Range(0,10) <= 6)
			{
				int howManyToken = 0;
				if(UnityEngine.Random.Range(0,15)<=1)// chance to get more tokens
				{
					howManyToken = gameMgr.AddFreeGiftToken (15,false);
				}
				else
				{
					howManyToken = gameMgr.AddFreeGiftToken (UnityEngine.Random.Range(2,6),false);
					
				}
				playGiveFreeTokenSound();
				ShowAutoMessage("YOU  GOT  "+howManyToken.ToString() + "  TOKENS!",updateMyTokenBalance,true,giftTokenImg,false,true,true);

				
				UnityAnalytics.CustomEvent("RandomTokensFromGiftBox",new Dictionary<string, object>{
					{ "RandomTokensFromGiftBox", howManyToken }
				} );
			}
			else
			{
				playNoGiftSound();
				ShowAutoMessage("!! OH  NO !!",updateMyTokenBalance,true,noGiftImg,false,true,true);
			}
			
			GiftImage.GetComponent<Animator>().Play("Regular");
		}
	}

	public void resetFreeTokenInfoText()
	{
		FreeTokenInfoText.GetComponent<FreeTokenController> ().ForceDisplayNonSynchronizedInfo ();//showup
		
	}

	public void onScreenShotImgFrameClicked()
	{
		//playMenuClickedSound ();
		Animator animator = ScreenShotFrame.GetComponent<Animator> ();
		if(animator.GetCurrentAnimatorStateInfo(0).IsName("ScreenShotImgAnimation"))
			animator.Play ("ScaleUp");
		else
			animator.Play ("ScaleDown");
	}
	
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
//using UnityEngine.Advertisements;
using GoogleMobileAds.Api;
#if UNITY_ANDROID
using GooglePlayGames;
#endif
#if UNITY_IOS
using UnityEngine.SocialPlatforms.GameCenter;
#endif
public class GameSceneEvents : MonoBehaviour {

	//[SerializeField]
	//GameObject yellowboardsButton = null;
	//[SerializeField]
	//GameObject UI_SmallLeaderBoardsPanel = null;
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
	GameObject UI_ColorIndicationPanel = null; 
	[SerializeField]
	GameObject UI_NoColorIndicationText = null;
	GameObject Player;


	GameManager gameMgr;
	SaveObject mysave;
	


	BannerView bannerView,bannerViewBottom;
	AdRequest request;

	[SerializeField]
	GameObject transitionImg = null; 




	void Start () {


		UI_DeathPanel.SetActive (false);

		if (gameMgr == null)
			InitGameMgr ();

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
		/*
		if (Advertisement.isSupported) {
			Advertisement.allowPrecache = true;
			string UnityAdsId="";
			#if UNITY_IOS && !UNITY_EDITOR
			//UnityAdsId = "33340";
			#endif
			#if UNITY_ANDROID && !UNITY_EDITOR
			//UnityAdsId = "34245";
			#endif


			Advertisement.Initialize(UnityAdsId);
			UI_ScoreText.GetComponent<UnityEngine.UI.Text>().text = "Platform supported";
		} else {
			Debug.Log("Platform not supported");
			UI_ScoreText.GetComponent<UnityEngine.UI.Text>().text = "Platform not supported";
		}
		*/

		// Create a 320x50 banner at the top of the screen.

		string bannerAdsId="";
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
			
			ShowOneOfTheBannerViews(true);
		}


			



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
		Invoke ("ShowDeathPanel", 1f);
	}

	public void onPlayerRespawn()
	{
		UpdateUISocre (0);
		showTutorial (true);
		SetColorIndicationPanel (true);//auto show
	}

	void ShowDeathPanel()
	{	
		UI_DeathPanel.SetActive (true);
		yourScore.text = gameMgr.currentScore.ToString ();
		yourBest.text = gameMgr.getBestScore ().ToString();

		ShowOneOfTheBannerViews ();

	}

	public void OnTryAgainButtonClicked()
	{
		HideAllBannerViews ();


		gameMgr.StartGame (-1);

		UI_DeathPanel.SetActive (false);


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
		UI_ScorePanel.SetActive (false);
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
#if UNITY_ANDROID
		Social.ShowLeaderboardUI();
#elif UNITY_IOS
		GameCenterPlatform.ShowLeaderboardUI(gameMgr.getCurrentLeaderBoardId(),UnityEngine.SocialPlatforms.TimeScope.AllTime);
#endif

	}

	public void onLeaderboardsButton_mainMenu_Clicked()
	{
		if (!Social.localUser.authenticated) {
			Utils.addLog("authenticated = " + Social.localUser.authenticated.ToString());
			gameMgr.login ();
		}
		
		#if UNITY_ANDROID
		Social.ShowLeaderboardUI();
		#elif UNITY_IOS
		GameCenterPlatform.ShowLeaderboardUI("",UnityEngine.SocialPlatforms.TimeScope.AllTime);

		//UI_SmallLeaderBoardsPanel.SetActive (true);
		//yellowboardsButton.SetActive (false);
		#endif


	}

	public void onLeaderboardButton_normalClicked()
	{
#if UNITY_IOS
		if (Social.localUser.authenticated)
			GameCenterPlatform.ShowLeaderboardUI(GameManager.leaderboardId,UnityEngine.SocialPlatforms.TimeScope.AllTime);
#endif
	}

	public void onLeaderboardButton_hardlClicked()
	{
#if UNITY_IOS
		if (Social.localUser.authenticated)
			GameCenterPlatform.ShowLeaderboardUI(GameManager.leaderboardId_hardcore,UnityEngine.SocialPlatforms.TimeScope.AllTime);
#endif
	}

	public void onPauseButtonClicked()
	{
		UI_PausePanel.SetActive (true);
		UI_PausePanel.GetComponent<Animator> ().Play ("PausePanelOpened");
		UI_ScorePanel.SetActive (false);
		ShowOneOfTheBannerViews();
		gameMgr.PauseGame ();

	}

	public void onRestartButtonClicked()
	{
		onResumebuttonClicked ();
		OnTryAgainButtonClicked ();
	}

	public void onResumebuttonClicked()
	{
		UI_PausePanel.SetActive (false);
		UI_ScorePanel.SetActive (true);
		HideAllBannerViews();
		gameMgr.UnPauseGame ();
	}

	public void onBackButtonClicked()
	{
		//back to main menu
		gameMgr.BackToMainMenu ();


		gameMgr.UnPauseGame ();
	}

	public void onNoAdsButtonClicked()
	{

	}

	public void onRateButtonClicked()
	{
		Utils.rateGame ();
	}

	public void onHardcoreClicked()
	{
		DoTransition (DoHardCoreButton);
	}

	public void OnStartButtonClicked()
	{
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
	}

	public void SetScorePanel(bool bActive)
	{
		UI_ScorePanel.SetActive (bActive);
	}

	public void SetDeathPanel(bool bActive)
	{
		UI_DeathPanel.SetActive (bActive);
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
}

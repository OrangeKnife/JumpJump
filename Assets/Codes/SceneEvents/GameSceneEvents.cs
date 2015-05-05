using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
//using UnityEngine.Advertisements;
using GoogleMobileAds.Api;
#if UNITY_ANDROID
using GooglePlayGames;
#endif
public class GameSceneEvents : MonoBehaviour {

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

	GameObject Player;


	GameManager gameMgr;
	SaveObject mysave;

	List<GameObject> abilityUISlots = new List<GameObject>();


	BannerView bannerView;
	AdRequest request;
	void Start () {


		UI_DeathPanel.SetActive (false);

		if (gameMgr == null)
			InitGameMgr ();

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
		//ca-app-pub-7183026460514946/5522304910 is for IOS now
		//ca-app-pub-7183026460514946/2010435315 is for android
		// Create a 320x50 banner at the top of the screen.

		//string bannerAdsId="";
		#if UNITY_IOS && !UNITY_EDITOR
		//bannerAdsId = "ca-app-pub-7183026460514946/5522304910";
		#endif
		#if UNITY_ANDROID && !UNITY_EDITOR
		//bannerAdsId = "ca-app-pub-7183026460514946/2010435315";
		#endif
		/*
		if (bannerView == null) {
			bannerView = new BannerView (
			bannerAdsId, AdSize.Banner, AdPosition.Top);
			// Create an empty ad request.
			request = new AdRequest.Builder ().Build ();
			// Load the banner with the request.
			bannerView.LoadAd (request);
			bannerView.Hide ();
		}
		else
			bannerView.Hide ();
			*/



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
	}

	void ShowDeathPanel()
	{	
		UI_DeathPanel.SetActive (true);
		yourScore.text = gameMgr.currentScore.ToString ();
		yourBest.text = gameMgr.bestScore.ToString ();

		if(bannerView!=null)
			bannerView.Show ();

	}

	public void OnTryAgainButtonClicked()
	{
		if(bannerView!=null)
			bannerView.Hide ();


		gameMgr.StartGame ();

		UI_DeathPanel.SetActive (false);


	}

	public void OnChangeCharacterButtonClicked()
	{
		
		if(bannerView!=null)
			bannerView.Hide ();


		gameMgr.EndGame ();
	}
	

	public void UpdateUISocre(int newScore)
	{
		UI_ScoreText.GetComponent<UnityEngine.UI.Text>().text = newScore.ToString();
	}

	public void UpdateUILife(int newLife)
	{
		UI_LifeText.GetComponent<UnityEngine.UI.Text> ().text = newLife.ToString ();
	}

	public void CleanUpAbilityUISlots()
	{
		abilityUISlots.Clear ();
	}

	public void OnTapToStartButtonClicked()
	{
		UI_StartPanel.SetActive (false);
		gameMgr.RespawnPlayer ();
		gameMgr.StartGame ();
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

		Social.ShowLeaderboardUI();


	}
}

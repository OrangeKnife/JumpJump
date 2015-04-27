﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Advertisements;
using GoogleMobileAds.Api;

public class GameSceneEvents : MonoBehaviour {

	[SerializeField]
	GameObject UI_DeathPanel = null;
	[SerializeField]
	GameObject UI_ScorePanel = null;
	[SerializeField]
	GameObject UI_ScoreText = null;
	[SerializeField]
	GameObject UI_StartPanel = null;

	GameObject Player;


	GameManager gameMgr;


	List<GameObject> abilityUISlots = new List<GameObject>();


	BannerView bannerView;
	AdRequest request;
	void Start () {


		UI_DeathPanel.SetActive (false);

		if (gameMgr == null)
			InitGameMgr ();

	}

	void Awake() {
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

		//ca-app-pub-7183026460514946/5522304910 is for IOS now
		//ca-app-pub-7183026460514946/2010435315 is for android
		// Create a 320x50 banner at the top of the screen.

		string bannerAdsId="";
		#if UNITY_IOS && !UNITY_EDITOR
		//bannerAdsId = "ca-app-pub-7183026460514946/5522304910";
		#endif
		#if UNITY_ANDROID && !UNITY_EDITOR
		//bannerAdsId = "ca-app-pub-7183026460514946/2010435315";
		#endif
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
	}

	void InitGameMgr()
	{
		gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();

	}

	public void onPlayerDead() 
	{
		Invoke ("ShowDeathPanel", 2f);
	}

	public void onPlayerRespawn()
	{
		UpdateUISocre (0);
	}

	void ShowDeathPanel()
	{	
		UI_DeathPanel.SetActive (true);

		if(bannerView!=null)
			bannerView.Show ();

	}

	public void OnTryAgainButtonClicked()
	{
		if(bannerView!=null)
			bannerView.Hide ();


		gameMgr.RespawnPlayer();

		UI_DeathPanel.SetActive (false);
		UI_ScorePanel.SetActive (true);


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

	}

	public void addLog(string logstring)
	{
		GameObject.Find ("LogText").GetComponent<UnityEngine.UI.Text> ().text += logstring;
	}

}

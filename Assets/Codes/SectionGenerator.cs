using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SectionGenerator : MonoBehaviour {

	private GameObject player;
	private Transform playerTransform;

	public bool bInitSpawnTestSection;
	public bool bInitSpawnTutorialSection = false;
	public List<GameObject> SectionArray_TestSection;
	public List<GameObject> SectionArray_Tutorial;
	public List<GameObject> SectionArray_Easy;
	public List<GameObject> SectionArray_Normal;
	public List<GameObject> SectionArray_Hard;
	public List<GameObject> SectionArray_Wtf;

	private List<GameObject> currentLevel = null;
	// store spawned section here, so we know how many we have and can kill it properly
	private LinkedList<GameObject> SpawnedSectionList;
	private float LastSectionBeginPosition;


	/// <summary>
	/// Difficulty related
	/// </summary>
	public struct SectionPicker
	{
		 
	}

	private int NumberOfGeneratedSection;
	private int DifficultyMod = 5;
	private SectionPicker NormalPicker;
	private SectionPicker SurprisePicker;
	
	GameManager gameMgr;
	// Use this for initialization
	void Start () 
	{
		currentLevel = SectionArray_Normal;

		if (gameMgr == null)
			gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();

//		///// grab player
//		player = GameObject.FindGameObjectWithTag ("Player");
//		playerTransform = player.transform;

		//InitLevel();
	}

	public void InitLevel()
	{
		if (SpawnedSectionList == null)
			SpawnedSectionList = new LinkedList<GameObject>();
		
		NumberOfGeneratedSection = 0;
		currentLevel = SectionArray_Normal;

		///// grab player
		//player = GameObject.FindGameObjectWithTag ("Player");
		if (gameMgr == null)
			gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();

		player = gameMgr.GetCurrentPlayer();
		playerTransform = player.transform;

		///// kill old sections
		int count = SpawnedSectionList.Count;
		for (int i = 0; i < count; ++i) 
		{
			Destroy (SpawnedSectionList.First.Value);
			SpawnedSectionList.RemoveFirst();
		}

		if (bInitSpawnTestSection) {
			print ("test section");
			for (int i = 0; i < SectionArray_TestSection.Count; ++i) {
				SpawnSection (SectionArray_TestSection [i]);
			}
		} else if (bInitSpawnTutorialSection) {
			print ("tut section");
			// spawn tutorial sections
			for (int i = 0; i < SectionArray_Tutorial.Count; ++i) {
				SpawnSection (SectionArray_Tutorial [i]);
			}
		} else {
		}
			 

	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	void SpawnSection(GameObject template)
	{
	}

	public int GetDifficulty()
	{
		return (int)Mathf.Ceil(NumberOfGeneratedSection / (float)DifficultyMod);
	}

}

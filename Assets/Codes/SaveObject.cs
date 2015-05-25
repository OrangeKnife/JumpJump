
using System;

[Serializable]
public class SaveObject
{
	public SaveObject(bool inFirstRun)
	{
		firstRun = inFirstRun;
		bestScore = 0;
		bestScore_hardcore = 0;
		deathCount = 0;
		unlockedHardCore = false;
		rated = false;
		rateLaterDeathCount = 5;

		currentJumpType = 1;

		lastSelectedSkin = 0;
	}
	public bool firstRun;
	public int bestScore,bestScore_hardcore;
	public int deathCount;
	public bool unlockedHardCore;
	public bool rated;
	public int rateLaterDeathCount;
	public int version = 0;

	public int currentJumpType;
	public int accumulatedPlayTime;
	public int lastSelectedSkin;
	//
	public bool[] additionalInfoBool = new bool[10];
	public int[] additionalInfoInt = new int[10];
	public float[] additionalInfoFloat = new float[10];
	public string[] additionalInfoString = new string[10];
	}


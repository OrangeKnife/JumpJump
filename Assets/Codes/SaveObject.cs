
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
	}
	public bool firstRun;
	public int bestScore,bestScore_hardcore;
	public int deathCount;
	public bool unlockedHardCore;
	public bool rated;
	public int rateLaterDeathCount;
	public int version = 0;
}



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
	}
	public bool firstRun;
	public int bestScore,bestScore_hardcore;
	public int deathCount;
	public bool unlockedHardCore;
}


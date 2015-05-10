
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
	}
	public bool firstRun;
	public int bestScore,bestScore_hardcore;
	public int deathCount;
}


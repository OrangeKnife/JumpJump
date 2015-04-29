
using System;

[Serializable]
public class SaveObject
{
	public SaveObject(bool inFirstRun)
	{
		firstRun = inFirstRun;
		bestScore = 0;
	}
	public bool firstRun;
	public int bestScore;
}


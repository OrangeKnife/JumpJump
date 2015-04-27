
using System;

[Serializable]
public class SaveObject
{
	public SaveObject(bool inFirstRun)
	{
		firstRun = inFirstRun;

		optionMusic = true;
	}
	public bool firstRun;

	public bool optionMusic;
}


﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Utils {

	static int logLineCount = 0;
	static List<string> stringLog = new List<string>();
	static int LogMaxLine = 30;

	public static void addLog(string logstring)
	{
		stringLog.Add(logstring);
		
		GameObject logText = GameObject.Find ("LogText");
		if(logText)
		{
			logText.GetComponent<UnityEngine.UI.Text> ().text = "";
			for (int i = 0; i < stringLog.Count; ++i)
			{
				logText.GetComponent<UnityEngine.UI.Text> ().text += stringLog[i] +"\n";
			}
		}
		
		logLineCount++;
		
		if (logLineCount > LogMaxLine)
		{
			stringLog.RemoveAt(0);
		}
	}
	
	public static void clearLog()
	{
		stringLog.Clear();
		
		GameObject logText = GameObject.Find ("LogText");
		if(logText)
			logText.GetComponent<UnityEngine.UI.Text> ().text = "";
		
		logLineCount = 0;
	}
}

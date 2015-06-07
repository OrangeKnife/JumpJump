using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Utils {

	static int logLineCount = 0;
	static List<string> stringLog = new List<string>();
	static int LogMaxLine = 20;

	static string appId;

	public static bool bDebug;
	public static void addLog(string logstring)
	{
		if (!bDebug)
			return;

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

	public static void rateGame()
	{
		#if UNITY_ANDROID
		appId = "com.JunshengYao.ColorJump";
		Application.OpenURL("market://details?id="+appId);
		#elif UNITY_IPHONE
		appId = "id992004670";
		Application.OpenURL("itms-apps://itunes.apple.com/app/"+appId);
		#endif
	}

	public static Texture2D LoadPNG(int w, int h,string filePath) {
		
		Texture2D tex = null;
		byte[] fileData;
		
		if (System.IO.File.Exists(filePath))     {
			fileData = System.IO.File.ReadAllBytes(filePath);
			tex = new Texture2D(w, h);
			tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
		}
		return tex;
	}

	public static Texture2D ScaleTexture(Texture2D source,int targetWidth,int targetHeight) {
		Texture2D result=new Texture2D(targetWidth,targetHeight,source.format,false);
		
		for (int i = 0; i < result.height; ++i) {
			for (int j = 0; j < result.width; ++j) {
				Color newColor = source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
				result.SetPixel(j, i, newColor);
			}
		}
		
		result.Apply();
		return result;
	}

	public static void forceAddToken(int i)
	{
		Soomla.Store.StoreInventory.GiveItem(ColorJumpStoreAssets.ONE_FREEGIFT_TOKEN.ItemId,i);
	}
	
}

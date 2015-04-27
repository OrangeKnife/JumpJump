using UnityEngine;
public class SceneManager
{
	public static void OpenScene(string newSceneName)
	{
		Debug.Log("Open Scene:" +  newSceneName);
		Application.LoadLevel(newSceneName);
	}
}

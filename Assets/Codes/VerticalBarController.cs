using UnityEngine;
using System.Collections;

public class VerticalBarController : MonoBehaviour
{
	Camera myCam;
	// Use this for initialization
	public bool left,right;
	SpriteRenderer spriteRenderer;
		 
	void Start ()
	{
		spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
		Vector2 screenPos = new Vector2 ();
		if (left)
			screenPos.x = 0; // :D
		else if (right)
			screenPos.x = Screen.width;

		screenPos.y = Screen.height / 2;

		myCam = GameObject.Find("Main Camera").GetComponent<Camera>();
		Vector3 p = myCam.ScreenToWorldPoint(new Vector3(screenPos.x,screenPos.y, 10)); // 10 means at the z = 0, which camera is at -10
		p.y = 0f;//I want it follow camera
		p.z = 10f;//I want it follow camera
		gameObject.transform.position = p;
	}

	public void changeColor(Color c, float alpha)
	{
		c.a = alpha;
		spriteRenderer.color = c;
	}
}


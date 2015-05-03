using UnityEngine;
using System.Collections;

public class MaterialScrollingController : MonoBehaviour {

	private SpriteRenderer barSpriteRenderer;
	private Transform playerTransform;

	private float scrollSpeed = 0.1f;
	private Vector2 savedOffset;

	private float currentOffset;

	BarController barController;

	void Start () 
	{
		barSpriteRenderer = GetComponent<SpriteRenderer>();

		savedOffset = barSpriteRenderer.material.mainTextureOffset;

		barController = GetComponent<BarController> ();
	}
	
	void Update () 
	{
		currentOffset = currentOffset + (Time.deltaTime * scrollSpeed);
		//float x = Mathf.Repeat (Time.time * scrollSpeed, 1);
		float x = Mathf.Repeat (currentOffset + 0.4f, 0.8f) - 0.4f;//need -0.4f --> 0.4f 
		Vector2 offset = new Vector2 (x, savedOffset.y);
		barSpriteRenderer.material.mainTextureOffset = offset;

		barController.NotifyBarColorChanged (offset);
	}
	
	void OnDisable () 
	{
		barSpriteRenderer.material.mainTextureOffset = savedOffset;
	}

	public void SetScrollingSpeed(float speed)
	{
		scrollSpeed = speed;
	}

	 
}

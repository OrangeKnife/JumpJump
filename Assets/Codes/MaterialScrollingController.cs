using UnityEngine;
using System.Collections;

public class MaterialScrollingController : MonoBehaviour {

	private SpriteRenderer barSpriteRenderer;

	private float scrollSpeed = 0.05f;

	private float srollSpeedMultiplier = 1f;

	private Vector2 savedOffset;

	private float currentOffset;

	BarController barController;

	void Start () 
	{
		barSpriteRenderer = GetComponent<SpriteRenderer>();

		savedOffset = barSpriteRenderer.material.mainTextureOffset;
		currentOffset = savedOffset.x;
		barController = GetComponent<BarController> ();
	}
	
	void Update () 
	{
		currentOffset = currentOffset + (Time.deltaTime * scrollSpeed * srollSpeedMultiplier);
		//float x = Mathf.Repeat (Time.time * scrollSpeed, 1);
		float x = Mathf.Repeat (currentOffset + 7f / 16f, 8f / 16f) - 7f / 16f;
		Vector2 offset = new Vector2 (x, savedOffset.y);
		barSpriteRenderer.material.mainTextureOffset = offset;

		barController.NotifyBarColorChanged (offset);
	}
	

	public void SetScrollingSpeed(float speed)
	{
		scrollSpeed = speed;
	}

	public void SetScrollingSpeedMultiplier(float mul)
	{
		srollSpeedMultiplier = mul;
	}
	 
}

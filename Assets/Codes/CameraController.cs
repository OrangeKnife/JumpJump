using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	
	//public float smooth = 5;
	public Vector3 CamOffset;
	public bool interpolation;
	public float interpSpeed;

	private GameObject player;
	private Transform playerTransform;
	//private PlayerController playerCon;


	void Start () 
	{
	}

	public void ResetCamera(GameObject inPlayer, bool forceBackZero = false)
	{
		if (forceBackZero) {
			transform.position = new Vector3 (0, 0, -10) + CamOffset;
		}

		if (inPlayer != null) {
			player = inPlayer;
			playerTransform = player.transform;
			transform.position = new Vector3 (playerTransform.position.x, playerTransform.position.y, -10) + CamOffset;
		}
	}

	void Update () 
	{
		if (player == null)
			return;

		if (transform.position.y < playerTransform.position.y) {
			if (interpolation)
				transform.position = Vector3.Lerp (transform.position, new Vector3 (playerTransform.position.x, playerTransform.position.y, -10) + CamOffset, interpSpeed);
			else
				transform.position = new Vector3 (playerTransform.position.x, playerTransform.position.y, -10);
		}

	}
	
}

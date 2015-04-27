using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	
	//public float smooth = 5;
	public Vector3 CamOffset;
	public bool interpolation;
	public float interpSpeed;

	private GameObject player;
	private Transform playerTransform;
	private PlayerController playerCon;
	
	GameManager gameMgr;

	// Use this for initialization
	void Start () 
	{
		gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();
	}

	public void ResetCamera(GameObject inPlayer)
	{
		player = inPlayer;
		playerTransform = player.transform;
		transform.position = new Vector3 (playerTransform.position.x, playerTransform.position.y, -10) + CamOffset;
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

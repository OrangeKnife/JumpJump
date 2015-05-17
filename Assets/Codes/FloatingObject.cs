using UnityEngine;
using System.Collections;

public class FloatingObject : MonoBehaviour
{
	public Vector3 movingVelocity;
	public bool moving,reachEdgeDie,reachEdgeComeBack;
	public float leftEdge,rightEdge;
	
	void Start ()
	{
		movingVelocity = movingVelocity * Random.Range (0.5f, 1f);
		gameObject.transform.localScale = gameObject.transform.localScale * Random.Range (0.8f, 1.2f);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (moving) {
			gameObject.transform.position = gameObject.transform.position + movingVelocity * Time.deltaTime;

			if (gameObject.transform.position.x < leftEdge || gameObject.transform.position.x > rightEdge)
			{
				if(reachEdgeDie)
				{
					GameObject.Destroy(this);
					return;
				}

				if(reachEdgeComeBack)
					movingVelocity = movingVelocity * -1;
			}
		}
	}
}


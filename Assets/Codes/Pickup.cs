using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pickup : MonoBehaviour
{
	public bool gainLife;
	public int gainLifeNum;

	public bool slowTime;
	public float timescale;
	public float slowTimeRecoverySpeed;
	public AudioClip slowTimeLoopSound;

	public bool AddJump;
	public int AddJumpNum;
	public float lifespanAfterPickup;

	public bool UnlockHardMode;
	public bool TutorialFinish;

	public bool GiveToken;
	public int GiveTokenNum;

	public List<AudioClip> audioClips = new List<AudioClip>();

	AudioSource audioSource;
	bool picked = false;

	public string popupMessage;
	public Vector3 popUpTextOffset;
	public GUIStyle popupStyle;

	public float sfxVolume = 0.2f;

	void Start ()
	{
		audioSource = GetComponent<AudioSource> ();
		audioSource.volume = sfxVolume;

	}
	
	// Update is called once per frame
	void Update () 
	{

	}

	void OnTriggerEnter2D(Collider2D  other) 
	{
		if (!picked && other.gameObject.tag == "Player") {
			if(other.gameObject.GetComponent<PlayerController> ().Pickup (this))
			{
				OnPickedUp();
			}
		}
	}

	void OnPickedUp()
	{
		picked = true;
		audioSource.clip = audioClips [0];
		audioSource.Play ();
		GetComponent<SpriteRenderer>().enabled = false;
		Invoke("DestoryDelay",lifespanAfterPickup);

	}

	void DestoryDelay()
	{
		Destroy (gameObject);
	}
}


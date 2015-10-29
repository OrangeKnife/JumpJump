using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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

    private event EventHandler onPickedUp = delegate { };

	void Start ()
	{
		audioSource = GetComponent<AudioSource> ();
		audioSource.volume = sfxVolume;

	}
	
	// Update is called once per frame
	void Update () 
	{

	}

    public void addOnPickedUpEventHandler(EventHandler eh)
    {
        onPickedUp += eh;
    }

    public void removeOnPickedUpEventHandler(EventHandler eh)
    {
        onPickedUp -= eh;
    }

	void OnTriggerEnter2D(Collider2D  other) 
	{
        if (onPickedUp != null)
        {
            onPickedUp(this, EventArgs.Empty);
        }
        PostPickedUp();
    }

	void PostPickedUp()
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


using UnityEngine;
using System.Collections;

public class Audios : MonoBehaviour {

	private AudioSource audioSource;

	public void Awake() {
		audioSource=gameObject.AddComponent<AudioSource>();
	}
	public void playSound(string type)
	{
		/*
		audioSource.clip = _audioClip;
		audioSource.Play ();
		*/
	}
}

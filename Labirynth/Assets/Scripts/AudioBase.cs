using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBase : MonoBehaviour {

	[SerializeField]
	private AudioClip gameComplete = null;

	[SerializeField]
	private AudioSource source = null;

	public void playSoundGameComplete()
	{
		if (source.isPlaying) {
			source.Stop ();
		}
		source.PlayOneShot (gameComplete);
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

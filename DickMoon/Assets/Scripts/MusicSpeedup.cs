﻿using UnityEngine;
using System.Collections;

public class MusicSpeedup : MonoBehaviour {
	AudioSource source;

	// Use this for initialization
	void Start () {
		source = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		source.pitch = (Time.timeScale - 1) / 20 + 1;
	}
}

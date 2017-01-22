using UnityEngine;
using System.Collections;

public class WaveSound : MonoBehaviour
{
	protected static WaveSound instance;
	
	public AudioSource source;

	public void Start()
	{
		instance = this;
	}

	public static void Play()
	{
		instance.source.Play();
	}
}
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class Spawnable
{
	public GameObject Prefab;
}

public class ChunkLoader : MonoBehaviour
{
	public static readonly float ANGLE_BETWEEN_CHUNKS = Mathf.PI / 12f * Mathf.Rad2Deg;
	public static readonly int SPAWNS_BEFORE_CLEANUP = 15;
	public static readonly float WATER_CHANCE = 0.45f;
	public static readonly float BUILDING_CHANCE = 0.8f;
	public static readonly float BOAT_CHANCE = 0.1f;

	public Transform EarthRoot;
	public Transform ReferencePoint;
	public Spawnable[] WaterChunks;
	public Spawnable[] LandChunks;
	public Spawnable[] WaterLandmarks;
	public Spawnable[] Landmarks;

	protected int numSpawns = 0;
	protected Queue<Transform> cleanupQueue = new Queue<Transform>();

	protected Quaternion angleAtLastPlace = Quaternion.identity;

	protected void Spawn(Spawnable s, bool isWedge)
	{
		Transform t = GameObject.Instantiate<GameObject>(s.Prefab).transform;
		t.SetParent(EarthRoot, false);
		t.position = ReferencePoint.transform.position;
		t.rotation = Quaternion.identity;
		if (isWedge)
		{
			t.localScale = Vector3.one * 0.95f;
		}
		cleanupQueue.Enqueue(t);
	}

	protected void Cleanup(Transform t)
	{
		if (t != null)
		{
			GameObject.Destroy(t.gameObject);
		}
	}

	public void Spawn()
	{
		Spawnable wedge;
		Spawnable landmark = Landmarks[0];
		if (UnityEngine.Random.Range(0f, 1f) < WATER_CHANCE)
		{
			wedge = WaterChunks[UnityEngine.Random.Range(0, WaterChunks.Length)];
			if (UnityEngine.Random.Range(0f, 1f) < BOAT_CHANCE)
			{
				landmark = WaterLandmarks[UnityEngine.Random.Range(0, WaterLandmarks.Length)];
			}
		}
		else
		{
			wedge = LandChunks[UnityEngine.Random.Range(0, LandChunks.Length)];
			if (UnityEngine.Random.Range(0f, 1f) < BUILDING_CHANCE)
			{
				landmark = Landmarks[UnityEngine.Random.Range(1, Landmarks.Length)];
			}
		}

		Spawn(wedge, true);
		Spawn(landmark, false);
		angleAtLastPlace = EarthRoot.rotation;

		numSpawns ++;

		if (numSpawns > SPAWNS_BEFORE_CLEANUP)
		{
			Cleanup(cleanupQueue.Dequeue());
			Cleanup(cleanupQueue.Dequeue());
		}
	}

	protected void Prewarm()
	{
		for (int i = 0; i < SPAWNS_BEFORE_CLEANUP; i++)
		{
			Spawn();
			EarthRoot.transform.Rotate(0f, 0f, ANGLE_BETWEEN_CHUNKS);
		}
		EarthRoot.transform.Rotate(0f, 0f, -ANGLE_BETWEEN_CHUNKS);
		angleAtLastPlace = EarthRoot.transform.rotation;
	}

	public void Start()
	{
		Prewarm();
	}

	public void Update()
	{
		if (Quaternion.Angle(angleAtLastPlace, EarthRoot.rotation) > ANGLE_BETWEEN_CHUNKS)
		{
			Spawn();
		}
	}

    public void OnDestroy()
    {
        while( cleanupQueue.Count > 0 )
        {
            Cleanup(cleanupQueue.Dequeue());
        }
    }
}
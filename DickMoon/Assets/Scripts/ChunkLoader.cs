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

	public Transform EarthRoot;
	public Transform ReferencePoint;
	public Spawnable[] LandChunks;
	public Spawnable[] Landmarks;

	protected int numSpawns = 0;
	protected Queue<Transform> cleanupQueue = new Queue<Transform>();

	protected Quaternion angleAtLastPlace = Quaternion.identity;

	protected void Spawn(Spawnable s)
	{
		Transform t = GameObject.Instantiate<GameObject>(s.Prefab).transform;
		t.SetParent(EarthRoot, false);
		t.position = ReferencePoint.transform.position;
		t.rotation = Quaternion.identity;
		cleanupQueue.Enqueue(t);
	}

	public void Update()
	{
		if (Quaternion.Angle(angleAtLastPlace, EarthRoot.rotation) > ANGLE_BETWEEN_CHUNKS)
		{
			int landIndex = UnityEngine.Random.Range(0, LandChunks.Length);
			int markIndex = UnityEngine.Random.Range(0, Landmarks.Length);
			if (landIndex == 2)
			{
				markIndex = 6;
			}

			Spawn(LandChunks[landIndex]);
			Spawn(Landmarks[markIndex]);
			angleAtLastPlace = EarthRoot.rotation;

			numSpawns ++;

			if (numSpawns > SPAWNS_BEFORE_CLEANUP)
			{
				GameObject.Destroy(cleanupQueue.Dequeue().gameObject);
				GameObject.Destroy(cleanupQueue.Dequeue().gameObject);
			}
		}
	}
}
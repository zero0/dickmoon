using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class Spawnable
{
	public GameObject Prefab;
}

public class ChunkLoader : MonoBehaviour
{
	public static readonly float ANGLE_BETWEEN_CHUNKS = Mathf.PI / 6f * Mathf.Rad2Deg;

	public Transform EarthRoot;
	public Transform ReferencePoint;
	public Spawnable[] LandChunks;
	public Spawnable[] Landmarks;

	protected Quaternion angleAtLastPlace = Quaternion.identity;

	protected void Spawn(Spawnable s)
	{
		Transform t = GameObject.Instantiate<GameObject>(s.Prefab).transform;
		t.SetParent(EarthRoot, false);
		t.position = ReferencePoint.transform.position;
		t.rotation = Quaternion.identity;
	}

	public void Update()
	{
		if (Quaternion.Angle(angleAtLastPlace, EarthRoot.rotation) > ANGLE_BETWEEN_CHUNKS)
		{
			Spawn(LandChunks[0]);
			Spawn(Landmarks[0]);
			angleAtLastPlace = EarthRoot.rotation;
		}
	}
}
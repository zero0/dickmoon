using UnityEngine;
using System.Collections;

public class EarthController : MonoBehaviour
{
	public static readonly float ROTATION_RATE = Mathf.PI / 9f * Mathf.Rad2Deg;

	public void Update()
	{
		transform.rotation = Quaternion.Euler(0f, 0f, ROTATION_RATE * Time.timeSinceLevelLoad);
	}
}
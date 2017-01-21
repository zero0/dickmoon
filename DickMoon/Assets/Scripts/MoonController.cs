using UnityEngine;
using System.Collections;

public class MoonController : MonoBehaviour
{
	public static readonly float MAX_HEIGHT = 300f;
	public static readonly float MIN_HEIGHT = 100f;
	public static readonly float MAX_VERTICAL_SPEED = 200f;
	public static readonly float LERP_TIME = 0.4f;

	protected float speed;

	public void Update()
	{
		float targetHeight = MAX_HEIGHT;
		if (Input.GetKey(KeyCode.Space))
		{
			targetHeight = MIN_HEIGHT;
		}

		Vector3 position = transform.position;
		position.y = Mathf.SmoothDamp(position.y, targetHeight, ref speed, LERP_TIME, MAX_VERTICAL_SPEED);
		transform.position = position;
	}
}
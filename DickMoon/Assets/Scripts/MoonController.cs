using UnityEngine;
using System.Collections;

public class MoonController : MonoBehaviour
{
	// FIX THIS BASED ON CANVAS SCALER
	public static readonly float MAX_HEIGHT = 250f;
	public static readonly float MIN_HEIGHT = -250f;
	public static readonly float MAX_VERTICAL_SPEED = 2000f;
	public static readonly float LERP_TIME = 0.08f;

	protected float speed;

    private Vector3 _vel = Vector3.zero;

	public void Update()
	{
		float targetHeight = MAX_HEIGHT;
		if (Input.GetMouseButton(0))
		{
			targetHeight = MIN_HEIGHT;
		}

		Vector3 position = transform.localPosition;
		position.y = Mathf.SmoothDamp(position.y, targetHeight, ref speed, LERP_TIME, MAX_VERTICAL_SPEED);
		transform.localPosition = position;
	}
}
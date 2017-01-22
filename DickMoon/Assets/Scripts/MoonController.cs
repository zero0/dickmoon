﻿using UnityEngine;
using System.Collections;

public class MoonController : MonoBehaviour
{
	// FIX THIS BASED ON CANVAS SCALER
	public static readonly float MAX_HEIGHT = 200f;
	public static readonly float MIN_HEIGHT = 100f;
	public static readonly float MAX_VERTICAL_SPEED = 200f;
	public static readonly float LERP_TIME = 0.4f;

	protected float speed;

    private Vector3 _vel = Vector3.zero;

	public void Update()
	{
		float targetHeight = MAX_HEIGHT;
		if (Input.GetKey(KeyCode.Space))
		{
			targetHeight = MIN_HEIGHT;
		}

		Vector3 position = transform.localPosition;
		position.y = Mathf.SmoothDamp(position.y, targetHeight, ref speed, LERP_TIME, MAX_VERTICAL_SPEED);
		transform.localPosition = position;

        //transform.position = Vector3.SmoothDamp(position, targetHeight * Vector3.one, ref _vel, MAX_VERTICAL_SPEED);
	}
}
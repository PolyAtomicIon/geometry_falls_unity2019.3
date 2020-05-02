using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

	public Player target;

	public float smoothSpeed = 0.125f;
	public Vector3 offset;

	void Start()
	{
		// change background color randomly
		//StartCoroutine(CameraBackgroundChanger.beginToChangeColor());
	}

	
	// Smooth Camera follow, follows target
	void FixedUpdate ()
	{
		if( target == null )
			target = FindObjectOfType<Player>();
		
		Vector3 desiredPosition = target.transform.position + offset;
		Vector3 smoothedPosition = Vector3.Lerp(transform.transform.position, desiredPosition, smoothSpeed);
		transform.position = smoothedPosition;

        smoothSpeed += 0.0005f;

		//transform.LookAt(target);
	}

}

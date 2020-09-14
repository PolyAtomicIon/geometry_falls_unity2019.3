using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

	public Player target;
	public float smoothSpeed = 0.125f;
	public Vector3 offset;

	void Start()
	{
		transform.position = new Vector3(0f, 18f, 2f);
	}
	
	// Smooth Camera follow, follows target
	void FixedUpdate ()
	{
		if( target == null )
			target = FindObjectOfType<Player>();
		else{

			Vector3 desiredPosition = target.transform.position + offset;

			if( desiredPosition.y > transform.position.y && !target.is_game_over ){
				transform.position = new Vector3(0f, 18f, 2f);
			}
			else{
				Vector3 smoothedPosition = Vector3.Lerp(transform.transform.position, desiredPosition, smoothSpeed);
				transform.position = smoothedPosition;

				smoothSpeed += 0.0005f;
			}
			
		}
	}

}

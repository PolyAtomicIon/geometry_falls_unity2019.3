using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

	public Player target;

	public float smoothSpeed = 0.125f;
	public Vector3 offset;

	// float gap = 40f;

	void Start()
	{
		// change background color randomly
		//StartCoroutine(CameraBackgroundChanger.beginToChangeColor());
	}

	// IEnumerator animate(){
	// 	yield return new WaitForSeconds(0.15f);
	// 	transform.position = new Vector3(transform.position.x, transform.position.y - gap, transform.position.z);
	// }

	// public void Animation(){
	// 	StartCoroutine(animate());
	// }
	
	// Smooth Camera follow, follows target
	void FixedUpdate ()
	{
		if( target == null )
			target = FindObjectOfType<Player>();
		
		Vector3 desiredPosition = target.transform.position + offset;
		Vector3 smoothedPosition = Vector3.Lerp(transform.transform.position, desiredPosition, smoothSpeed);
		transform.position = smoothedPosition;

        smoothSpeed += 0.0005f;
		// gap += 0.005f;

		//transform.LookAt(target);
	}

}

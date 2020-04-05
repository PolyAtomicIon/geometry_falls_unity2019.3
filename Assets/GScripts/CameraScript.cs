using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

	public Player target;

	public float smoothSpeed = 0.125f;
	public Vector3 offset;

	public float transitionTimeInSec = 2f;

	private bool changingColor = false;

	private Color color1;
	private Color color2;


	void Start()
	{
		StartCoroutine(beginToChangeColor());
	}

	IEnumerator beginToChangeColor()
	{
		Camera cam = Camera.main;
		color1 = Random.ColorHSV(Random.value, Random.value);
		color2 = Random.ColorHSV(Random.value, Random.value);

		while (true)
		{
			//Lerp Color and wait here until that's done
			yield return lerpColor(cam, color1, color2, transitionTimeInSec);

			//Generate new color
			color1 = cam.backgroundColor;
			color2 = Random.ColorHSV(Random.value, Random.value);
		}
	}

	IEnumerator lerpColor(Camera targetCamera, Color fromColor, Color toColor, float duration)
	{
		if (changingColor)
		{
			yield break;
		}
		changingColor = true;
		float counter = 0;

		while (counter < duration)
		{
			counter += Time.deltaTime;

			float colorTime = counter / duration;
			//Debug.Log(colorTime);

			//Change color
			targetCamera.backgroundColor = Color.Lerp(fromColor, toColor, counter / duration);
			//Wait for a frame
			yield return null;
		}
		changingColor = false;
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

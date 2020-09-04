using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBackgroundChanger : MonoBehaviour {

	public static float transitionTimeInSec = 2f;

	private static bool changingColor = false;

	private static Color color1;
	private static Color color2;

    void Start(){
		changingColor = false;
    }

	public static IEnumerator beginToChangeColor()
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

	public static IEnumerator lerpColor(Camera targetCamera, Color fromColor, Color toColor, float duration)
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

	void Update(){
		if( !changingColor ){	
			// change background color randomly
			StartCoroutine(beginToChangeColor());
		}
	}

}
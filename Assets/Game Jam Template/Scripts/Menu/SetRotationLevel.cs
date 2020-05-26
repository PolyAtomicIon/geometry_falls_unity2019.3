using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetRotationLevel : MonoBehaviour {

	public Manager game_manager;					//Used to hold a reference to the AudioMixer mainMixer


	//Call this function and pass in the float parameter musicLvl to set the volume of the AudioMixerGroup Music in mainMixer
	public void SetRBRotationLevel(float lvl)
	{
		game_manager.rotation_lvl = lvl*100f + 2500f;
        Debug.Log(game_manager.rotation_lvl);
	}
/*
	public void SetRBRotationLevel(float lvl)
	{
		game_manager.ang_drag = lvl*100f + 3000f;
        Debug.Log(game_manager.rotation_lvl);
	}*/

}

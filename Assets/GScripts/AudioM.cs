using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine;
using Random=System.Random;
using Random2=UnityEngine.Random;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;
using System;   


public class AudioM : MonoBehaviour
{   
    public AudioSource bgMusic;
    public List<AudioClip> audios;
    public List<AudioSource> soundEffects;

    public void SetAudio(){
        AudioListener audioListener = GetComponent<AudioListener>(); 
        audioListener.enabled = ( audioListener.enabled ^ true );
    }

    public void hit(){
        // hit sound
        soundEffects[1].Play();
    }

    public void pass(){
        // pass sound
        soundEffects[0].Play();
    }

    void Start(){

        // AUDIO Settings

        int audioID = PlayerPrefs.GetInt("bgAudioID");
        bgMusic.clip = audios[audioID];

        bgMusic.Play();

        if( PlayerPrefs.GetInt("isAudio") == 0 ){
            bgMusic.volume = 0.0f;
        } 

    }

}

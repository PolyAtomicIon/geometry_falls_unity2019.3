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

    int passSoundIndex = 0;
    public List<AudioSource> passSounds;
    public AudioSource hitSound;

    public AudioSource levelPassSound;

    public void SetAudio(){
        AudioListener audioListener = GetComponent<AudioListener>(); 
        audioListener.enabled = ( audioListener.enabled ^ true );
    }

    public void hit(){
        // hit sound
        hitSound.Play();
    }

    public void levelPass(){
        levelPassSound.Play();
    }

    public void pass(){
        // pass sound
        
        passSounds[passSoundIndex].Play();
        passSoundIndex += 1;
        passSoundIndex %= passSounds.Count;
    }

    void Start(){

        // AUDIO Settings

        // int audioID = PlayerPrefs.GetInt("bgAudioID");
        int audioID = ThreadSafeRandom.ThisThreadsRandom.Next(audios.Count);
        bgMusic.clip = audios[audioID];

        bgMusic.Play();

        if( PlayerPrefs.GetInt("isAudio") == 0 ){
            bgMusic.volume = 0.0f;
        } 

    }

}

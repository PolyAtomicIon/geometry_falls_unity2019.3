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

public class Highscore : MonoBehaviour
{
    
    static public Highscore instance;
        
    void Awake(){ 
        instance = this;
    } 

    public static int getHighscore(){
        return PlayerPrefs.GetInt("high_score");
    }

    // if local highscore is greater than online, in case player beat the highscore in offline mode
    public static void setHighscore_fromOffline(int score){

        int cur_record = getHighscore();
        
        if( cur_record > score ){
            instance.StartCoroutine(setHighscore_request());  
        }

    }

    public static void setHighscore(int score){

        int cur_record = getHighscore();

        if( cur_record < score ){
            PlayerPrefs.SetInt("high_score", score);
        }

        instance.StartCoroutine(setHighscore_request());

    }

    static IEnumerator setHighscore_request()
    {

        int highscore = getHighscore();

        WWWForm form = new WWWForm();
        form.AddField("value", highscore); 

        string url = "http://94.247.128.162/api/game/high_score/";

        using (UnityWebRequest www = UnityWebRequest.Post(url, form ))
        {
            
            www.SetRequestHeader("Authorization", Manager.get_token());    
            yield return www.SendWebRequest();

            Debug.Log("Highscore: Request sent!");
        
            Debug.Log(www.isNetworkError);      
            Debug.Log(www.isHttpError); 
            
            if ( !www.isNetworkError && !www.isHttpError ){
                Debug.Log("Highscore has been set");
            }
            else{
                Debug.Log("Problem with setting the highscore");
            }

        }
    }

}

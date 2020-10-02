using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;
using SimpleJSON;
using UnityEngine.UI;

public class ApplicationManager : MonoBehaviour {
    public GameObject noInternetConnection_panel;
   
    void Start(){

    }

    void Update(){
        
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.Log("Error. Check internet connection!");
            noInternetConnection_panel.SetActive(true);
        }
        else{
            noInternetConnection_panel.SetActive(false);
        }

    }

}

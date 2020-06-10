using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class startScene : MonoBehaviour
{

    void Start()
    {

        if( PlayerPrefs.GetInt("isLaunched") == 0 ){
            PlayerPrefs.SetInt("isLaunched", 1);
            SceneManager.LoadScene("Tutorial");
        }
        else{
            SceneManager.LoadScene("MainMenu");
        }

    }

}

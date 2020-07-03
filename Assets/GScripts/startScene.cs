using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class startScene : MonoBehaviour
{

    IEnumerator ShowLogo(){
        yield return new WaitForSeconds(0.5f);
        
        if( PlayerPrefs.GetInt("isLaunched") == 0 ){
            PlayerPrefs.SetInt("isLaunched", 1);
            SceneManager.LoadScene("Tutorial");
        }
        else{
            SceneManager.LoadScene("MainMenu");
        }

    }

    void Start()
    {

        PlayerPrefs.SetInt("id", -1);
           
        StartCoroutine( ShowLogo() );

    }

}

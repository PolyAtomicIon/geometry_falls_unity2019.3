using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class startScene : MonoBehaviour
{
    
    public Animator SplashScreenAnimator;

    void Start()
    {
        SplashScreenAnimator.Play("Login to Loading");
        StartCoroutine(LoadYourAsyncScene());
    }

    IEnumerator LoadYourAsyncScene()
    {
        
        yield return new WaitForSeconds(0.25f);
        string SceneName = "newMainScene";

        // if( PlayerPrefs.GetInt("isLaunched") == 0 ){
        //     PlayerPrefs.SetInt("isLaunched", 1);
        //     // SceneManager.LoadScene("Tutorial");
        //     SceneName = "Tutorial";
        // }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneName);

        // Wait until the asynchronous scene fully loads    
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

}

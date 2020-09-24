using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameTutorial : MonoBehaviour
{

    public GameObject Tutorial_panel;

    bool drag = false;
    bool tutorialActive = true;

    IEnumerator Tutorial(){
        yield return new WaitForSeconds(1.2f);
        tutorialActive = true;
        Tutorial_panel.SetActive(true);
        Time.timeScale = 0;
    }

    void stopTutorial(){
        tutorialActive = false;
        Tutorial_panel.SetActive(false);
        Time.timeScale = 1;
    }

    void Start()
    {
        // PlayerPrefs.SetInt("isLaunched", 0);
        StartCoroutine( Tutorial() ); 
        drag = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
            drag = true;

        if( drag && tutorialActive ){
            stopTutorial();
        }

    }
}

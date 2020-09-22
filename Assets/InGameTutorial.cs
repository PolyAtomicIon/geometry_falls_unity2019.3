using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameTutorial : MonoBehaviour
{

    public GameObject Tutorial_panel;

    IEnumerator Tutorial(){
        yield return new WaitForSeconds(2f);
        Tutorial_panel.SetActive(true);
        Time.timeScale = 0.00001f;
        yield return new WaitForSeconds(0.00002f);  
        Time.timeScale = 1f;
        Debug.Log("all the pain");
        Tutorial_panel.SetActive(false);
    }

    void Start()
    {
        PlayerPrefs.SetInt("isLaunched", 0);
    }

    // Update is called once per frame
    void Update()
    {
        if( PlayerPrefs.GetInt("isLaunched") == 0 ){
            PlayerPrefs.SetInt("isLaunched", 1);
                
            StartCoroutine(Tutorial());
        }
    }
}

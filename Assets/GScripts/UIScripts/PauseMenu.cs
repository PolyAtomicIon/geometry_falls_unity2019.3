using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    public GameObject Pause_panel;
    public GameObject Pause_button;

    public void Pause(){
        Pause_panel.SetActive(true);
        Pause_button.SetActive(false);
        Time.timeScale = 0f;
    }

    public void Resume(){
        Time.timeScale = 1f;
        Pause_button.SetActive(true);
        Pause_panel.SetActive(false);
    }


}

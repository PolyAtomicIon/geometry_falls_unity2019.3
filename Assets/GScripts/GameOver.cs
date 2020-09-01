using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOver : MonoBehaviour
{

    public Manager game_manager;

    // buttons: restart, game_over
    public GameObject section;

    public GameObject spinning_object;

    void Start()
    {
        game_manager = FindObjectOfType<Manager>();
    }

    public void game_over(int score, bool isEvent = false){
        
        Time.timeScale = 1f;

        if( isEvent )
            spinning_object.SetActive(true);

        section.SetActive(true);
    }

    void Update()
    {
        
    }
}

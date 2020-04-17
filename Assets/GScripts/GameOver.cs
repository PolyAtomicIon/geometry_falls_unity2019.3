using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOver : MonoBehaviour
{

    public Manager game_manager;

    public TextMeshProUGUI scoreLabel;

    public GameObject section;

    public GameObject spinning_object;

    void Start()
    {
        game_manager = FindObjectOfType<Manager>();
    }

    public void game_over(int score){
        
        Time.timeScale = 1f;
        spinning_object.SetActive(true);
        section.SetActive(true);

        string zero = "";
        if( score < 10 ){
            zero = "0";
        }
        scoreLabel.text = "Level: " + zero + score.ToString();
    }
// Test Comment
    void Update()
    {
        
    }
}

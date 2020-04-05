using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameOver : MonoBehaviour
{

    public Manager game_manager;

    public TextMeshProUGUI scoreLabel;

    public GameObject section;

    void Start()
    {
        game_manager = FindObjectOfType<Manager>();
        section = GetComponent<GameObject>();
    }

    public void game_over(int score){
        section.SetActive(true);
        string zero = "";
        if( score < 10 ){
            zero = "0";
        }
        scoreLabel.text = "Level: " + zero + score.ToString();
    }

    void Update()
    {
        
    }
}

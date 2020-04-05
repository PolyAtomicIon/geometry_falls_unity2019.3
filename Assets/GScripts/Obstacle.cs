using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, IPooledObject
{

   
    private Manager game_manager;
    private Player player;

    void Start(){
        player = FindObjectOfType<Player>();
        game_manager = FindObjectOfType<Manager>();
    }

    void OnObjectSpawn(){
        return;
    }

    void Update(){
        
        // check player's score

    }

}

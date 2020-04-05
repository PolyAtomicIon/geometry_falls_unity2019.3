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

    public void OnObjectSpawn(){
        return;
    }

    void Update(){
        
        if( player.get_position_y_axis() < transform.position.y ){
            game_manager.increment_score();
            gameObject.SetActive(false);
        }

    }

}

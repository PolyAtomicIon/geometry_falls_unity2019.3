using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, IPooledObject
{

   
    private Manager game_manager;
    private Player player;

    private Renderer render;

    ObjectPooler objectPooler;

    void Start(){
        player = FindObjectOfType<Player>();
        game_manager = FindObjectOfType<Manager>();
        render = GetComponent<Renderer>();
        
        objectPooler = ObjectPooler.Instance;
    }

    public void OnObjectSpawn(){
        Start();
    }

    void Update(){
        
        if( player.get_position_y_axis() < -10f + transform.position.y ){
            game_manager.increment_score();
            // move it down
            transform.position = objectPooler.new_obstacle_position();
        }

    }

}

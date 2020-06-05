using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, IPooledObject
{

   
    private Manager game_manager;
    private Renderer render;
    private bool is_active = true, done = false;

    private float dissolveLevel = -1.25f;

    int obstacle_index;

    ObjectPooler objectPooler;

    /* WARNING!!! */
    /* GOVNOKOD */
    Vector3 initialPos;
    /* END */

    void Start(){
        game_manager = FindObjectOfType<Manager>();
        objectPooler = ObjectPooler.Instance;
        gameObject.layer = LayerMask.NameToLayer("Obstacle");
    }

    public void OnObjectSpawn(){
        Start(); 
        is_active = true;
        gameObject.SetActive(is_active);

        initialPos = transform.position;

        // 5f for safety
        float temp_obstacle_index = (transform.position.y + 5f) / game_manager.gap;
        obstacle_index = (int) temp_obstacle_index - 1;
    }

    void Update(){

        if( game_manager.current_obstacle > obstacle_index ){
            is_active = false;
        }

        if( !is_active ){
            transform.Translate(Vector3.forward * Time.deltaTime * 50);
        }

        if( !is_active && transform.position.y - initialPos.y >= 100f ){
            gameObject.SetActive(false);
        }

    }



}

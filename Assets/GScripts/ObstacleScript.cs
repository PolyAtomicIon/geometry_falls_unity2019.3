using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine;
using System;   


public class ObstacleScript : MonoBehaviour, IPooledObject
{

   
    private Manager game_manager;
    private Renderer render;
    private Collider collider;
    private bool is_active = true, done = false;

    private float dissolveLevel = -1.25f;

    int obstacle_index;

    ObjectPooler objectPooler;

    /* WARNING!!! */
    /* GOVNOKOD */
    Vector3 initialPos;
    /* END */

    bool rotated = false;

    void Start(){
        game_manager = FindObjectOfType<Manager>();
        
        collider = GetComponent<Collider>();

        objectPooler = ObjectPooler.Instance;
        gameObject.layer = LayerMask.NameToLayer("Obstacle");
    }

    public void OnObjectSpawn(){
        Start(); 

        collider.enabled = true;

        is_active = true;
        rotated = false;
        gameObject.SetActive(is_active);

        initialPos = transform.position;

        // 5f for safety
        float temp_obstacle_index = (transform.position.y + 5f) / game_manager.gap;
        obstacle_index = (int) temp_obstacle_index - 1;
    }

    private IEnumerator SetRotation(Vector3 newRotation, float duration = 1.0f)
    { 
        Quaternion from = transform.rotation;
        
        Transform target = transform;
        target.eulerAngles = newRotation;
        Quaternion to = target.rotation;

        float elapsed = 0.0f;
        while( elapsed < duration )
        {
        transform.rotation = Quaternion.Slerp(from, to, elapsed / duration );
        elapsed += Time.deltaTime;
        yield return null;
        }
        transform.rotation = to;
    }

    void obstaclePassAnimation(){
        collider.enabled = false;

        StartCoroutine( SetRotation(new Vector3(-180, 0, 0), 0.175f) ); 

        transform.Translate(new Vector3(1, 0, 0) * Time.deltaTime * 20f);
    }

    void GetInitialAngleAndSetInitialRotation(){
        rotated = true;

        float angle = game_manager.obstacle_angles[obstacle_index];
        Vector3 initial_rotation = new Vector3(-90, angle, 0f);

        StartCoroutine( SetRotation(initial_rotation, 0.5f) );
    }

    void Update(){

        if( (game_manager.current_obstacle == obstacle_index || obstacle_index == 0 ) && !rotated ){
            GetInitialAngleAndSetInitialRotation();
        }

        if( game_manager.current_obstacle > obstacle_index ){
            is_active = false;
        }

        if( !is_active ){
            obstaclePassAnimation();
        }

        if( !is_active && Math.Abs(transform.position.z) >= 3f ){
            gameObject.SetActive(false);
        }

    }



}

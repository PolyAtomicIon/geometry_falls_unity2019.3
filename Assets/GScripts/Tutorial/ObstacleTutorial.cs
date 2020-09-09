using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine;

public class ObstacleTutorial : MonoBehaviour, IPooledObject
{

   
    private Tutorial game_manager;
    private Renderer render;
    private bool is_active = true, done = false;

    private float dissolveLevel = -1.25f;

    int obstacle_index;

    ObjectPooler objectPooler;

    /* WARNING!!! */
    /* GOVNOKOD */
    Vector3 initialPos;
    /* END */
    PlayerTutorial player;

    void Start(){
        game_manager = FindObjectOfType<Tutorial>();
        objectPooler = ObjectPooler.Instance;
        gameObject.layer = LayerMask.NameToLayer("Obstacle");
        
        is_active = true;
        gameObject.SetActive(is_active);

        initialPos = transform.position;
        player = FindObjectOfType<PlayerTutorial>();
    }

    public void OnObjectSpawn(){
        Start(); 
    }

    void Update(){
        
        if( is_active && player.get_position_y_axis() < transform.position.y - 2.5f ){
            is_active = false;
            GameObject ChildGameObject = this.transform.GetChild(0).gameObject;
            ChildGameObject.SetActive(false);
        }

        if( !is_active ){
            transform.Translate(Vector3.forward * Time.deltaTime * 50);
        }
    }



}

using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, IPooledObject
{

   
    private Manager game_manager;
    private Player player;

    private Renderer render;
    private bool is_active = true;

    ObjectPooler objectPooler;

    void Start(){
        player = FindObjectOfType<Player>();
        game_manager = FindObjectOfType<Manager>();
        render = GetComponent<Renderer>();

        objectPooler = ObjectPooler.Instance;
        gameObject.layer = LayerMask.NameToLayer("Obstacle");
    }

    public void OnObjectSpawn(){
        Start(); 
        is_active = true;
        gameObject.SetActive(is_active);
    }

    void Update(){
        
        if( is_active && player.get_position_y_axis() < transform.position.y - 2.5f ){
            is_active = false;
            game_manager.increment_score();
            //gameObject.SetActive(is_active);
            // disabled moving down
        }
        if( !is_active ){
            // Debug.Log("There is nothing that can stop you~");
            transform.Translate(Vector3.right * Time.deltaTime * 50);
        }

    }

}

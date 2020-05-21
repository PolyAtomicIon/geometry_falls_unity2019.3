using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, IPooledObject
{

   
    private Manager game_manager;
    private Player player;
    private Renderer render;
    private bool is_active = true, done = false;

    private float dissolveLevel = -1.25f;

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

    private IEnumerator DissolveEffect()
    {
        if( dissolveLevel >= 0.25f) 
            done = true;
        dissolveLevel += 0.05f;
        yield return 0.25f;
    }

    void Update(){

        if( game_manager.figure_plane.transform.position.y < transform.position.y ){
            Vector3 position_f = transform.position;
            position_f.y -= 0.1f;
            game_manager.figure_plane.transform.position = position_f;
        }

        if( is_active && player.get_position_y_axis() < transform.position.y - objectPooler.get_gap_between_obstacles() - 2.5f ){    
            Vector3 position_f = transform.position;
            position_f.y -= 0.1f;
            game_manager.figure_plane.transform.position = position_f;
        }
        
        if( is_active && player.get_position_y_axis() < transform.position.y - 2.5f ){
            is_active = false;
          
            // !!! Dissolve effect
            //render.material = game_manager.DissolveMaterial;
            player.increment_score();
            game_manager.increment_score();
            //gameObject.SetActive(is_active);
            // disabled moving down
        }

        // !!! Dissolve effect
        // if( !is_active && !done ){
        //     game_manager.DissolveMaterial.SetFloat("Vector1_DB3F2BFC", dissolveLevel);            
        //     StartCoroutine(DissolveEffect());
        // }

        if( !is_active ){
            // Debug.Log("There is nothing that can stop you~");
            transform.Translate(Vector3.forward * Time.deltaTime * 50);
        }

    }



}

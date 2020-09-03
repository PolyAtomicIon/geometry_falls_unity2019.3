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

    bool rotated = false;

    void Start(){
        game_manager = FindObjectOfType<Manager>();
        objectPooler = ObjectPooler.Instance;
        gameObject.layer = LayerMask.NameToLayer("Obstacle");
    }

    public void OnObjectSpawn(){
        Start(); 
        is_active = true;
        rotated = false;
        gameObject.SetActive(is_active);

        initialPos = transform.position;

        // 5f for safety
        float temp_obstacle_index = (transform.position.y + 5f) / game_manager.gap;
        obstacle_index = (int) temp_obstacle_index - 1;
    }

    private IEnumerator RotateAnimation( float duration = 0.35f )
    {
        float angle = game_manager.obstacle_angles[obstacle_index];
        rotated = true;
        Quaternion startRotation = transform.rotation;
        Transform target = transform;
        target.eulerAngles = new Vector3(-90, angle, 0f);
        Quaternion endRotation = target.rotation;

        for( float t = 0 ; t < Time.deltaTime * 40; t+= Time.deltaTime )
        {
            transform.rotation = Quaternion.Lerp( startRotation, endRotation, t / duration ) ;
            yield return null;
        }

        transform.rotation = endRotation;
    }

    void Update(){

        if( (game_manager.current_obstacle == obstacle_index || obstacle_index == 0 ) && !rotated ){
            StartCoroutine( RotateAnimation() );
        }

        if( game_manager.current_obstacle > obstacle_index ){
            is_active = false;
        }

        if( !is_active ){
            transform.Translate(Vector3.forward * Time.deltaTime * 50f);
        }

        if( !is_active && transform.position.y - initialPos.y >= 100f ){
            gameObject.SetActive(false);
        }

    }



}

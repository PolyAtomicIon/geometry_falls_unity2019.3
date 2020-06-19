using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine;
using System;   
public class Player : MonoBehaviour, IPooledObject
{

    private Rigidbody rb;
    private Transform transform;
    // private float angular_drag = 1.5f;
    private float fall_down_speed = -27f;
    public float acceleration = -0.075f;
    public float speed;
    public Renderer render;

    // private float maxSpeed = 7f;

    // private Vector3 movement;

    private Manager game_manager;

    private int score = 0;

    public float get_position_y_axis(){
        return transform.position.y;
    }

    public float get_velocity_y_axis(){
        return rb.velocity.y;
    }

    private void fallDown(float speed){
        rb.velocity = new Vector3(0, speed, 0);
    }

    public bool dragging = false;
    

    public void increment_score(){
        score += 1;
    }
    
    void OnMouseDown(){
        dragging = false;
    }
    
    void OnMouseUp(){
        dragging = false;
    }

    void Start(){
        Physics.gravity = new Vector3(0, acceleration, 0);    

        rb = GetComponent<Rigidbody>();
        render = GetComponent<Renderer>();
        transform = GetComponent<Transform>();

        rb.centerOfMass = Vector3.zero;
        rb.inertiaTensorRotation = Quaternion.identity;

        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }

    public void OnObjectSpawn(){
        Start();
        game_manager = FindObjectOfType<Manager>();
        game_manager.player = this;
        fall_down_speed = game_manager.fall_down_speed;
        fallDown(fall_down_speed);
    }

    void Update(){

        if ( dragging ){
            
            Vector3 mouseScreenPosition = new Vector3( Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z );
            
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint( mouseScreenPosition );

            mouseWorldPosition.y = Mathf.Min(8.5f, mouseWorldPosition.y);

            // Debug.Log(mouseWorldPosition);

            if( Mathf.Abs(0f - Mathf.Abs(mouseWorldPosition.x)) <= 0.5f && Mathf.Abs(6.2f + mouseWorldPosition.z) <= 0.5f){
                mouseWorldPosition = new Vector3(0.0f, 9.9f, -6.2f);
            }

            // Debug.Log(mouseWorldPosition);
            
            Vector3 relativePos = mouseWorldPosition - transform.position;
            if( relativePos.magnitude > 0f ){
                Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
                transform.rotation = rotation;
            }

        }

    }

    
     void OnCollisionEnter (Collision col)
    {
        // 9th layer Obstacle, check inspector
        if( col.gameObject.layer == 9 ){
            Debug.Log(col.gameObject.name);    
            rb.velocity = new Vector3(0f, 0f, 0f);
            gameObject.SetActive(false);
            game_manager.is_level_started = false;
            game_manager.game_over();
        }
    }

}
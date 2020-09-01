﻿using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine;
using System;   
public class Player : MonoBehaviour, IPooledObject
{

    public Rigidbody rb;
    private Transform transform;

    private float fall_down_speed = -27f;
    private float acceleration = -0.035f;
    public float speed;
    public Renderer render;

    private Manager game_manager;

    private int score = 0;

    [SerializeField] private float rotation_duration = 0.35f;
    private float time = 5;
    public float rotation_degree = 160f;
    private bool rotating = false;

    private bool dragging = false;
    
    // Rotation variables
    public float rotationSpeed = 40000f;
    public float angular_drag = 0.45f;
    float rotX, rotY;
    
    public float value_t = 8f;

    public int hold_time = 0;
    public int hold_time_limit = 8;
    
    CameraScript cameraScript;

    bool game_over = false;

    public void increment_score(){
        score += 1;
    }

    public float get_position_y_axis(){
        return transform.position.y;
    }

    public float get_velocity_y_axis(){
        return rb.velocity.y;
    }

    private void fallDown(float speed){
        rb.velocity = new Vector3(0, speed, 0);
    }
    
    private IEnumerator Rotate( Vector3 angles, float duration = 1.0f )
    {
        rotating = true ;
        Quaternion startRotation = transform.rotation ;
        Quaternion endRotation = Quaternion.Euler( angles ) * startRotation ;

        for( float t = 0 ; t < Time.deltaTime * 10; t+= Time.deltaTime )
        {
            transform.rotation = Quaternion.Lerp( startRotation, endRotation, t / duration ) ;
            yield return null;
        }

        transform.rotation = endRotation;
        rotating = false;
    }

    public void rotate(int direction){

        if( rotating ) return;

        if( direction == 0 ){
            StartCoroutine( Rotate( Vector3.right * rotation_degree, rotation_duration ) );
        }
        if( direction == 2 ){
            StartCoroutine( Rotate( -Vector3.right * rotation_degree, rotation_duration ) );
        }
        if( direction == 3 ){
            StartCoroutine( Rotate( Vector3.up * rotation_degree, rotation_duration ) );
        }
        if( direction == 1 ){
            StartCoroutine( Rotate( -Vector3.up * rotation_degree, rotation_duration ) );
        }
    }

    void Start(){
        Physics.gravity = new Vector3(0, acceleration, 0);    

        rb = GetComponent<Rigidbody>();
        render = GetComponent<Renderer>();
        transform = GetComponent<Transform>();

        cameraScript = FindObjectOfType<CameraScript>();

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
        
        rb.angularDrag = angular_drag;
    }

    void Update(){

        if( game_over ){    
            rb.velocity = new Vector3(0f, 0f, 0f);
        } 

        if( Input.GetMouseButtonDown(0) && !game_over ){
            dragging = true;
        }

        if( Input.GetMouseButtonUp(0) ){
            dragging = false;
        }

        if( rb.angularVelocity.magnitude < value_t ){
            rb.angularVelocity = Vector3.zero;
        }


    }

    void FixedUpdate(){
        
        if( dragging ){
            rotX = Input.GetAxis("Mouse X") * Mathf.Deg2Rad * 1.5f;
            rb.AddTorque (Vector3.down * -rotX * rotationSpeed * Time.fixedDeltaTime);

            hold_time += 1;
        }

    }
    
     void OnCollisionEnter (Collision col)
    {
        Debug.Log(col.gameObject.name);    
        dragging = false;

        rb.velocity = new Vector3(0f, 0f, 0f);
        rb.Sleep();
        
        game_over = true;
        game_manager.is_level_active = false;
        
        game_manager.game_over();
    }

}

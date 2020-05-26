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

    private void fallDown(float speed){
        rb.velocity = new Vector3(0, speed, 0);
    }

    // private void move_object(Vector3 direction){
    //     rb.MovePosition(transform.position + (direction * speed * Time.deltaTime));
    // }

    // Rotating Object
    private int isVertical = 0; // 0 = top, 2 = down;
    private int isHorizontal = -1; // 0 = forward, 1 = right, 3 = left;
    [SerializeField] private float rotation_duration = 0.8f;
    private float time = 5;
    private float rotation_degree = 90f;
    private bool rotating = false;
    private float dissolveLevel;

    private bool dragging = false;
    private bool dragging2 = true;
    
    
    public float rotationSpeed = 5000f;
    public float angular_drag = 0.85f;
    public float divisor = 12f;
    float rotX, rotY;
    
    public float value_t = 2f;

    public void increment_score(){
        score += 1;
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
        // speed = 25f;
        game_manager = FindObjectOfType<Manager>();
        fall_down_speed = game_manager.fall_down_speed;
        fallDown(fall_down_speed);
    }

    void Update(){

        game_manager.fall_down_speed = rb.velocity.y;

        rb.angularDrag = angular_drag;
        
        
        if( score >= (int) game_manager.object_in_level() ){
            game_manager.start_next_level();
        }    
        

        if( Input.GetMouseButtonDown(0) ){
            dragging = true;
        }
        if( Input.GetMouseButtonUp(0) ){
            dragging = false;
        }
        /*
        if( rb.angularVelocity.magnitude < value_t ){
            rb.angularVelocity = Vector3.zero;
        }*/
        
        if( rb.angularVelocity.magnitude < value_t ){
            rb.angularVelocity = Vector3.zero;
            
            Vector3 movement = new Vector3( game_manager.joystick.Horizontal, game_manager.joystick.Vertical, 0f );

            transform.RotateAround(Vector3.up, movement.x / divisor * Time.deltaTime);
            transform.RotateAround(Vector3.right, movement.y / divisor * Time.deltaTime);
        }

        // if( dragging ){
            //float rotX2 = Input.GetAxis("Mouse X") * Mathf.Deg2Rad / 3f;
            //float rotY2 = Input.GetAxis("Mouse Y") * Mathf.Deg2Rad / 3f;
            
            
        // }

    }

    void FixedUpdate(){
        
        if( dragging ){

            Vector3 movement = new Vector3( game_manager.joystick.Horizontal, game_manager.joystick.Vertical, 0f );

            // // As in PolySphere game, Torque
            rotX = Input.GetAxis("Mouse X") * Mathf.Deg2Rad;
            rotY = Input.GetAxis("Mouse Y") * Mathf.Deg2Rad;
            
            // rotX = movement.x * Mathf.Deg2Rad;
            // rotY = movement.y * Mathf.Deg2Rad;

            if ( Math.Abs(rotX) > Math.Abs(rotY) )
                rb.AddTorque (Vector3.down * -rotX * rotationSpeed * Time.fixedDeltaTime);
            else
                rb.AddTorque (Vector3.right * rotY * rotationSpeed * Time.fixedDeltaTime);

            // rb.AddTorque (Vector3.down * -rotX);
            // rb.AddTorque (Vector3.right * rotY);
        }

    }
    
     void OnCollisionEnter (Collision col)
    {
        Debug.Log(col.gameObject.name);    
        rb.velocity = new Vector3(0f, 0f, 0f);
        gameObject.SetActive(false);
        game_manager.game_over();
    }

}

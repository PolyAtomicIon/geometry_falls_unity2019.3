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

    // private void move_object(Vector3 direction){
    //     rb.MovePosition(transform.position + (direction * speed * Time.deltaTime));
    // }

    private int isVertical = 0; // 0 = top, 2 = down;
    private int isHorizontal = -1; // 0 = forward, 1 = right, 3 = left;
    [SerializeField] private float rotation_duration = 0.8f;
    private float time = 5;
    public float rotation_degree = 90f;
    private bool rotating = false;
    private float dissolveLevel;

    private bool dragging = false;
    private bool dragging2 = true;
    private bool dragX = false;
    private bool dragY = false;
    
    
    public float rotationSpeed = 5000f;
    public float rotationSpeed2 = 2500f;
    public float angular_drag = 0.85f;
    public float divisor = 12f;
    public float multiplier = 75f;
    float rotX, rotY;
    
    public float value_t = 2f;

    public int hold_time = 0;
    public int hold_time_limit = 20;

    public void increment_score(){
        score += 1;
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

        rb.centerOfMass = Vector3.zero;
        rb.inertiaTensorRotation = Quaternion.identity;

        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }

    public void OnObjectSpawn(){
        Start();
        // speed = 25f;
        game_manager = FindObjectOfType<Manager>();
        game_manager.player = this;
        fall_down_speed = game_manager.fall_down_speed;
        fallDown(fall_down_speed);
    }

    void Update(){

        rb.angularDrag = angular_drag;
        
        if( Input.GetMouseButtonDown(0) ){
            dragging = true;
        }
        if( Input.GetMouseButtonUp(0) ){
            dragging = false;
            dragY = false;
            dragX = false;
        }

        if( rb.angularVelocity.magnitude < value_t ){
            rb.angularVelocity = Vector3.zero;
        }

    }

    void FixedUpdate(){
        
        if( dragging ){
            
            // As in PolySphere game, Torque
            rotX = Input.GetAxis("Mouse X") * Mathf.Deg2Rad * 1.25f;
            rotY = Input.GetAxis("Mouse Y") * Mathf.Deg2Rad;
            //d

            if ( Math.Abs(rotX) > Math.Abs(rotY) && !dragY ){
                rb.AddTorque (Vector3.down * -rotX * rotationSpeed2 * Time.fixedDeltaTime);
                dragX = true;
                dragY = false;
            }
            else if( Math.Abs(rotX) < Math.Abs(rotY) && !dragX ){
                rb.AddTorque (Vector3.right * rotY * rotationSpeed2 * Time.fixedDeltaTime);
                dragY = true;
                dragX = false;
            }
            else{
                dragX = false;
                dragY = false;
            }

            hold_time += 1;
        }

    }
    
     void OnCollisionEnter (Collision col)
    {
        Debug.Log(col.gameObject.name);    
        rb.velocity = new Vector3(0f, 0f, 0f);
        gameObject.SetActive(false);
        game_manager.is_level_started = false;
        game_manager.game_over();
    }

}
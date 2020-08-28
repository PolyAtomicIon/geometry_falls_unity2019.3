using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine;
using System;   
public class Player : MonoBehaviour, IPooledObject
{

    public Rigidbody rb;
    private Transform transform;

    private Collider collider;

    // private float angular_drag = 1.5f;
    private float fall_down_speed = -27f;
    private float acceleration = -0.035f;
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
    [SerializeField] private float rotation_duration = 0.35f;
    private float time = 5;
    public float rotation_degree = 160f;
    private bool rotating = false;
    private float dissolveLevel;

    private bool dragging = false;
    private bool dragging2 = true;
    private bool dragX = false;
    private bool dragY = false;
    
    
    public float rotationSpeed = 40000f;
    public float rotationSpeed2 = 40000f;
    public float angular_drag = 0.45f;
    public float divisor = 0.75f;
    public float multiplier = 45f;
    float rotX, rotY;
    
    public float value_t = 8f;

    public int hold_time = 0;
    public int hold_time_limit = 8;

    // Dobule Click 
    bool one_click = false;
    bool timer_running;
    float timer_for_double_click;
    
    //this is how long in seconds to allow for a double click
    float delay = 0.25f;

    // Double Click
    
    CameraScript cameraScript;

    bool game_over = false;

    // new update: Lives

    Vector3 ImpulseForce = new Vector3(0, 0, 0);
    bool isMoveUpward = false;
    bool collided = false;
    float moveUpwardSpeed = 50f;    
    [SerializeField] Vector3 target_position;
    public int lives = 10;
    private Vector3 current_velocity;

    public void set_gravity(float acceleration = 0f){
        Physics.gravity = new Vector3(0, acceleration, 0);    
    }

    public void addImpulse(float force){
        ImpulseForce = new Vector3(0, force, 0);
        rb.AddForce(ImpulseForce, ForceMode.Impulse);
    }

    public void increment_score(){
        score += 1;
        // cameraScript.Animation();
    }

    void moveUpward(){
        // float step =  moveUpwardSpeed * Time.deltaTime; // calculate distance to move

        // transform.position = Vector3.MoveTowards(transform.position, target_position, step);

        // Check if the position of the cube and sphere are approximately equal.
        if (Vector3.Distance(transform.position, target_position) < 5f)
        {
            isMoveUpward = false;
            collided = false;
            resumeGame();
        }
    }

    public void resumeGame(){
        // rb.WakeUp();
        // rb.velocity = current_velocity;
        // current_velocity = new Vector3(0, 0, 0);
        addImpulse(-rb.velocity.y * 2);
        collider.enabled = !collider.enabled;
    }

    public void obstacle_hit(){
        
        collider.enabled = !collider.enabled;
        lives -= 1;
        Debug.Log("Lives " +  lives);

        isMoveUpward = true;
        Debug.Log(current_velocity);
        // rb.velocity = new Vector3(0f, 0f, 0f);
        // gameObject.SetActive(false);
        
        // game over
        if( lives <= 0 ){
            rb.velocity = new Vector3(0f, 0f, 0f);
            dragging = false;
            game_over = true;
            game_manager.is_level_started = false;
            game_manager.game_over();
            rb.Sleep();
        }
        else{
            StartCoroutine(InitialPosition(rotation_duration));
            // set_gravity();
            addImpulse(-current_velocity.y);
            rb.velocity = new Vector3(0, 0 , 0);
            // target_position = transform.position - game_manager.gap_between;
            target_position = transform.position - new Vector3(0, game_manager.gap_between.y / 2, 0);
        }
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

    private IEnumerator InitialPosition( float duration = 1.0f )
    {
        rotating = true ;
        Quaternion startRotation = transform.rotation;
        Transform target = transform;
        target.eulerAngles = new Vector3(0f, 0f, 0);
        Quaternion endRotation = target.rotation;

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

        lives = 10;
        isMoveUpward = false;

        set_gravity(acceleration);

        rb = GetComponent<Rigidbody>();
        render = GetComponent<Renderer>();
        transform = GetComponent<Transform>();
        collider = GetComponent<Collider>();

        cameraScript = FindObjectOfType<CameraScript>();

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
        
        rb.angularDrag = angular_drag;
    }

    void Update(){
        
        // rb.angularDrag = angular_drag;

        // if( Input.GetKeyDown(KeyCode.W) ){
                
        //     ImpulseForce = new Vector3(0, 2*-rb.velocity.y, 0);
        //     rb.AddForce(ImpulseForce, ForceMode.Impulse);
        // }

        // if( Input.GetKeyDown(KeyCode.E) ){
                
        //     ImpulseForce = new Vector3(0, 2*-rb.velocity.y, 0);
        //     rb.AddForce(ImpulseForce, ForceMode.Impulse);
        // }


        if( game_over ){    
            rb.velocity = new Vector3(0f, 0f, 0f);
        } 

        if( isMoveUpward ){
            moveUpward();
            // Debug.Log("RB V " + rb.velocity);
        }

        if( !isMoveUpward && -rb.velocity.y > 20f  ){
            // Debug.Log("CURRENT V" + current_velocity.y);
            current_velocity = new Vector3(0, rb.velocity.y, 0);   
        }

        if( Input.GetMouseButtonDown(0) && !game_over ){
            dragging = true;

            // Double Tap Detection

            // if (!one_click){
            //     timer_for_double_click = Time.time;
            //     one_click = true;
            // }
            // else{
            //     if ((Time.time - timer_for_double_click) > delay){
            //         timer_for_double_click = Time.time;
            //     } 
            //     else{
            //         //Do something here if double clicked
            //         Debug.Log("Double Click!");
            //         // transform.eulerAngles = new Vector3(0f, 0f, 0);
            //         StartCoroutine(InitialPosition(rotation_duration));
    
            //         one_click = false;
            //     }
            // }
       
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
            // rotY = Input.GetAxis("Mouse Y") * Mathf.Deg2Rad;

            // rb.AddTorque (Vector3.down * -rotX * 5000 * Time.fixedDeltaTime);
            // rb.AddTorque (Vector3.right * rotY * 5000 * Time.fixedDeltaTime);

            // if ( Math.Abs(rotX) > Math.Abs(rotY) && !dragY ){
                rb.AddTorque (Vector3.down * -rotX * rotationSpeed2 * Time.fixedDeltaTime);

            //     dragX = true;
            //     dragY = false;
            // }
            // else if( Math.Abs(rotX) < Math.Abs(rotY) && !dragX ){
            //     rb.AddTorque (Vector3.right * rotY * rotationSpeed2 * Time.fixedDeltaTime);
            //     dragY = true;
            //     dragX = false;
            // }
            // else{
            //     dragX = false;
            //     dragY = false;
            // }

            hold_time += 1;
        }

    }
    
     void OnCollisionEnter (Collision col)
    { 

        Debug.Log("Collided");    
        Debug.Log("Cur V" + current_velocity);    
        Debug.Log("Rb V" + rb.velocity);    
        Debug.Log(col.gameObject.name);    
        obstacle_hit(); 
    }

}
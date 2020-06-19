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
    [SerializeField]
    private float speed;
    public Renderer render;

    // private float maxSpeed = 7f;

    // private Vector3 movement;

    private Manager game_manager;

    private int score = 0;

    float ScreenWidth;
    float ScreenHeight;
    float ScreenHeightOffset;

    public float get_position_y_axis(){
        return transform.position.y;
    }

    public float get_velocity_y_axis(){
        return rb.velocity.y;
    }

    private void fallDown(float speed){
        rb.velocity = new Vector3(0, speed, 0);
    }

<<<<<<< HEAD
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
=======
    public bool dragging = false;
>>>>>>> b925818ba18bc21f5f7213b4608aff0a940af0b1
    

    public void increment_score(){
        score += 1;
    }
    
    // void OnMouseDown(){
    //     dragging = true;
    // }
    
    // void OnMouseUp(){
    //     dragging = false;
    // }

    void Start(){
        speed = 6.5f;

        Physics.gravity = new Vector3(0, acceleration, 0);    

        rb = GetComponent<Rigidbody>();
        render = GetComponent<Renderer>();
        transform = GetComponent<Transform>();

        rb.centerOfMass = Vector3.zero;
        rb.inertiaTensorRotation = Quaternion.identity;

        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ ;

        ScreenHeight = Screen.height;
        ScreenWidth = Screen.width;
        ScreenHeightOffset = ScreenHeight / 100f;
        Debug.Log(ScreenWidth);        
        Debug.Log(ScreenHeight);
    }

    public void OnObjectSpawn(){
        Start();
        game_manager = FindObjectOfType<Manager>();
        game_manager.player = this;
        fall_down_speed = game_manager.fall_down_speed;
        fallDown(fall_down_speed);
    }

    
    void Update(){

        if( Input.GetMouseButtonDown(0) ){
            dragging = true;
        }
        if( Input.GetMouseButtonUp(0) ){
            dragging = false;
            dragY = false;
            dragX = false;
        }

        if( dragging ){
            Vector3 mouseScreenPosition = new Vector3( Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z );

            if( Mathf.Abs(ScreenWidth / 2 - Mathf.Abs(mouseScreenPosition.x)) <= 150f && Mathf.Abs(ScreenHeight / 2 - mouseScreenPosition.y) <= 150f){
                mouseScreenPosition.x = ScreenWidth / 2;
                mouseScreenPosition.y = ScreenHeight / 2 - Mathf.Abs(ScreenHeight / 2 - mouseScreenPosition.y);
            }

            Debug.Log( mouseScreenPosition );            

            Ray mouseRay = Camera.main.ScreenPointToRay( mouseScreenPosition );

            float midPoint = (transform.position - Camera.main.transform.position).magnitude * 0.96f;
            Vector3 finalState = mouseRay.origin + mouseRay.direction * midPoint;

            // transform.LookAt(finalState);
            
            Quaternion rotation = Quaternion.LookRotation(finalState - transform.position);
            transform.rotation = Quaternion.Slerp (transform.rotation, rotation, speed * Time.deltaTime);
        }

    }

    // void Update(){

    //     if( Input.GetMouseButtonDown(0) ){
    //         dragging = true;
    //     }
    //     if( Input.GetMouseButtonUp(0) ){
    //         dragging = false;
    //     }


        // if ( dragging ){
            
<<<<<<< HEAD
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
=======
        //     Vector3 mouseScreenPosition = new Vector3( Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z );
            
            // Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint( mouseScreenPosition );   

        //     // mouseWorldPosition.z = 0f;

        //     Debug.Log(mouseWorldPosition);

        //     // if( Mathf.Abs(0f - Mathf.Abs(mouseWorldPosition.x)) <= 0.5f && Mathf.Abs(6.2f + mouseWorldPosition.z) <= 0.5f){
        //     //     mouseWorldPosition.x = 0f;
        //     // }

        //     // Debug.Log(mouseWorldPosition);
            
        //     Vector3 relativePos = mouseWorldPosition - transform.position;
        //     if( relativePos.magnitude > 0f ){
        //         Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        //         transform.rotation = Quaternion.Slerp (transform.rotation, rotation, 4 * Time.deltaTime);
        //     }

        // }

    // }
>>>>>>> b925818ba18bc21f5f7213b4608aff0a940af0b1

    
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
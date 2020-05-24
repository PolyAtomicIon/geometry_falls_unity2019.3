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

    private int done = 0;
    private int start = 0;

    bool dragging;
    public float rotationSpeed = 8100f;
    float rotX, rotY;
    
    // Rotation, like smooth
    private IEnumerator Rotate( Vector3 angles, float duration = 1.0f )
    {
        rotating = true ;
        Quaternion startRotation = transform.rotation ;
        Quaternion endRotation = Quaternion.Euler( angles ) * startRotation ;
        
        for( float t = 0 ; t < Time.deltaTime * 2; t+= Time.deltaTime )
        {
            transform.rotation = Quaternion.Lerp( startRotation, endRotation, t / duration ) ;
            yield return null;
        }
        
        transform.rotation = endRotation;
        rotating = false;
    }
    
    private void Rotate(Vector3 to){
        transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, to, Time.deltaTime);
    }

    public void rotate(int direction){

        if( rotating ) return;

        if( direction == 0 ){
            StartCoroutine( Rotate( Vector3.right * rotation_degree, rotation_duration ) );
            // Rotate(new Vector3(rotation_degree, 0, 0));
            
            // transform.Rotate(90.0f, 0.0f, 0.0f, Space.World);
        }
        if( direction == 2 ){
            StartCoroutine( Rotate( -Vector3.right * rotation_degree, rotation_duration ) );
            // Rotate(new Vector3(-rotation_degree, 0, 0));

            // transform.Rotate(-90.0f, 0.0f, 0.0f, Space.World);
        }
        if( direction == 3 ){
            StartCoroutine( Rotate( Vector3.up * rotation_degree, rotation_duration ) );
            // Rotate(new Vector3(0, rotation_degree, 0));
            
            // transform.Rotate(0.0f, 90.0f, 0.0f, Space.World);
        }
        if( direction == 1 ){
            StartCoroutine( Rotate( -Vector3.up * rotation_degree, rotation_duration ) );
            // Rotate(new Vector3(0, -rotation_degree, 0));
            
            // transform.Rotate(0.0f, -90.0f, 0.0f, Space.World);
        }
    }

    
    // Silly rotation
    // public void rotate(int direction)
    // {

    //     if( rotating ) return;

    //     // UP
    //     if( direction == 0 ){
    //         // From horizontal position to vertical
    //         if( isHorizontal == 1 ){    
    //             StartCoroutine( Rotate( Vector3.forward * rotation_degree, rotation_duration ) );
    //             isHorizontal = -1;
    //             isVertical = 0;
    //         }
    //         else if( isHorizontal == 3 ){    
    //             StartCoroutine( Rotate( -Vector3.forward * rotation_degree, rotation_duration ) );
    //             isHorizontal = -1;
    //             isVertical = 0;
    //         }
    //         // Turn vertically
    //         else{
    //             StartCoroutine( Rotate( Vector3.right * rotation_degree, rotation_duration ) );
    //             if( isVertical == 0 ){
    //                 isVertical = 1;
    //                 isHorizontal = 0;
    //             }
    //             else if( isVertical == 1 ){
    //                 isVertical = 2;
    //                 isHorizontal = -1;
    //             }
    //             else if( isVertical == 2 ){
    //                 isVertical = 3;
    //                 isHorizontal = 2;  
    //             }
    //             else{
    //                 isVertical = 0;
    //                 isHorizontal = -1;
    //             }
    //         }
    //     }
    //     // Left
    //     if( direction == 1 ){
    //         // Turn horizontally
    //         if(  isVertical != 0 && isVertical != 2 ){
    //             StartCoroutine( Rotate( -Vector3.up * rotation_degree, rotation_duration ) );
    //             isHorizontal -= 1;
    //             if( isHorizontal < 0 )
    //                 isHorizontal = 3;
    //         }
    //         // Turn from top or down position, to left
    //         else{
    //             // from Top
    //             if( isVertical == 0 ){
    //                 StartCoroutine( Rotate( Vector3.forward * rotation_degree, rotation_duration ) );
    //                 isHorizontal = 3;
    //             }
    //             // from Bottom
    //             else{
    //                 StartCoroutine( Rotate( -Vector3.forward * rotation_degree, rotation_duration ) );
    //                 isHorizontal = 3;
    //             }
    //         }
            
    //         if( isHorizontal == 0 )
    //             isVertical = 1;
    //         else if( isHorizontal == 2 )
    //             isVertical = 3;
    //         else
    //             isVertical = -1;
    //     }
    //     // Down
    //     if( direction == 2 ){
    //         // From horizontal position to vertical
    //         if( isHorizontal == 1 ){    
    //             StartCoroutine( Rotate( -Vector3.forward * rotation_degree, rotation_duration ) );
    //             isHorizontal = -1;
    //             isVertical = 2;
    //         }
    //         else if( isHorizontal == 3 ){    
    //             StartCoroutine( Rotate( Vector3.forward * rotation_degree, rotation_duration ) );
    //             isHorizontal = -1;
    //             isVertical = 2;
    //         }
    //         // Turn Vertically
    //         else{
    //             StartCoroutine( Rotate( -Vector3.right * rotation_degree, rotation_duration ) );
    //             if( isVertical == 0 ){
    //                 isVertical = 3;
    //                 isHorizontal = 2;
    //             }
    //             else if( isVertical == 1 ){
    //                 isVertical = 0;
    //                 isHorizontal = -1;
    //             }
    //             else if( isVertical == 2 ){
    //                 isVertical = 1;
    //                 isHorizontal = 0;  
    //             }
    //             else{
    //                 isVertical = 2;
    //                 isHorizontal = -1;
    //             }

    //         }
    //     }
    //     // Right
    //     if( direction == 3 ){
    //         // Turn horizontally
    //         if( isVertical != 0 && isVertical != 2 ){
    //             StartCoroutine( Rotate( Vector3.up * rotation_degree, rotation_duration ) );    
    //             isHorizontal += 1;
    //             isHorizontal = isHorizontal % 4;
    //         }
    //         // Turn from top or down position, to left
    //         else{    
    //             // from Top
    //             if( isVertical == 0 ){
    //                 StartCoroutine( Rotate( -Vector3.forward * rotation_degree, rotation_duration ) );
    //                 isHorizontal = 1;
    //             }
    //             // from Bottom
    //             else{
    //                 StartCoroutine( Rotate( Vector3.forward * rotation_degree, rotation_duration ) );
    //                 isHorizontal = 1;
    //             }
    //         }

    //         if( isHorizontal == 0 )
    //             isVertical = 1;
    //         else if( isHorizontal == 2 )
    //             isVertical = 3;
    //         else
    //             isVertical = -1;
    //     }
    
        
    
    // }
    

    // Just experiment

    //  public void rotate1(int direction){

    //     if( rotating ) return;

    //     // UP
    //     if( direction == 0 ){
    //         StartCoroutine( Rotate( transform.TransformDirection(Vector3.right) * rotation_degree, rotation_duration ) );
    //         //transform.Rotate(90.0f * Time.deltaTime, 0.0f, 0.0f, Space.Self);
    //     }
    //     // Left
    //     if( direction == 1 ){
    //         StartCoroutine( Rotate( -transform.TransformDirection(Vector3.up) * rotation_degree, rotation_duration ) );
    //     }
    //     // Down
    //     if( direction == 2 ){
    //         StartCoroutine( Rotate( -transform.TransformDirection(Vector3.right) * rotation_degree, rotation_duration ) );
    //     }
    //     // Right
    //     if( direction == 3 ){
    //         StartCoroutine( Rotate( transform.TransformDirection(Vector3.up) * rotation_degree, rotation_duration ) );    
    //     }
    
    // }

    public void increment_score(){
        score += 1;
    }

    void Start(){
        Physics.gravity = new Vector3(0, acceleration, 0);    

        rb = GetComponent<Rigidbody>();
        render = GetComponent<Renderer>();
        transform = GetComponent<Transform>();
        // rb.angularDrag = angular_drag;

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
        
        // Debug.Log(rb.velocity.y);
        // To move object in x,y axis
        // movement = new Vector3( Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical") );
        // movement = new Vector3( joystick.Horizontal, 0f, joystick.Vertical );

        game_manager.fall_down_speed = rb.velocity.y;

        /*
        if( Input.GetKeyDown("s") )
            rotate(2);
        
        if( Input.GetKeyDown("w") )
            rotate(0);
        
        if( Input.GetKeyDown("d") )
            rotate(3);

        if( Input.GetKeyDown("a") )
            rotate(1);

        if( score >= (int) game_manager.object_in_level() ){
            game_manager.start_next_level();
        }    
        */

        if( Input.GetMouseButtonDown(0) ){
            dragging = true;
        }
        if( Input.GetMouseButtonUp(0) ){
            dragging = false;
        }
        
        if( rb.angularVelocity.magnitude < 5.3f ){
            rb.angularVelocity = Vector3.zero;
            /*
            if( dragging ){
                float rotX2 = Input.GetAxis("Mouse X") * Mathf.Deg2Rad;
                float rotY2 = Input.GetAxis("Mouse Y") * Mathf.Deg2Rad;

                Debug.Log(rotX2);

                transform.RotateAround(Vector3.up, rotX2 * 150f * Time.deltaTime);
                transform.RotateAround(Vector3.right, rotY2 * 150f * Time.deltaTime);
            }*/
        }

    }

    void FixedUpdate(){
        
        if( dragging ){
            // As in PolySphere game, Torque
            rotX = Input.GetAxis("Mouse X") * Mathf.Deg2Rad;
            rotY = Input.GetAxis("Mouse Y") * Mathf.Deg2Rad;

            rb.AddTorque (Vector3.down * -rotX * rotationSpeed * Time.fixedDeltaTime);
            rb.AddTorque (Vector3.right * rotY * rotationSpeed * Time.fixedDeltaTime);

            //rb.AddTorque (Vector3.down * -rotX);
            //rb.AddTorque (Vector3.right * rotY);
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

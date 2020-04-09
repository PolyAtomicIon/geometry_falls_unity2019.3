using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IPooledObject
{

    private Rigidbody rb;
    private Transform transform;
    public float angular_drag = 1.5f;
    public float fall_down_speed;
    public float speed;

    private float maxSpeed = 7f;

    private Vector3 movement;

    private Manager game_manager;

    public float get_position_y_axis(){
        return transform.position.y;
    }

    private void fallDown(float speed){
        rb.velocity = new Vector3(0, speed, 0);
    }

    private void move_object(Vector3 direction){
        rb.MovePosition(transform.position + (direction * speed * Time.deltaTime));
    }

    void Start(){
        rb = GetComponent<Rigidbody>();
        transform = GetComponent<Transform>();
        rb.angularDrag = angular_drag;
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        game_manager = FindObjectOfType<Manager>();
    }

    public void OnObjectSpawn(){
        Start();
        speed = 25f;
        fallDown(fall_down_speed);
    }

    void Update(){
        
        // Debug.Log(rb.velocity.y);
        // To move object in x,y axis
        // movement = new Vector3( Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical") );
        // movement = new Vector3( joystick.Horizontal, 0f, joystick.Vertical );

    }

    void FixedUpdate()
    { 

        //move_object(movement);  

    }

     void OnCollisionEnter (Collision col)
    {
        Debug.Log(col.gameObject.name);    
        rb.velocity = new Vector3(0f, 0f, 0f);
        game_manager.game_over();
    }

}

using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IPooledObject
{

    private Rigidbody rb;
    private Transform transform;

    public float fall_down_speed;
    public float speed;

    public float maxSpeed = 7f;

    private Vector3 movement;
    
    //public Joystick joystick;

    private Manager game_manager;

    private void fallDown(float speed){
        rb.velocity = new Vector3(0, speed, 0);
    }

    private void move_object(Vector3 direction){
        rb.MovePosition(transform.position + (direction * speed * Time.deltaTime));
    }

    void Start(){
        rb = GetComponent<Rigidbody>();
        transform = GetComponent<Transform>();
        
        game_manager = FindObjectOfType<Manager>();
    }

    void OnObjectSpawn(){
        fall_down_speed = -4f;
        speed = 25f;
        fallDown(fall_down_speed);
    }

    void Update(){
        
        Debug.Log(rb.velocity.y);
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

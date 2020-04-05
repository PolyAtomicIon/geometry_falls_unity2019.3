using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatableObject : MonoBehaviour
{

    private float rotSpeed = 90f;
    
    //public Joystick joystick;

    public Transform transform_player;

    bool dragging;

    Rigidbody rb;

    /*
    void OnMouseDrag(){
        dragging = true;
    }
    */
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //joystick = FindObjectOfType<Joystick>();
        
        rb.centerOfMass = Vector3.zero;
        rb.inertiaTensorRotation = Quaternion.identity;
    }

    void Update()
    {

        if( Input.GetMouseButtonDown(0) ){
            dragging = true;
        }
        if( Input.GetMouseButtonUp(0) ){
            dragging = false;
        }

        /*
        if( dragging ){
             
            // Moving by joysticks
            //float rotX = joystick.Horizontal * rotSpeed * Mathf.Deg2Rad * Time.deltaTime;
            //float rotY = joystick.Vertical * rotSpeed * Mathf.Deg2Rad * Time.deltaTime;
        
            // Moving without joysticks
            //float rotX = Input.GetAxis("Mouse X") * rotSpeed * 1.7f * Time.deltaTime * Mathf.Deg2Rad;
            //float rotY = Input.GetAxis("Mouse Y") * rotSpeed * 1.7f *  Time.deltaTime * Mathf.Deg2Rad;
            
            // Without torque
            //transform.RotateAround(Vector3.up, rotX);
            //transform.RotateAround(Vector3.right, rotY);

        }
        */
        
    }


    void FixedUpdate(){
        
        if( dragging ){
            // As in PolySphere game, Torque
            float rotX = Input.GetAxis("Mouse X") * rotSpeed * 90f * Mathf.Deg2Rad * Time.fixedDeltaTime;
            float rotY = Input.GetAxis("Mouse Y") * rotSpeed * 90f * Mathf.Deg2Rad * Time.fixedDeltaTime;

            rb.AddTorque (Vector3.down * -rotX);
            rb.AddTorque (Vector3.right * rotY);
        }

    }


}

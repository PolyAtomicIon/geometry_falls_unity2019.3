using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectRotator : MonoBehaviour
{
    public float Speed;
    float AngleUpX, AngleDownX;

    [SerializeField]
    bool dragging = false;

    void Update(){
        
        if( Input.GetMouseButtonDown(0) ){
            dragging = true;
        }
        if( Input.GetMouseButtonUp(0) ){
            dragging = false;
        }
    
        if( dragging ){
            RotateDownX();
            // RotateUpX();
        }

    }

    void RotateUpX(){
        AngleUpX += Input.GetAxis("Mouse X") * Speed * Time.deltaTime;
        
        // Upward Sections
        AngleUpX = Mathf.Clamp(AngleUpX, -90, 90);
        
        transform.localRotation = Quaternion.AngleAxis(AngleUpX, Vector3.up);
    }

    void RotateDownX(){
        AngleDownX += Input.GetAxis("Mouse X") * Speed * - Time.deltaTime;
        
        // Downward Sections
        AngleDownX = Mathf.Clamp(AngleDownX, 90, 270);
        
        transform.localRotation = Quaternion.AngleAxis(AngleDownX, Vector3.up);
    }

}

using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine;
using System;   
public class RotateOnDrag : MonoBehaviour
{

    // [SerializeField]
    // private float speed;

    // float ScreenWidth;
    // float ScreenHeight;
    // float ScreenHeightOffset;

    // bool dragging = false;

    // void OnMouseDown(){
    //     dragging = true;
    // }
    
    // void OnMouseUp(){
    //     dragging = false;
    // }

    // void Start(){

    //     speed = 6.5f;

    //     ScreenHeight = Screen.height;
    //     ScreenWidth = Screen.width;
    //     ScreenHeightOffset = ScreenHeight / 100f;
        
    //     Debug.Log(ScreenWidth);        
    //     Debug.Log(ScreenHeight);
    
    // }
    
    // void Update(){

    //     if( dragging ){
    //         Vector3 mouseScreenPosition = new Vector3( Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z );

    //         if( Mathf.Abs(ScreenWidth / 2 - Mathf.Abs(mouseScreenPosition.x)) <= 150f && Mathf.Abs(ScreenHeight / 2 - mouseScreenPosition.y) <= 150f){
    //             mouseScreenPosition.x = ScreenWidth / 2;
    //             mouseScreenPosition.y = ScreenHeight / 2 - Mathf.Abs(ScreenHeight / 2 - mouseScreenPosition.y);
    //         }

    //         Debug.Log( mouseScreenPosition );            

    //         Ray mouseRay = Camera.main.ScreenPointToRay( mouseScreenPosition );

    //         float midPoint = (transform.position - Camera.main.transform.position).magnitude * 0.96f;
    //         Vector3 finalState = mouseRay.origin + mouseRay.direction * midPoint;

    //         // transform.LookAt(finalState);
            
    //         Quaternion rotation = Quaternion.LookRotation(finalState - transform.position);
    //         transform.rotation = Quaternion.Slerp (transform.rotation, rotation, speed * Time.deltaTime);
    //     }

    // }

}
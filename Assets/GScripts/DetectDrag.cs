using UnityEngine;
 using System.Collections;
 
 public class DetectDrag : MonoBehaviour 
 {

    public RotateOnDragPlayer player;

    void OnMouseDown(){
        player.dragging = true;
    }
    
    void OnMouseUp(){
        player.dragging = false;
    }
 }
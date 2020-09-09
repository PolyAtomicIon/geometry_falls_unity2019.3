using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine;
using System;   
using UnityEngine.EventSystems;
public class ControlButtonRight : MonoBehaviour,  IPointerUpHandler, IPointerDownHandler, IPointerExitHandler
{

    float pointerDownTime;
    float pointerUpTime;

    public bool isLeft = false;
    
    private bool turned = false;
    public bool isPressed = false;

    Player player;

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDownTime = Time.time;
        isPressed = true;
        turned = false;
    }

    //Do this when the mouse click on this selectable UI object is released.
    public void OnPointerUp(PointerEventData eventData)
    {
        if( isPressed ){
            isPressed = false;
            player.Turn(0);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // isPressed = false;
        // player.Turn(0);
        return;
    }

    void Update(){

		if( player == null )
			player = FindObjectOfType<Player>();

        if( isPressed && !turned ){
            float holdTime = Time.time - pointerDownTime;
            if( holdTime > 0.2f ){   
                if( isLeft )
                    player.Turn(-1);
                else     
                    player.Turn(1);
                turned = true;
            }
        }

    }


}

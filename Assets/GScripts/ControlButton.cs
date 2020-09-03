using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine;
using System;   
using UnityEngine.EventSystems;
public class ControlButton : MonoBehaviour,  IPointerUpHandler, IPointerDownHandler
{

    float pointerDownTime;
    float pointerUpTime;

    public bool isLeft = true;
    public bool isPressed = false;

    Player player;

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDownTime = Time.time;
        isPressed = true;
    }

    //Do this when the mouse click on this selectable UI object is released.
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        player.Turn(0);
    }

    void Update(){

		if( player == null )
			player = FindObjectOfType<Player>();

        if( isPressed ){
            float holdTime = Time.time - pointerDownTime;
            if( holdTime > 0.12f ){        
                if( isLeft )
                    player.Turn(-1);
                else    
                    player.Turn(1);
            }
        }

    }


}

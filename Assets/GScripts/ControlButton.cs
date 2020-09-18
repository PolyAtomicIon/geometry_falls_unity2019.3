using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine;
using System;   
using UnityEngine.EventSystems;
public class ControlButton : MonoBehaviour,  IPointerUpHandler, IPointerDownHandler
{
    float pointerDownTime;

    public bool isLeft = true;
    
    private bool turned = false;
    public bool isPressed = false;

    Player player;

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDownTime = Time.time;
        isPressed = true;
        turned = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if( isPressed ){
            isPressed = false;
            player.Turn(0);
        }
    }

    void Update(){

		if( player == null )
			player = FindObjectOfType<Player>();

        if( isPressed && !turned ){
            float holdTime = Time.time - pointerDownTime;
            if( holdTime > 0.05f ){        
                if( isLeft )
                    player.Turn(-1);
                else     
                    player.Turn(1);
                turned = true;
            }
        }

    }


}
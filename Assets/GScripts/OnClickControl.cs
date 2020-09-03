using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine;
using System;   
using UnityEngine.EventSystems;
public class OnClickControl : MonoBehaviour
{
    Player player;

    public void RotateObject(){
        player.OnClickControl();
    }

    void Update(){

		if( player == null )
			player = FindObjectOfType<Player>();

    }


}

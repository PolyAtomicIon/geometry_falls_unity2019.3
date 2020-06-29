using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine;
using Random=System.Random;
using Random2=UnityEngine.Random;
using TMPro;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{   

    public GameObject swipeRight;
    public GameObject swipeLeft;
    public GameObject dragVerticalUp;
    public GameObject dragVerticalDown;
    public GameObject dragHorizontalRight;
    public GameObject dragHorizontalLeft;

    private bool is_active_scene = true;
    float ScreenWidth = Screen.width;

    public Material TunnelMaterial;
    public Color tunnel_color;  
    public int palette = 0;

    public float fall_down_speed;

    public PlayerTutorial player;
    
    public GameObject Finish;

    private void Awake(){
    }

    public void restartTutorial(){
        Time.timeScale = 1f;
        SceneManager.LoadScene("Tutorial");
    }
        
    public void finish_tutorial(){
        Finish.SetActive(true);
    }

    public void go_to_menu(){
        SceneManager.LoadScene("MainMenu");
    }

    public void start_game(){
        SceneManager.LoadScene("Base");
    }

    void Start(){

    }

    void Update()
    {

        float p_pos_y = player.get_position_y_axis();

        if( p_pos_y < -900f ){
            Finish.SetActive(true);
        }

        if( p_pos_y < -970f ){
            player.DisableObject();
        }

        if( p_pos_y > -135f ){
            dragVerticalDown.SetActive(true);
        }

        else if( p_pos_y < -135f && p_pos_y > -225f ){
            dragVerticalDown.SetActive(false);
            dragVerticalUp.SetActive(true);
        }
        
        else if( p_pos_y < -225f && p_pos_y > -415f ){
            dragVerticalUp.SetActive(false);
            dragHorizontalRight.SetActive(true);
        }

        else if( p_pos_y < -415f && p_pos_y > -505f ){
            dragHorizontalRight.SetActive(false);
            dragVerticalUp.SetActive(true);
        }

        else if( p_pos_y < -505f && p_pos_y > -595f ){
            dragVerticalUp.SetActive(false);
            dragHorizontalLeft.SetActive(true);
        }

        else if( p_pos_y < -595f && p_pos_y > -685f ){
            dragHorizontalLeft.SetActive(false);
            swipeRight.SetActive(true);
        }
        
        else if( p_pos_y < -685f && p_pos_y > -775f ){
            swipeRight.SetActive(false);
            swipeLeft.SetActive(true);
        }

        else if( p_pos_y < -775f && p_pos_y > -865f ){
            swipeLeft.SetActive(false);
            dragHorizontalRight.SetActive(true);
        }
        
        else{
            dragHorizontalRight.SetActive(false);
        }

    }

}

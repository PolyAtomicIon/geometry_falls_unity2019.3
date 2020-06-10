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

    public GameObject swipe;
    public GameObject dragVertical;
    public GameObject dragHorizontal;

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

        if( p_pos_y < -700f ){
            Finish.SetActive(true);
        }

        if( p_pos_y < -775f ){
            finish_tutorial();
        }

        if( p_pos_y > -135f ){
            dragVertical.SetActive(true);
            dragHorizontal.SetActive(false);
            swipe.SetActive(false);
        }

        else if( p_pos_y < -135f && p_pos_y > -240f ){
            dragVertical.SetActive(false);
            dragHorizontal.SetActive(true);
            swipe.SetActive(false);
        }
        
        else if( p_pos_y < -240f && p_pos_y > -400f ){
            dragVertical.SetActive(false);
            dragHorizontal.SetActive(false);
            swipe.SetActive(true);
        }

        else{
            dragVertical.SetActive(false);
            dragHorizontal.SetActive(false);
            swipe.SetActive(false);
        }

    }

}

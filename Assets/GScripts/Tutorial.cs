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
    public GameObject horizantal_drag;
    public GameObject vertical_drag;
    


    private bool is_active_scene = true;
    float ScreenWidth = Screen.width;

    public Material TunnelMaterial;
    public Color tunnel_color;  
    public int palette = 3;

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

    public List<Material> materials = new List<Material>(); 
    public List<Color> colors = new List<Color>();
    
    public int cur_hint = 1;

    public Animator SplashScreenAnimator;
    public GameObject transition;

    private void set_materials_color(){
        // for obstacles
        materials[0].SetColor("_BaseColor", colors[0]); 

        // for Player, main objects material
        float intensity = 0.01f;
        
        materials[1].SetColor("_BaseColor", colors[1]);
    }

    public void DisableHints(){
        // foreach(GameObject hint in ui_hints)
        //     hint.SetActive(false);
        
        vertical_drag.SetActive(false);
        swipe.SetActive(false);
        horizantal_drag.SetActive(false);
    }

    void Start(){
        transition.SetActive(false);
        set_materials_color();
    }

    public void start_game(){
       StartCoroutine( LoadYourAsyncScene("Base") );
    }

    public void go_to_menu(){
       StartCoroutine( LoadYourAsyncScene("MainMenu") );
    }

    IEnumerator LoadYourAsyncScene(string SceneName)
    {
        transition.SetActive(true);
        SplashScreenAnimator.Play("Login to Loading");

        if( SceneName == "Base" )
            PlayerPrefs.SetInt("tutorial", 1);
        PlayerPrefs.SetInt("bgAudioID", 0);
        PlayerPrefs.SetInt("isAudio", 1);
        PlayerPrefs.SetInt("levels", 1000);
        PlayerPrefs.SetInt("id", -1);
        
        // SceneManager.LoadScene("Base"); 
        // yield return null;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    void Update()
    {

        float p_pos_y = player.get_position_y_axis();

        if( p_pos_y < -415f ){
            finish_tutorial();
        }

        // if( p_pos_y < -970f ){
        //     player.DisableObject();
        // }

        if( p_pos_y < 0f && p_pos_y > -135f ){
            if( cur_hint == 1 ){
                vertical_drag.SetActive(true);   
                // Time.timeScale = 0;
                player.DisableObject();
            }
        }

        else if( p_pos_y < -145f && p_pos_y > -225f ){
            if( cur_hint == 2 ){
                vertical_drag.SetActive(false);
                horizantal_drag.SetActive(true);
                player.DisableObject();
            }
        }
        
        else{
            horizantal_drag.SetActive(false);
        }

    }

}

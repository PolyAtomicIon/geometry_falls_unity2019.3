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

    List<GameObject> ui_hints = new List<GameObject>();

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

    public void go_to_menu(){
        SceneManager.LoadScene("MainMenu");
    }

    public void start_game(){
        PlayerPrefs.SetInt("id", -1);
        SceneManager.LoadScene("Base");
    }

    IEnumerator continue_game(){
        PlayerPrefs.SetInt("tutorial", 1);
        PlayerPrefs.SetInt("bgAudioID", 0);
        PlayerPrefs.SetInt("isAudio", 1);
        PlayerPrefs.SetInt("levels", 1000);
        // Finish.SetActive(true);
        start_game();
        yield return true;
    }

    public List<Material> materials = new List<Material>(); 
    public List<Color> colors = new List<Color>();
    
    public int cur_hint = 1;

    private void set_materials_color(){
        // for obstacles
        materials[0].SetColor("_BaseColor", colors[0]); 

        // for Player, main objects material
        float intensity = 0.01f;
        
        materials[1].SetColor("_BaseColor", colors[1]);
    }

    public void DisableHints(){
        foreach(GameObject hint in ui_hints)
            hint.SetActive(false);
    }

    void Start(){
        set_materials_color();
        ui_hints.Add(swipeLeft);
        ui_hints.Add(swipeRight);
        ui_hints.Add(dragHorizontalLeft);
        ui_hints.Add(dragHorizontalRight);
        ui_hints.Add(dragVerticalDown);
        ui_hints.Add(dragVerticalUp);
    }

    void Update()
    {

        float p_pos_y = player.get_position_y_axis();

        if( p_pos_y < -865f ){
            StartCoroutine( continue_game() );
        }

        // if( p_pos_y < -970f ){
        //     player.DisableObject();
        // }

        if( p_pos_y < 0f && p_pos_y > -135f ){
            if( cur_hint == 1 ){
                dragVerticalDown.SetActive(true);   
                // Time.timeScale = 0;
                player.DisableObject();
            }
        }

        else if( p_pos_y < -145f && p_pos_y > -225f ){
            if( cur_hint == 2 ){
                // dragVerticalDown.SetActive(false);
                dragVerticalUp.SetActive(true);
                player.DisableObject();
            }
        }
        
        else if( p_pos_y < -235f && p_pos_y > -315f ){
            if( cur_hint == 3 ){
                // dragVerticalUp.SetActive(false);
                dragHorizontalRight.SetActive(true);
                player.DisableObject();
            }
        }

        else if( p_pos_y < -325f && p_pos_y > -415f ){
            if( cur_hint == 4 ){
                // dragVerticalUp.SetActive(false);
                dragHorizontalRight.SetActive(true);
                player.DisableObject();
            }
        }

        else if( p_pos_y < -425f && p_pos_y > -505f ){
            if( cur_hint == 5 ){
                // dragHorizontalRight.SetActive(false);
                dragVerticalUp.SetActive(true);
                player.DisableObject();
            }
        }

        else if( p_pos_y < -515f && p_pos_y > -595f ){
            if( cur_hint == 6 ){
                // dragVerticalUp.SetActive(false);
                dragHorizontalLeft.SetActive(true);
                player.DisableObject();
            }
        }

        else if( p_pos_y < -605f && p_pos_y > -685f ){
            if( cur_hint == 7 ){
                // dragHorizontalLeft.SetActive(false);
                swipeRight.SetActive(true);
                player.DisableObject();
            }
        }
        
        else if( p_pos_y < -695f && p_pos_y > -775f ){
            if( cur_hint == 8 ){
                // swipeRight.SetActive(false);
                swipeLeft.SetActive(true);
                player.DisableObject();
            }
        }

        else if( p_pos_y < -785f && p_pos_y > -865f ){
            if( cur_hint == 9 ){
                // swipeLeft.SetActive(false);
                dragHorizontalLeft.SetActive(true);
                player.DisableObject();
            }
        }
        
        else{
            dragHorizontalRight.SetActive(false);
        }

    }

}

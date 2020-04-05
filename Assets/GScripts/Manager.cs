using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{

    float ScreenWidth = Screen.width;
    bool game_over_bool = false;
    int touches = 0;

    ObjectPooler objectPooler;

    public void game_over(){
        Debug.Log("game over!");
        game_over_bool = true;
        Time.timeScale = 0f;
    }
    

    void Start()
    {

        Physics.gravity = new Vector3(0, -0.75f, 0);    
        
        objectPooler = ObjectPooler.Instance;

        int random_player_model_index = 0;

        // here i will spawn object
        //objectPooler.SpawnFromPool("x", spawnPosition, true);

    }

    void Update(){


        // All this stuff for restart of the level

        int i = 0;

        if( Input.GetKeyDown("r") ){
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        while (i < Input.touchCount) {
            
            Debug.Log("touch");
            if (Input.GetTouch (i).position.x > ScreenWidth / 2 || Input.GetTouch (i).position.x < ScreenWidth / 2){
                if( game_over_bool && (touches >= 25) ){
                    Debug.Log("touch");
                    Time.timeScale = 1f;
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                else{
                    touches += 1;
                }
            }
            
            i++;
        }

        // End of restart

    }

}

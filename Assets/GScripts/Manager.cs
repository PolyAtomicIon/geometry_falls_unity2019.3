using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine;
using Random=System.Random;

public class Manager : MonoBehaviour
{   
    enum Constants
    {
        max_touches = 25,
        object_in_level = 5,
        obstacles_on_scene = 20
    }
    
    float ScreenWidth = Screen.width;
    bool game_over_bool = false;
    int touches = 0;

    ObjectPooler objectPooler;

    private int level = 1;
    private int score = 1;

    public int get_score(){
        return score;
    }

    public void increment_score(){
        score += 1;
        level = score / (int) Constants.object_in_level;

        Debug.Log("Score: ");
        Debug.Log(score);
        Debug.Log("level: ");
        Debug.Log(level);
    }

    public void game_over(){
        Debug.Log("game over!");
        game_over_bool = true;
        Time.timeScale = 0f;
    }
    

    void Start()
    {

        Physics.gravity = new Vector3(0, -0.75f, 0);    
        
        objectPooler = ObjectPooler.Instance;

        // here We will spawn random object and its obstacles

        int n = objectPooler.models_tag.Count;
        
        Random rand = new Random();
        string random_model_tag = objectPooler.models_tag[ rand.Next(0, n) ];
            
        objectPooler.SpawnFromPool(random_model_tag, (int) Constants.obstacles_on_scene);

        // object has been spawned with it obstacles, done
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
                if( game_over_bool && (touches >= (int) Constants.max_touches) ){
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

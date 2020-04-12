using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine;
using Random=System.Random;
using TMPro;

public class Manager : MonoBehaviour
{   
    private enum Constants
    {
        max_touches = 25,
        object_in_level = 5,
        obstacles_on_scene = 15
    }
    
    
    public int object_in_level(){
        return (int) Constants.object_in_level;
    }

    private bool is_active_scene = true;

    float ScreenWidth = Screen.width;
    bool game_over_bool = false;
    int touches = 0;

    ObjectPooler objectPooler;
    public TextMeshProUGUI LevelLabel;
    public GameOver gameOverSection;

    private int level = 1;
    private int score = 0;

    public int get_score()
    {
        return score;
    }

    public void increment_score()
    {
        score += 1;

        if( score / ((int) Constants.object_in_level) + 1 > level ){
            level+=1;
            Debug.Log("RELOAD THE SCENE");
            UnloadAdditiveScene();
            LoadAdditiveScene();
        }

        Debug.Log("Score: ");
        Debug.Log(score);
        Debug.Log("level: ");
        Debug.Log(level);
    }

    public void game_over()
    {
        Debug.Log("game over!");
        LevelLabel.enabled = false;
        gameOverSection.game_over(level);
        game_over_bool = true;
        Time.timeScale = 0f;
    }
    
    private string levelString(){
        string zero = "";
        if( level < 10 ){
            zero = "0";
        }
        return "Level: " + zero + level.ToString();
    }

    private void Awake(){
    }
    
    /*
    private void StartNewLevelTransition(){
        is_active_scene = true;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Base"));
    }
    */

    IEnumerator Unload(){
        yield return 0.1f;
        
        SceneManager.UnloadScene("AdditiveScene");
    }

    private void UnloadAdditiveScene(){
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Base"));
        // StartCoroutine(Unload());
        
        SceneManager.UnloadScene("AdditiveScene");
    }

    private void LoadAdditiveScene(){
        SceneManager.LoadScene("AdditiveScene", LoadSceneMode.Additive);
    }

    void Start(){
        LoadAdditiveScene();
        objectPooler = ObjectPooler.Instance;
    }

    void Update()
    {

        // set level to Level Label UI
        LevelLabel.text = levelString();

        // All this stuff for restart of the level
    
        int i = 0;

        if( Input.GetKeyDown("r") ){
            Time.timeScale = 1f;
            SceneManager.LoadScene("Base");
        }

        while (i < Input.touchCount) {
            
            Debug.Log("touch");
            if (Input.GetTouch (i).position.x > ScreenWidth / 2 || Input.GetTouch (i).position.x < ScreenWidth / 2){
                if( game_over_bool && (touches >= (int) Constants.max_touches) ){
                    Debug.Log("touch");
                    Time.timeScale = 1f;
                    SceneManager.LoadScene("Base");
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

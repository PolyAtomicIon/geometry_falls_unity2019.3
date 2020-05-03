using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine;
using Random=System.Random;
using TMPro;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{   
    public enum Constants
    {
        max_touches = 25,
        object_in_level = 20,
        obstacles_on_scene = 50
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

    // For getting random models for each level 
    public List<int> random_models_indexes;
    private int next_model_index = 0;
    public int max_models_number = 2;
    // end

    public float fall_down_speed;

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

    public int get_next_random_model_index(){

        if( random_models_indexes.Count == 0 )
            return -1;

        int res = random_models_indexes[next_model_index];

        next_model_index += 1;
        next_model_index %= max_models_number;

        return res; 
    }

    public void create_random_models_indexes(){
        Debug.Log(max_models_number);

        for(int i=0; i<max_models_number; i++)
            random_models_indexes.Add(i);
        
        random_models_indexes.Shuffle();
    }

    public void game_over()
    {
        Debug.Log("game over!");
        
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Base"));

        LevelLabel.enabled = false;
        gameOverSection.game_over(level);
        game_over_bool = true;
    }
    
    private string levelString(){
        string zero = "";
        if( level < 10 ){
            zero = "0";
        }
        return "Level: " + zero + level.ToString();
    }

    public void restartLevel(){
        Time.timeScale = 1f;
        SceneManager.LoadScene("Base");
    }

    public void exit(){
        SceneManager.LoadScene("MainMenu");
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

        if( Input.GetKeyDown("r") ){
            restartLevel();
        }

    }

}

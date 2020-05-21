using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine;
using Random=System.Random;
using Random2=UnityEngine.Random;
using TMPro;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{   
    public enum Constants
    {
        max_touches = 25,
        object_in_level = 8,
        obstacles_on_scene = 50
    }
    
    public static IEnumerator lerpColor(Material target, Color fromColor, Color toColor, float duration)
	{

		float counter = 0;

		while (counter < duration)
		{
			counter += Time.deltaTime;

			float colorTime = counter / duration;
			//Debug.Log(colorTime);

			//Change color
            target.SetColor("_BaseColor", Color.Lerp(fromColor, toColor, counter / duration) );
       
			//Wait for a frame
			yield return null;
		}

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

    public Material DissolveMaterial;
    public Material TunnelMaterial;
    public Color tunnel_color;

    // For getting random models for each level 
    public List<int> random_models_indexes;
    private int next_model_index = 0;
    public GameObject figure_plane;
    public int max_models_number = 2;
    // end

    public int palette = 0;

    public float fall_down_speed;

    private int level = 1;
    private int score = 0;

    public int get_score()
    {
        return score;
    }


    public void change_tunnel_color(Color new_color){
        tunnel_color = new_color;
        StartCoroutine( lerpColor( TunnelMaterial, TunnelMaterial.color, tunnel_color, 2f) );
    }

    public void start_next_level(){
        level+=1;
        Debug.Log("RELOAD THE SCENE");
        
        UnloadAdditiveScene();
        LoadAdditiveScene();
        
        Vector3 pos_t = figure_plane.transform.position;
        pos_t.y -= -95.1f;
        // change color of tunnel 
        figure_plane.transform.position = pos_t;
    }

    public void increment_score()
    {
        score += 1;

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
        palette = ThreadSafeRandom.ThisThreadsRandom.Next(4);
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

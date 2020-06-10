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
        object_in_level = 12,
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

    public float rotation_lvl = 2500f;

    public Player player;

    /*  
    generating positoins for obstacle, to
    place obstacle and track player
    */

    public float gap = -90f;
    public Vector3 gap_between;
    public Vector3 obstacle_position;
    public Vector3[] obstacle_positions = new Vector3[(int) Constants.object_in_level];
    public int current_obstacle = 0;

    public List<GameObject> obstacles_array = new List<GameObject>();
 
    public bool is_level_started = false;
    
    private void generate_obstacle_positions(){
        gap_between = new Vector3(0f, gap, 0f);
        obstacle_position = new Vector3(0f, gap/2 + gap, 0f);

        Debug.Log("SIZE OF OBS");   
        Debug.Log(obstacle_positions.Length);

        for(int i=0; i < obstacle_positions.Length; i++){
            obstacle_positions[i] = obstacle_position;
            obstacle_position += gap_between;
        }
    }

    private void rearrange_obstacles_array(){

        int length = obstacles_array.Count;

        Debug.Log("LENGTH of the array");
        Debug.Log(length);

        // Remove objects.
        obstacles_array.Reverse();

        for(int i = length - 1; i >= length - objectPooler.number_each_prefab; i--){
            obstacles_array.RemoveAt(i);  
        }
        
        obstacles_array.Reverse();
        // Done

        // Add objects

        for(int i = 1; i <= objectPooler.number_each_prefab; i++){
            GameObject obstacle = objectPooler.objectPool.Dequeue();
            obstacles_array.Add(obstacle);
            objectPooler.objectPool.Enqueue(obstacle);
        }

        obstacles_array.Shuffle();


    }

    /* End of generating obstacle positions */
    public int get_score()
    {
        return score;
    }

    public void change_tunnel_color(Color new_color){
        tunnel_color = new_color;
        TunnelMaterial.SetColor("_BaseColor", tunnel_color );

        // smooth no need, because one static palette for one instance
        //StartCoroutine( lerpColor( TunnelMaterial, TunnelMaterial.color, tunnel_color, 2f) );
    }

    public void start_next_level(){

        level += 1;

        Debug.Log("RELOAD THE SCENE");
        
        // reareange array of obstacles, add harder one, delete easy ones
        rearrange_obstacles_array();

        UnloadAdditiveScene();
        LoadAdditiveScene();

        Vector3 plane_pos = obstacle_positions[0];
        plane_pos.y -= 0.3f;

        figure_plane.transform.position = plane_pos;

        // current obstacle
        current_obstacle = 0;
    }

    public void increment_score()
    {
        score += 1;

        Debug.Log("Score: ");
        Debug.Log(score);
        Debug.Log("level: ");
        Debug.Log(level);
    }

    public int current_model_index = -1;
    public int get_next_random_model_index(){

        if( random_models_indexes.Count == 0 )
            return -1;

        int res = random_models_indexes[next_model_index];

        next_model_index += 1;
        next_model_index %= max_models_number;

        return res; 
        // return 0;
    }

    public int get_current_random_model_index(){
        if( current_model_index == -1 ){
            current_model_index = get_next_random_model_index();
        }
        return current_model_index;
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
        
        generate_obstacle_positions();

        LoadAdditiveScene();
        objectPooler = ObjectPooler.Instance;
        palette = ThreadSafeRandom.ThisThreadsRandom.Next(4);

        /* Plane */
        Vector3 plane_pos = obstacle_positions[0];
        plane_pos.y -= 0.3f;

        figure_plane.transform.position = plane_pos;


    }

    void Update()
    {

        // set level to Level Label UI
        LevelLabel.text = levelString();

        // changed in LevelManager.cs & Player.cs -> on collision
        if( is_level_started ){

            if( player.get_position_y_axis() < obstacle_positions[current_obstacle].y - gap - 2.5f ){    
                Vector3 position_f = obstacle_positions[current_obstacle];
                
                if( current_obstacle + 1 >= (int) object_in_level() ){
                    position_f = obstacle_positions[0];
                }

                position_f.y -= 0.3f;
                figure_plane.transform.position = position_f;
            }
            
            if( player.get_position_y_axis() < obstacle_positions[current_obstacle].y - 2.5f ){
                player.increment_score();
                increment_score();
                current_obstacle++;
            }

        }
        
        if( current_obstacle >= (int) object_in_level()-1 ){
            fall_down_speed = player.get_velocity_y_axis();
            start_next_level();
        }    

        // All this stuff for restart of the level
        if( Input.GetKeyDown("r") ){
            restartLevel();
        }

    }

}

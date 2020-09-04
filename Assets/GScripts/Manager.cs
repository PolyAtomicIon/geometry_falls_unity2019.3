using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine;
using Random=System.Random;
using Random2=UnityEngine.Random;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;
using System;   

class LevelResult{
    public int level;
}

public class Manager : MonoBehaviour
{   
    public enum Constants
    {
        max_touches = 25,
        object_in_level = 9,
        obstacles_on_scene = 50
    }
    
    public static IEnumerator lerpColorMaterial(Material target, Color fromColor, Color toColor, float duration = 2f)
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

    // progression
    public Slider progression;
    public Image progressionColor;

    public GameOver gameOverSection;

    public Material TunnelMaterial;
    public Color tunnel_color;

    // For getting random models for each level 
    public List<int> random_models_indexes;
    private int next_model_index = 0;
    public GameObject figure_plane;
    public int max_models_number = 4;
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
    
    public Vector3[] obstacle_positions = new Vector3[(int) Constants.object_in_level + 1];
    public float[] obstacle_angles = new float[(int) Constants.object_in_level + 1];
    public float[] degree_levels = {180.0f, 90.0f, 30.0f, 15.0f, 7.5f};

    
    [SerializeField]
    private float Complexity = 1;
    [SerializeField]
    private float DegreeLevel = 0;
    private float complexityPeriod = 0;
    private float StartDegreeLevel = 0.5f;
    private float last_degree = -1;

    [SerializeField]
    private float Limit;
    [SerializeField]
    private int[] complexity_counter = new int[20];

    public int current_obstacle = 0;

    public List<GameObject> obstacles_array = new List<GameObject>();
    
    public bool is_level_active = false;
    
    public AudioSource bgMusic;
    public List<AudioClip> audios;
    public List<AudioSource> soundEffects;

    public Animator SplashScreenAnimator;
    public GameObject Loader;

    public GameObject allLevelsPassed;


    public void SetAudio(){
        AudioListener audioListener = GetComponent<AudioListener>(); 
        audioListener.enabled = ( audioListener.enabled ^ true );
    }

    private void generate_obstacle_positions(){
        gap_between = new Vector3(0f, gap, 0f);
        obstacle_position = new Vector3(0f, gap/2 + gap, 0f);

        // Debug.Log("SIZE OF OBS");   
        // Debug.Log(obstacle_positions.Length);

        for(int i=0; i < obstacle_positions.Length; i++){
            obstacle_positions[i] = obstacle_position;
            obstacle_position += gap_between;
        }
    }

    float convert_to_angle(int level){
        // something with if s or math
        // float degree = degree_levels[ level ];
        // float multiplier = ThreadSafeRandom.ThisThreadsRandom.Next( (int) 4 ) + 1;
        // Debug.Log(multiplier);
        // float result = degree * multiplier;
        // result %= 360;
        // while(result == last_degree)
        //     result += degree;
        // last_degree = result;
        // return result;


        // new Mechanics, sorry math :(
        float multiplier = ThreadSafeRandom.ThisThreadsRandom.Next( 3 );
        
        float start_degree = -90f;
        float degree = 120f;

        float initial_degree = start_degree - degree;
        float final_degree = start_degree + degree;

        float resultAngle = initial_degree + degree*multiplier;

        // to not repeat angles
        while(resultAngle == last_degree){
            resultAngle += degree;
        }

        if( resultAngle > final_degree ){
            resultAngle = initial_degree;
        }
        if( resultAngle < initial_degree ){
            resultAngle = final_degree;
        }

        last_degree = resultAngle;

        return resultAngle;

    }

    public void rearrange_obstacles_array(){

        // Limit = objectPooler.obstacles[1].Count;
        Limit = 4;
        Complexity = Math.Max(1.0f, Complexity);
        // DegreeLevel = Math.Max(1.0f, DegreeLevel);

        // Complexity = 1f;
        // DegreeLevel = 0f;

        // int length = obstacles_array.Count;
        int length = objectPooler.object_in_level;

        // we'll delete all obstacles, so no obstacle is repeated
        for(int i=0; i<20; i++)
            complexity_counter[i] = 0;

        if( obstacles_array.Count == length ){
            for(int i = 0; i < length; i++){
                obstacles_array[i].SetActive(false);
            }
        }
        
        obstacles_array.Clear();

        for(int i = 0; i < length; i++){
            
            // Find obstacle, insert, increment counters
            Debug.Log("Complexity" + (int) Complexity);
            
            // New added
            // if level > 3, start Random
            if( level > 3 )
                Complexity = ThreadSafeRandom.ThisThreadsRandom.Next( objectPooler.max_complexity_value ) + 1;

            GameObject c_obstacle = objectPooler.obstacles[ (int) Complexity ].Dequeue();
            // complexity_counter[(int) Complexity]++;
            obstacles_array.Add(c_obstacle);
            objectPooler.obstacles[ (int) Complexity ].Enqueue(c_obstacle);

            // Set angle and convert
            obstacle_angles[i] = convert_to_angle( (int) DegreeLevel );

            // Change complexity and degreesLevel
            Complexity += complexityPeriod;

            float angle = degree_levels[(int) DegreeLevel];
            int multiplier = ThreadSafeRandom.ThisThreadsRandom.Next( (int) (180 / angle) ) + 1;
            DegreeLevel += Math.Min(0.35f, (angle / 180)) * multiplier;

            if( DegreeLevel >= 4 ){
                DegreeLevel = StartDegreeLevel;
                StartDegreeLevel += 0.5f;
                StartDegreeLevel %= 4;
            }

            // Limit and - or + , random

            if( Complexity > objectPooler.max_complexity_value){
                Complexity = 1.0f; 
            }
        }

    }

    public Color getModelsandTunnelsMaterialColor(){
        return objectPooler.getMaterialById(1).GetColor("_BaseColor");
    }

    public Material getModelsandTunnelsMaterial(){
        return objectPooler.getMaterialById(1);
    }

    /* End of generating obstacle positions */
    public int get_score()
    {
        return score;
    }

    public void change_tunnel_color(Color new_color){
        tunnel_color = new_color;
        // TunnelMaterial.SetColor("_BaseColor", tunnel_color );

        // smooth no need, because one static palette for one instance
        //StartCoroutine( lerpColorMaterial( TunnelMaterial, TunnelMaterial.color, tunnel_color, 2f) );
    }

    IEnumerator all_levels_passed(int id, int levels){
        // player.rb.velocity = new Vector3(0f, 0f, 0f);
        // player.gameObject.SetActive(false);

        allLevelsPassed.SetActive(true);

        yield return new WaitForSeconds(1.75f);

        gameOverSection.game_over(level);
    }

    public void start_next_level(){

        if( game_over_bool ) return;

        int levels = PlayerPrefs.GetInt("levels");
        int id = PlayerPrefs.GetInt("id");  

        // To stop event game, if player passed all levels

        level += 1;

        if( level > levels && id != -1 ){
            game_over_bool = true;
            StartCoroutine( all_levels_passed(id, levels) ); 
            return;
        }

        // Debug.Log("RELOAD THE SCENE");
        
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
    }

    public int current_model_index = -1;
    public int get_next_random_model_index(){

        if( random_models_indexes.Count == 0 )
            return -1;

        int res = random_models_indexes[next_model_index];

        next_model_index += 1;
        next_model_index %= max_models_number;

        return res; 
    }

    public int get_current_random_model_index(){
        if( current_model_index == -1 ){
            current_model_index = get_next_random_model_index();
        }
        return current_model_index;
    }

    public void create_random_models_indexes(){
        // Debug.Log(max_models_number);

        for(int i=0; i<max_models_number; i++)
            random_models_indexes.Add(i);
        
        random_models_indexes.Shuffle();
    }

   
    public void game_over()
    {

        Debug.Log("game over!");

        is_level_active = false;
        game_over_bool = true;
        gameOverSection.game_over(level);
    }
    
    private string levelString(){

        int levels = PlayerPrefs.GetInt("levels");

        level = Math.Min(level, levels);

        string zero = "";
        if( level < 10 ){
            zero = "0";
        }
        return "УРОВЕНЬ: " + zero + level.ToString();
    }

    private float levelProgression(){
        int mx_sz = (int) object_in_level() + 1;
        int cur_o = current_obstacle + 1;
        return cur_o * 200.0f / mx_sz;
    }

    public void restartLevel(){
        Time.timeScale = 1f;
        SceneManager.LoadScene("Base");
    }

    public void exit(){
        SceneManager.LoadScene("MainMenu");
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
        
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Base"));
        // yield return true;
    }

    IEnumerator runLoadingAnimation(){
        // Syncing  Data or Loading Animation, no matter what is written as an argument
        SplashScreenAnimator.Play("Login to Loading");
        yield return new WaitForSeconds(0.75f);
        LoadAdditiveScene();
    }

    // public void RotateObject(int direction = 0){

    //     if( game_over_bool )
    //         return;

    //     Debug.Log("Direction " + direction );
    //     player.Turn(direction); 
    
    // }

    void Start(){

        // int is_tutorial = PlayerPrefs.GetInt("tutorial");
        int is_tutorial = -1;

        if( is_tutorial == -1 ){
            StartCoroutine( runLoadingAnimation() );
            palette = ThreadSafeRandom.ThisThreadsRandom.Next(4);
        }
        else{
            // if tutorial
            Loader.SetActive(false);
            LoadAdditiveScene();

            PlayerPrefs.SetInt("tutorial", -1);
            palette = 3;
        }

        generate_obstacle_positions();

        objectPooler = ObjectPooler.Instance;

        /* Plane */
        Vector3 plane_pos = obstacle_positions[0];
        plane_pos.y -= 0.3f;

        figure_plane.transform.position = plane_pos;

        objectPooler.palettes[palette].colors[1].a = 100;
        progressionColor.color = objectPooler.palettes[palette].colors[1];

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Base"));

        complexityPeriod = 1f / object_in_level();
        Debug.Log("complexity period = " + complexityPeriod);

    }

    void Update()
    {

        if( SceneManager.GetActiveScene().name != "Base" )
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Base"));

        // set level to Level Label UI
        LevelLabel.text = levelString();

        progression.value = levelProgression();

        // changed in LevelManager.cs & Player.cs -> on collision
        if( is_level_active ){
            
            current_obstacle = Math.Min(current_obstacle, (int) obstacles_array.Count - 1);

            if( player.get_position_y_axis() < obstacle_positions[current_obstacle].y - gap - 0.5f ){    
                Vector3 position_f = obstacle_positions[current_obstacle];
                
                if( current_obstacle + 1 > (int) object_in_level() ){
                    position_f = obstacle_positions[0];
                }

                position_f.y -= 0.3f;
                figure_plane.transform.position = position_f;
            }
            
            if( player.get_position_y_axis() < obstacle_positions[current_obstacle].y - 0.5f ){
                player.increment_score();
                increment_score();
                current_obstacle++;
            }

        }
        
        if( current_obstacle + 1 > (int) object_in_level() ){
            fall_down_speed = player.get_velocity_y_axis();
            start_next_level();
        }    

        // All this stuff for restart of the level
        if( Input.GetKeyDown("r") ){
            restartLevel();
        }

    }

}

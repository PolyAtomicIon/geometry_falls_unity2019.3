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
    public GameObject newHighscore_panel;

    public LevelProgession levelProgession;


    // For getting random models for each level 
    public List<int> random_models_indexes;
    private int next_model_index = 0;
    public GameObject figure_plane;
    private int max_models_number;
    
    public void setMaxModelsNumber(int modelsNumber){
        max_models_number = modelsNumber;
    }
    // end

    public int palette = 0;
    int paletteCount;

    public float fall_down_speed;

    private int level = 1;
    private int score = 0;

    public Player player;

    /*  
    generating positoins for obstacle, to
    place obstacle and track player
    */

    public float gap = -90f;
    public Vector3 gap_between;
    
    public Vector3[] obstacle_positions = new Vector3[(int) Constants.object_in_level + 1];
    public float[] obstacle_angles = new float[(int) Constants.object_in_level + 1];
    public int[] obstacle_complexities = new int[(int) Constants.object_in_level + 1];

    
    int maxComplexity = 0;
    [SerializeField]
    private float Complexity = 1;
    [SerializeField]
    private float DegreeLevel = 0;
    private float complexityPeriod = 0;
    private float StartDegreeLevel = 0.5f;
    private float last_degree = -1;

    [SerializeField] private float obstacle_start_degree = -90f;
    [SerializeField] private float degree = 120f;


    public int current_obstacle = 0;
    int NextObstacleIndex = 0;

    public Queue<Obstacle> Obstacles = new Queue<Obstacle>();
    
    public bool is_level_active = false;
    
    public AudioM audioManager;

    public Animator SplashScreenAnimator;
    public GameObject Loader;

    public GameObject allLevelsPassed;
    public GameObject Blur;
    public GameObject ServerError_panel;

    int current_highscore;

    public void ServerError(){
        ServerError_panel.SetActive(true);
    }

    public static string get_token(){
        if( !PlayerPrefs.HasKey("auth_token") )
            return "";
        return PlayerPrefs.GetString("auth_token");
    }

    public void SetAudio(){
        AudioListener audioListener = GetComponent<AudioListener>(); 
        audioListener.enabled = ( audioListener.enabled ^ true );
    }

    public void setMaxComplexityValue(int maxC){
        maxComplexity = maxC;
    }

    private void generate_obstacle_positions(){

        Vector3 obstacle_position = new Vector3(0f, gap/2 + gap, 0f);

        for(int i=0; i <= object_in_level(); i++){
            obstacle_positions[i] = obstacle_position;
            obstacle_position += gap_between;
        }
    }

    float getRandomObstacleDegree(){

        int angles = 360 / (int) degree;

        float multiplier = ThreadSafeRandom.ThisThreadsRandom.Next(angles);

        float initial_degree = obstacle_start_degree;
        float final_degree = obstacle_start_degree + degree * (angles-1);
        float resultAngle = obstacle_start_degree + degree * multiplier;

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

    public void initialize_object(GameObject objectToSpawn, Vector3 position, bool model, float angle){
        
        objectToSpawn.SetActive(false);
        objectToSpawn.SetActive(true);

        objectToSpawn.transform.position = position;              
                    
        // if Obstacle
        if( !model ){
            // Random rotation
            // angle - 180 -> animation -> see Obstacle.Start()
            // objectToSpawn.transform.eulerAngles = new Vector3(-90f, angle - 180.0f, 0);
            objectToSpawn.transform.eulerAngles = new Vector3(-90f, angle, 0);
        }

        // If Model
        if( model ){  
            // objectToSpawn.transform.eulerAngles = new Vector3(0f, 0f, 0f);
            objectToSpawn.transform.eulerAngles = new Vector3(0f, 0f, objectToSpawn.transform.eulerAngles.z);
        }

        // Call the OnObjectSpwan, differs for player and obstacle.
        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
        pooledObj.OnObjectSpawn();

    }

    void clearAndDeactivateObstacles(){
        while( Obstacles.Count != 0 ){
            // DeleteTopObstacleFromQueue();
            StartCoroutine( DeleteTopObstacleFromQueue() );
        }
    }

    public void AddObstacleToQueue(Obstacle c_obstacle){
        Obstacles.Enqueue(c_obstacle);
    }

    public IEnumerator DeleteTopObstacleFromQueue(){

        if( Obstacles.Count == 0 ) yield return null;

        Obstacle c_obstacle = Obstacles.Dequeue();
        yield return new WaitForSeconds(0.5f);
        c_obstacle.prefab.SetActive(false);
    }

    public void nextObstacleToInitialize(){

        if( NextObstacleIndex >= object_in_level() ) {
            return;
        }

        Vector3 pos = obstacle_positions[NextObstacleIndex];
        float angle = obstacle_angles[NextObstacleIndex];

        if( NextObstacleIndex >= 2 )
            StartCoroutine( DeleteTopObstacleFromQueue() );

        int c_complexity = obstacle_complexities[NextObstacleIndex];
        
        Obstacle c_obstacle = objectPooler.obstacles[c_complexity].Dequeue();
        AddObstacleToQueue(c_obstacle);
        objectPooler.obstacles[c_complexity].Enqueue(c_obstacle);

        NextObstacleIndex++;

        initialize_object(c_obstacle.prefab, pos, false, angle);

    }

    public void rearrange_obstacles_array(){

        Complexity = Math.Max(1.0f, Complexity);

        clearAndDeactivateObstacles();

        for(int i = 0; i < object_in_level(); i++){
            
            // if level > 3, generate Random
            if( level > 3 )
                Complexity = ThreadSafeRandom.ThisThreadsRandom.Next( maxComplexity ) + 1;
           
            obstacle_complexities[i] = (int) Complexity;
            obstacle_angles[i] = getRandomObstacleDegree();

            // Change complexity and degreesLevel
            Complexity += complexityPeriod;

            if( Complexity > maxComplexity){
                Complexity = 1.0f; 
            }
        }

    }

    public Color getModelsMaterialColor(){
        return objectPooler.palettes[palette].getModelColor();
    }

    public Color getObstaclesMaterialColor(){
        return objectPooler.palettes[palette].getObstacleColor();
    }

    public Material getModelsMaterial(){
        return objectPooler.materials.getModelMaterial();
    }

    public Material getTunnelsMaterial(){
        return objectPooler.materials.getTunnelsMaterial();
    }

    /* End of generating obstacle positions */
    public int get_score()
    {
        return score;
    }

    IEnumerator all_levels_passed(int id, int levels){
        // player.rb.velocity = new Vector3(0f, 0f, 0f);
        // player.gameObject.SetActive(false);

        allLevelsPassed.SetActive(true);
        Blur.SetActive(true);

        yield return new WaitForSeconds(1.75f);

        gameOverSection.game_over(levels+1);
    }

    public void start_next_level(){

        if( game_over_bool ) return;

        int levels = PlayerPrefs.GetInt("levels");
        int id = PlayerPrefs.GetInt("id");

        // To stop event game, if player passed all levels

        levelProgession.refresh();
        score = 0;
        
        Debug.Log( current_highscore + " vs  " + level );

        if( current_highscore < level ){
            newHighscore(level);
        }

        level += 1;

        if( level > levels && id != -1 ){
            game_over_bool = true;
            StartCoroutine( all_levels_passed(id, levels) );
            return;
        }

        palette += 1;
        palette %= paletteCount;

        rearrange_obstacles_array();

        UnloadAdditiveScene();
        LoadAdditiveScene();

        SetPlanesInitialParameters();

        // current obstacle
        current_obstacle = 0;
        NextObstacleIndex = 0;
        
        SetProgressionBarColor();
        audioManager.levelPass();

    }

    void newHighscore(int score){
        Highscore.setHighscore(score);
        newHighscore_panel.SetActive(true);
    }

    void SetPlanesInitialParameters(){
        Vector3 plane_pos = obstacle_positions[0];
        plane_pos.y -= 0.3f;

        figure_plane.transform.position = plane_pos;
    }

    void SetProgressionBarColor(){
        objectPooler.palettes[palette].colors[1].a = 100;
        progressionColor.color = objectPooler.palettes[palette].colors[1];
    }

    public void increment_score()
    {
        score += 1;
        levelProgession.increment(score - 1);
    }

    public int currentModelIndex = -1;
    
    public void setCurrentModelIndex(int index){
        currentModelIndex = index;
    }

    public int getCurrentModelIndex(){
        return currentModelIndex;
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

        // string zero = "";
        // if( level < 10 ){
        //     zero = "0";
        // }
        
        return level.ToString();
    }

    public void restartLevel(){
        Time.timeScale = 1f;
        SceneManager.LoadScene("Base");
    }

    public void exit(){
        SceneManager.LoadScene("newMainScene");
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
        yield return new WaitForSeconds(1f);
        LoadAdditiveScene();
    }

    void Start(){

        gap_between = new Vector3(0f, gap, 0f);

        generate_obstacle_positions();

        StartCoroutine( runLoadingAnimation() );

        objectPooler = ObjectPooler.Instance;
        
        paletteCount = objectPooler.palettes.Count;
        palette = ThreadSafeRandom.ThisThreadsRandom.Next(paletteCount);

        /* Plane */
        SetPlanesInitialParameters();

        SetProgressionBarColor();

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Base"));

        complexityPeriod = 1f / object_in_level();
        Debug.Log("complexity period = " + complexityPeriod);

        current_highscore = Highscore.getHighscore();

    }

    void Update()
    {

        if( SceneManager.GetActiveScene().name != "Base" )
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Base"));

        // set level to Level Label UI
        LevelLabel.text = levelString();

        // changed in LevelManager.cs & Player.cs -> on collision
        if( is_level_active ){
            
            current_obstacle = Math.Min(current_obstacle, object_in_level() - 1);

            if( current_obstacle + 1 <= object_in_level() && player.get_position_y_axis() < obstacle_positions[current_obstacle + 1].y - gap + 0.5f ){    
                Vector3 position_f = obstacle_positions[current_obstacle+1];
                
                if( current_obstacle + 1 >= (int) object_in_level() ){
                    position_f = obstacle_positions[0];
                }

                position_f.y -= 0.3f;
                figure_plane.transform.position = position_f;
            }
            
            if( player.get_position_y_axis() < obstacle_positions[current_obstacle].y - 0.5f ){
                player.increment_score();
                increment_score();
                
                nextObstacleToInitialize();
                current_obstacle++;
            }

        }
        
        // if( current_obstacle + 1 > (int) object_in_level() ){
        if( player.get_position_y_axis() <  (object_in_level() + 1) * gap + 20f){
            fall_down_speed = player.get_velocity_y_axis();
            start_next_level();
        }    

        // All this stuff for restart of the level
        if( Input.GetKeyDown("r") ){
            restartLevel();
        }

    }

}

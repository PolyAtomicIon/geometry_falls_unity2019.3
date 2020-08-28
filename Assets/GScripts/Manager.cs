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
        object_in_level = 6,
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

    // progression
    public Slider progression;
    public Image progressionColor;

    public GameOver gameOverSection;

    public Material DissolveMaterial;
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
    
    public Vector3[] obstacle_positions = new Vector3[(int) Constants.object_in_level];
    public float[] obstacle_angles = new float[(int) Constants.object_in_level];
    public float[] degree_levels = {180.0f, 90.0f, 30.0f, 15.0f, 7.5f};
    // public float[] degree_levels = {7.5f, 15.0f, 30.0f, 90.0f, 180.0f};

    
    [SerializeField]
    private float Complexity = 1;
    [SerializeField]
    private float DegreeLevel = 0;
    private float StartDegreeLevel = 0.5f;
    private float last_degree = -1;

    [SerializeField]
    private float Limit;
    [SerializeField]
    private int[] complexity_counter = new int[20];

    public int current_obstacle = 0;

    public List<GameObject> obstacles_array = new List<GameObject>();
    
    public bool is_level_started = false;
    
    public AudioSource bgMusic;
    public List<AudioClip> audios;
    public List<AudioSource> soundEffects;

    public Animator SplashScreenAnimator;
    public GameObject Loader;

    public FortuneWheelManager FortuneWheelManager;
    public List<int> values_randomizer;
    public int present_id = -1;

    bool is_present = false;
    public GameObject couponInformation;
    public GameObject couponInformation2;

    public GameObject allLevelsPassed;

    public void GiveAward(){
        if( is_present )
            couponInformation.SetActive(true);
        else
            couponInformation2.SetActive(true);
    }

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
        float degree = degree_levels[ level ];
        float multiplier = ThreadSafeRandom.ThisThreadsRandom.Next( (int) 4 ) + 1;
        Debug.Log(multiplier);
        float result = degree * multiplier;
        result %= 360;
        while(result == last_degree)
            result += degree;
        last_degree = result;
        return result;
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
            Debug.Log((int) Complexity);
            GameObject c_obstacle = objectPooler.obstacles[ (int) Complexity ].Dequeue();
            // complexity_counter[(int) Complexity]++;
            obstacles_array.Add(c_obstacle);
            objectPooler.obstacles[ (int) Complexity ].Enqueue(c_obstacle);

            // Set angle and convert
            obstacle_angles[i] = convert_to_angle( (int) DegreeLevel );

            // Change complexity and degreesLevel
            Complexity += 0.13f;

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

    IEnumerator all_levels_passed(int id, int levels){
        // player.rb.velocity = new Vector3(0f, 0f, 0f);
        // player.gameObject.SetActive(false);

        allLevelsPassed.SetActive(true);

        yield return new WaitForSeconds(1.75f);

        StartCoroutine( GetPrize(id, levels) );
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

        // Debug.Log("Score: ");
        // Debug.Log(score);
        // Debug.Log("level: ");
        // Debug.Log(level);
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
        // return 1;
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

    public string get_token(){
        if( !PlayerPrefs.HasKey("auth_token") )
            return "";
        return PlayerPrefs.GetString("auth_token");
    }

    IEnumerator GetPrize(int id, int levels_done)
    {
        WWWForm form = new WWWForm();
        form.AddField("level", levels_done); 

        string url = "http://94.247.128.162/api/game/events/" + id.ToString() + "/present/";

        // Debug.Log( "LEVELS done " + levels_done.ToString());
        // Debug.Log( url );

        using (UnityWebRequest www = UnityWebRequest.Post(url, form ))
        {
            Debug.Log(get_token());
            
            www.SetRequestHeader("Authorization", get_token());
            
            yield return www.SendWebRequest();

            Debug.Log("Request sent!");
        
            int present_value = 0;

            Debug.Log(www.isNetworkError);      
            Debug.Log(www.isHttpError); 
            

            // Retrieave Data is no errors have been found
            // Else just show empty randomizer
            if ( !www.isNetworkError && !www.isHttpError ){

                Debug.Log("env");
                JSONNode details = JSONNode.Parse(www.downloadHandler.text);
                Debug.Log(details);

                // 1. GET LIST OF PRESENTS
                // 2. Get index of present
                // 3. SEND INFO TO FORTUNE WHEEL

                for(int i=0; i<details["presents"].Count; i++)
                    if( details["presents"][i]["win"] ){
                        present_value = details["presents"][i]["value"];
                        values_randomizer.Add( details["presents"][i]["value"] );
                        is_present = true;
                        break;
                    }

                for(int i=0; i<details["presents"].Count; i++){
                    if( values_randomizer.Count >= 11 ) break;
                    if( details["presents"][i]["win"] ){
                        continue;
                    }
                    values_randomizer.Add( details["presents"][i]["value"] );
                }
                    
                if( details["present"] != null ){
                    couponInformation.GetComponentsInChildren<TMP_Text>()[3].text = details["present"]["provider"]["name"];
                    couponInformation.GetComponentsInChildren<TMP_Text>()[5].text = details["present"]["value"];
                }

            }

            values_randomizer.Add(0);

            int tmp_id = 0;

            // number of Present coupon value is repeated, it should be less
            int pr_sz = 12 / values_randomizer.Count;
            if( values_randomizer.Count > 1 ){
                pr_sz --;
            }
            int pr_cnt = 1; // number of Present coupon inserted

            while( values_randomizer.Count < 12 ){

                if( values_randomizer[tmp_id] == present_value && pr_cnt < pr_sz ){
                    values_randomizer.Add(values_randomizer[tmp_id]);
                    pr_cnt++;
                }
                else if( values_randomizer[tmp_id] != present_value ){
                    values_randomizer.Add(values_randomizer[tmp_id]);
                }

                tmp_id++;
                tmp_id %= values_randomizer.Count;
            }
            // Debug.Log(values_randomizer);
            values_randomizer.Shuffle();

            for(int i=0; i<12; i++){
                if( present_value == values_randomizer[i] ){
                    present_id = i;
                    break;
                }
            }

            FortuneWheelManager.ParsedData();
            
            gameOverSection.game_over(level, true);

            /*
            {
                "presents": [{
                    "id": 1,
                    "value": 10,
                    "provider": {
                        "id": 1,
                        "name": "KFC"
                    },
                    "win": true
                }],
                "present": {
                    "id": 2,
                    "key": "8x2OgcrpjZOOoZcP2Qw1KKkalYzU9u15",
                    "value": 10,
                    "provider": {
                        "id": 1,
                        "name": "KFC"
                    }
                }
            }
            */
        }
    }

    public void game_over()
    {
        // sound
        soundEffects[1].Play();
        Debug.Log("game over!");
        
        // get ID of Event
        int id = PlayerPrefs.GetInt("id");
        int levels = PlayerPrefs.GetInt("levels");
     
        if( id != -1 ){
            Debug.Log("Getting Prize");
            // Debug.Log(id);
            Debug.Log(level);
            StartCoroutine(GetPrize(id, level-1));
            // gameOverSection.game_over(level, true);
        }
        else{
            Debug.Log("Chill, just practice");
            gameOverSection.game_over(level, false);
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Base"));

        // LevelLabel.enabled = false;
        game_over_bool = true;
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

        // AUDIO Settings

        int audioID = PlayerPrefs.GetInt("bgAudioID");
        bgMusic.clip = audios[audioID];

        bgMusic.Play();

        if( PlayerPrefs.GetInt("isAudio") == 0 ){
            bgMusic.volume = 0.0f;
        } 

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Base"));

    }

    void Update()
    {

        if( SceneManager.GetActiveScene().name != "Base" )
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("Base"));

        // set level to Level Label UI
        LevelLabel.text = levelString();

        progression.value = levelProgression();

        // changed in LevelManager.cs & Player.cs -> on collision
        if( is_level_started ){
            
            current_obstacle = Math.Min(current_obstacle, (int) obstacles_array.Count - 1);

            if( player.get_position_y_axis() < obstacle_positions[current_obstacle].y - gap - 2.5f ){    
                Vector3 position_f = obstacle_positions[current_obstacle];
                
                if( current_obstacle + 1 > (int) object_in_level() ){
                    position_f = obstacle_positions[0];
                }

                position_f.y -= 0.3f;
                figure_plane.transform.position = position_f;
            }
            
            if( player.get_position_y_axis() < obstacle_positions[current_obstacle].y - 2.5f ){
                player.increment_score();
                increment_score();
                // sound
                soundEffects[0].Play();
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

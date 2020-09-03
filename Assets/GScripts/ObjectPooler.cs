using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine; 
using RandomS=System.Random;
using UnityEngine.SceneManagement; 
using Random=UnityEngine.Random;
using System.Threading;

public class ObjectPooler : MonoBehaviour
{

    // player spawning point
    public Vector3 VectorZero = Vector3.zero;
    
    private const int number_of_colors = 4;

    [System.Serializable]
    public class Obstacle{
        public GameObject prefab;
        public int complexity;
    }

    [System.Serializable]
    public class Pool{
        public string tag;
        public GameObject prefab;
        public int size;
        public List<Obstacle> obstacles_prefab;
    }

    [System.Serializable]
    public class Materials{

        public Material[] materials_list = new Material[number_of_colors];

        private int material_index = 0;
    
        public int next_material(){
            int result = material_index;
            material_index = (material_index + 1) % number_of_colors;
            return result;
        }

    }

    [System.Serializable]
    public class Palette{
        public Color[] colors = new Color[number_of_colors];
        public Color[] emission_colors = new Color[number_of_colors];
    }

    #region Singleton

    public static ObjectPooler Instance;

    private void Awake(){
        Instance = this;
    }

    #endregion

    public int max_complexity_value = 5;

    public List<Pool> models;
     
    public List<string> models_tag;  

    public List<Palette> palettes;
    public Materials materials;

    public Dictionary<string, GameObject> modelsDictionary;

    private int random_palette = 0;
    private int last_random_index = -1;

    public Manager game_manager;

    public int object_in_level = 12;
    public int number_each_prefab = 8;
    
    public Queue<GameObject>[] obstacles = new Queue<GameObject>[30];

    public void Start()
    {
        
        game_manager = FindObjectOfType<Manager>();
        
        game_manager.max_models_number = models.Count;
        game_manager.create_random_models_indexes();

        object_in_level = (int) game_manager.object_in_level();
        number_each_prefab = object_in_level;

        int start_complexity = 1;

        int id = PlayerPrefs.GetInt("id");

        // if id == -1 -> it's Practice game
        if( id == -1 ){
            max_complexity_value = 5;
            start_complexity = 1;
        }

        modelsDictionary = new Dictionary<string, GameObject>();

        // create for every type of prefab 'number_each_prefab' clones
        Pool pool = models[game_manager.get_current_random_model_index()];
        
        foreach (Obstacle obstacle in pool.obstacles_prefab){
            
            for(int i=0; i<number_each_prefab; i++){

                // Debug.Log( obstacle.complexity );

                GameObject obstacle_prefab = Instantiate(obstacle.prefab) as GameObject;
                obstacle_prefab.SetActive(false);

                // Store by complexity, complexity is unique
                if( obstacles[obstacle.complexity] == null ){
                    obstacles[obstacle.complexity] = new Queue<GameObject>();
                }
                obstacles[obstacle.complexity].Enqueue(obstacle_prefab);

            }

        }

        // Initiate first level objects ....
        // game_manager.obstacles_array;
        game_manager.rearrange_obstacles_array();
        

        // Player's model, add tag, add to dictionary
        
        foreach(Pool pl in models)
            models_tag.Add(pl.tag);

        GameObject model = Instantiate(pool.prefab) as GameObject;
        model.SetActive(false);
        modelsDictionary.Add(pool.tag, model);
    
    }

}

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine; 
using RandomS=System.Random;
using UnityEngine.SceneManagement; 
using Random=UnityEngine.Random;
using System.Threading;

[System.Serializable]
public class Obstacle{
    public GameObject prefab;
    public int complexity;

    public Obstacle(int c, GameObject p){
        complexity = c;
        prefab = p;
    }

}

public class ObjectPooler : MonoBehaviour
{

    // player spawning point
    public Vector3 VectorZero = Vector3.zero;
    
    private const int number_of_colors = 4;

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

        public Material GetMaterial(int id){
            return materials_list[id];
        }

        public Material getObstacleMaterial(){
            return materials_list[0];
        }

        public Material getModelMaterial(){
            return materials_list[1];
        }

    }

    [System.Serializable]
    public class Palette{
        public Color[] colors = new Color[number_of_colors];
        public Color[] emission_colors = new Color[number_of_colors];
    
        public Color getObstacleColor(){
            return colors[0];
        }

        public Color getModelColor(){
            return colors[1];
        }
    }

    class LevelComplexity{
        int currentMin = 1;
        int currentMax = 5;
        
        int easyMin = 1;
        int easyMax = 3;

        public int Min(){
            return this.currentMin;
        }

        public int Max(){
            return this.currentMax;
        }

        public void changeToEasyMode(){
            currentMax = easyMax;
            currentMin = easyMin;
        }

        public bool IsComplexityInRange(int complexity){
            return complexity >= Min() && complexity <= Max();
        }

    }

    #region Singleton

    public static ObjectPooler Instance;

    private void Awake(){
        Instance = this;
    }

    #endregion

    public List<Pool> models;
     
    public List<string> models_tag;  

    public List<Palette> palettes;
    public Materials materials;

    public Dictionary<string, GameObject> modelsDictionary;

    private int random_palette = 0;
    private int last_random_index = -1;

    public Manager game_manager;

    int randomModel;

    private int object_in_level;
    private int number_each_prefab = 3;
    
    public Queue<Obstacle>[] obstacles = new Queue<Obstacle>[30];

    LevelComplexity levelComplexity = new LevelComplexity();

    public Material getMaterialById(int id){
        return materials.GetMaterial(id);
    }

    void setRandomModelIndex(){
        randomModel = ThreadSafeRandom.ThisThreadsRandom.Next(models.Count);
        game_manager.setCurrentModelIndex(randomModel);
    }

    void DefineLevelComplexity(){
        int id = PlayerPrefs.GetInt("id");

        // if id == -1 -> it's Practice game
        if( id == -1 ){
            levelComplexity.changeToEasyMode();
        }
    }

    void SetGameObjectsMaterial(GameObject gObject, bool obstacle = true){
        Renderer rd = gObject.GetComponent<Renderer>();
        if( obstacle )
            rd.material = materials.getObstacleMaterial();
        else
            rd.material = materials.getModelMaterial();
    }

    void InstantiateObstacleAndAddToQueue(int complexity, GameObject g_obstacle){
        g_obstacle = Instantiate(g_obstacle) as GameObject;
        g_obstacle.SetActive(false);

        // Store by complexity, complexity is unique
        if( obstacles[complexity] == null ){
            obstacles[complexity] = new Queue<Obstacle>();
        }

        SetGameObjectsMaterial(g_obstacle);

        Obstacle n_obstacle = new Obstacle(complexity, g_obstacle);

        obstacles[complexity].Enqueue(n_obstacle);
    }

    public void Start()
    {
        
        game_manager = FindObjectOfType<Manager>();
        
        game_manager.setMaxModelsNumber(models.Count);

        object_in_level = game_manager.object_in_level();

        setRandomModelIndex();
        DefineLevelComplexity();

        modelsDictionary = new Dictionary<string, GameObject>();
        Pool pool = models[randomModel];

        int maxComplexity = 0;

        foreach (Obstacle obstacle in pool.obstacles_prefab){
            
            if( !levelComplexity.IsComplexityInRange(obstacle.complexity) )
                continue;

            for(int i=0; i<number_each_prefab; i++){            
                maxComplexity = Math.Max(maxComplexity, obstacle.complexity);
                InstantiateObstacleAndAddToQueue(obstacle.complexity, obstacle.prefab);
            }

        }

        maxComplexity = Math.Min(levelComplexity.Max(), maxComplexity);
        game_manager.setMaxComplexityValue(maxComplexity);

        // Initiate first level objects ....
        // game_manager.obstacles_array;
        game_manager.rearrange_obstacles_array();
        

        // Player's model, add tag, add to dictionary
        foreach(Pool pl in models)
            models_tag.Add(pl.tag);

        GameObject model = Instantiate(pool.prefab) as GameObject;
        model.SetActive(false);
        SetGameObjectsMaterial(model, false);
        modelsDictionary.Add(pool.tag, model);
    
    }

}

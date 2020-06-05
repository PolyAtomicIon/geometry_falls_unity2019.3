using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine; 
using RandomS=System.Random;
using UnityEngine.SceneManagement; 
using Random=UnityEngine.Random;
using System.Threading;

public static class ThreadSafeRandom
{
    [ThreadStatic] private static RandomS Local;

    public static RandomS ThisThreadsRandom
    {
        get { return Local ?? (Local = new RandomS(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
    }
}

public static class Shuffler
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
        n--;
        int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
        T value = list[k];
        list[k] = list[n];
        list[n] = value;
        }
    }
}

public class ObjectPooler : MonoBehaviour
{

    // player spawning point
    Vector3 VectorZero = Vector3.zero;
    
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

    public int max_complexity_value = 3;

    public int number_each_prefab = 3;

    public List<Pool> models;
     
    public List<string> models_tag;  

    public List<Palette> palettes;
    public Materials materials;

    public Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, GameObject> modelsDictionary;

    private int random_palette = 0;
    private int last_random_index = -1;

    public Manager game_manager;

    public int object_in_level = 5;

    private void StartNewLevel(){
        // here We will spawn random object and its obstacles

        // int random_model_index = game_manager.get_next_random_model_index();
        int random_model_index = 0;

        if ( random_model_index == -1 ){
            game_manager.max_models_number = models_tag.Count;
            game_manager.create_random_models_indexes();
            random_model_index = game_manager.get_next_random_model_index();
        } 
        
        string random_model_tag = models_tag[random_model_index];

        SpawnFromPool(random_model_tag, object_in_level);

        // object has been spawned with it obstacles, done
    }

    public void Start()
    {
        
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("AdditiveScene"));

        game_manager = FindObjectOfType<Manager>();
        object_in_level = (int) game_manager.object_in_level();

        // get prefabs from folder;
        // In build this is not working, LOL

        // for(int i = 0; i < models.Count; i++){
        //     models[i].prefab = ( Resources.Load("Levels/" + models[i].tag + "/" + models[i].tag) ) as GameObject;

        //     for(int j = 1; j <= models[i].size; j++){

        //         string pos = j.ToString();
        //         if( j == 1 ) pos = "";

        //         GameObject temp = ( Resources.Load("Levels/" + models[i].tag + "/" + "obstacle_" + models[i].tag + pos + "_prefab" ) ) as GameObject;

        //         models[i].obstacles_prefab.Add(temp);
        //     }

        // }
        // end
        // actually no need!


        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        modelsDictionary = new Dictionary<string, GameObject>();

        // Add all gameobjects here, then we will shuffle, sorted by obstacle complexity
        List<GameObject>[] temp_gameobjects = new List<GameObject>[max_complexity_value+1];

        // create for every type of prefab 'number_each_prefab' clones
        foreach(Pool pool in models){

            // clean all obstacles
            for (int i = 0; i <= max_complexity_value; i++)
                temp_gameobjects[i] = new List<GameObject>();

            Queue<GameObject> objectPool = new Queue<GameObject>();


            foreach (Obstacle obstacle in pool.obstacles_prefab){
                
                for(int j = 0; j < number_each_prefab; j++){
                    GameObject obstacle_prefab = Instantiate(obstacle.prefab) as GameObject;
                    obstacle_prefab.SetActive(false);
                    // Random
                    temp_gameobjects[obstacle.complexity].Add(obstacle_prefab);
                    // not random
                    // temp_gameobjects[1].Add(obstacle_prefab);
                }

            }

            // Shuffle, add them to Queue
            for(int i=1; i<=max_complexity_value; i++){

                temp_gameobjects[i].Shuffle();
            
                foreach(GameObject prefab in temp_gameobjects[i])
                    objectPool.Enqueue(prefab);
            
            }
            
            poolDictionary.Add(pool.tag, objectPool);
            // end

            // Player's model, add tag, add to dictionary
            models_tag.Add(pool.tag);
            GameObject model = Instantiate(pool.prefab) as GameObject;
            model.SetActive(false);
            modelsDictionary.Add(pool.tag, model);
        }

         StartNewLevel();

    }

    private void initialize_object(GameObject objectToSpawn, Vector3 position, bool model = false){
        
        objectToSpawn.SetActive(false);
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;              
            
        Renderer rd = objectToSpawn.GetComponent<Renderer>();
        
        // if Obstacle
        if( !model ){
            // Random rotation
            objectToSpawn.transform.eulerAngles = new Vector3(-90f, ThreadSafeRandom.ThisThreadsRandom.Next(4) * 90f, 0);
                
            rd.material = materials.materials_list[0];;
        }

        // If Model
        if( model ){  
            rd.material = materials.materials_list[1];
        }

        // Call the OnObjectSpwan, differs for player and obstacle.
        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
        pooledObj.OnObjectSpawn();

    }

    private void set_materials_color(int palette_index){
        // for obstacles
        materials.materials_list[0].SetColor("_BaseColor", palettes[random_palette].colors[0]); 
    
        // for Player, main objects material
        float intensity =  0.8f;
        
        materials.materials_list[1].SetColor("_BaseColor", palettes[random_palette].colors[1]);    
        materials.materials_list[1].EnableKeyword ("_EMISSION");
        materials.materials_list[1].SetColor("_EmissionColor", palettes[random_palette].emission_colors[1] * intensity);
    }

    public void SpawnFromPool (string tag, int size = 0){
        
        Debug.Log(tag + " has been spawned");

        // Get random palettes
        random_palette = game_manager.palette;
        Debug.Log("Palette number");
        Debug.Log(random_palette);
        set_materials_color(random_palette);
        // end

        // Change tunnel color, acces to Manger
        game_manager.change_tunnel_color( palettes[random_palette].colors[0] );
        
        // player's model
        GameObject model = modelsDictionary[tag];
        initialize_object(model, VectorZero, true);        

        // Place obstacles by positions from GAME MANAGER.cs
        for(int i=0; i<size; i++){
            GameObject obstacle = poolDictionary[tag].Dequeue();

            initialize_object(obstacle, game_manager.obstacle_positions[i]);

            poolDictionary[tag].Enqueue(obstacle);
        }    
        
        

    }

    void Update()
    {
        
    }
}

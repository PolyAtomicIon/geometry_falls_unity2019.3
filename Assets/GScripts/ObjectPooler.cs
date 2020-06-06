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

    public int max_complexity_value = 3;

    public int number_each_prefab = 3;

    public List<Pool> models;
     
    public List<string> models_tag;  

    public List<Palette> palettes;
    public Materials materials;

    public Dictionary<string, Queue<GameObject>> poolDictionary;
    public Dictionary<string, GameObject> modelsDictionary;

    private int random_palette = 0;
    private int last_random_index = -1;

    public Manager game_manager;

    public int object_in_level = 5;

    public void Start()
    {
        
        // SceneManager.SetActiveScene(SceneManager.GetSceneByName("AdditiveScene"));

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

    
        game_manager.max_models_number = models_tag.Count;
        game_manager.create_random_models_indexes();

    }

    void Update()
    {
        
    }
}

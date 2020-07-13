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

    public List<Pool> models;
     
    public List<string> models_tag;  

    public List<Palette> palettes;
    public Materials materials;

    // public Dictionary<string, Queue<GameObject>> poolDictionary;
    public Dictionary<string, GameObject> modelsDictionary;

    private int random_palette = 0;
    private int last_random_index = -1;

    public Manager game_manager;

    public int object_in_level = 12;
    public int number_each_prefab = 0;
    
    // place where all obstacles are stored 
    public Queue<GameObject> objectPool = new Queue<GameObject>();

    public void Start()
    {
        
        // SceneManager.SetActiveScene(SceneManager.GetSceneByName("AdditiveScene"));

        game_manager = FindObjectOfType<Manager>();
        
        game_manager.max_models_number = models.Count;
        game_manager.create_random_models_indexes();


        object_in_level = (int) game_manager.object_in_level();
        number_each_prefab = object_in_level / 3;

        int id = PlayerPrefs.GetInt("id");

        if( id == -1 ){
            max_complexity_value = 2;
        }

        // poolDictionary = new Dictionary<string, Queue<GameObject>>();
        modelsDictionary = new Dictionary<string, GameObject>();

        // Add all gameobjects here, then we will shuffle, sorted by obstacle complexity
        List<GameObject>[] temp_gameobjects = new List<GameObject>[10+1];

        // create for every type of prefab 'number_each_prefab' clones
        Pool pool = models[game_manager.get_current_random_model_index()];
        
        // clean all obstacles
        for (int i = 0; i <= 10; i++)
            temp_gameobjects[i] = new List<GameObject>();
        
        // the first type of obstacle will appear 3*number_each_prefab times
        // second one will appear 2*number_each_prefab times
        // other will appear only number_each_prefab times

        int multiplier = 2;

        foreach (Obstacle obstacle in pool.obstacles_prefab){
            
            for(int j = 0; j < number_each_prefab * multiplier; j++){

                if( obstacle.complexity > max_complexity_value )
                    continue;

                GameObject obstacle_prefab = Instantiate(obstacle.prefab) as GameObject;
                obstacle_prefab.SetActive(false);
                // Random
                temp_gameobjects[obstacle.complexity].Add(obstacle_prefab);
            }

            if( multiplier > 1 )
                multiplier -= 1;

        }

        // Shuffle, add them to Queue
        for(int i=1; i<=max_complexity_value; i++){
            foreach(GameObject prefab in temp_gameobjects[i])
                objectPool.Enqueue(prefab);
        }
        
        // create obstacles_array
        for(int i=1; i<=object_in_level; i++){
            GameObject obstacle = objectPool.Dequeue();
            game_manager.obstacles_array.Add(obstacle);
            objectPool.Enqueue(obstacle);
        }

       game_manager.obstacles_array_shuffled.Clear();
        foreach(GameObject obs in game_manager.obstacles_array)
            game_manager.obstacles_array_shuffled.Add(obs);

        game_manager.obstacles_array_shuffled.Shuffle();

        //poolDictionary.Add(pool.tag, objectPool);
        // end

        // Player's model, add tag, add to dictionary
        
        foreach(Pool pl in models)
            models_tag.Add(pl.tag);

        GameObject model = Instantiate(pool.prefab) as GameObject;
        model.SetActive(false);
        modelsDictionary.Add(pool.tag, model);
    
    }

    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine; 
using RandomS=System.Random;
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

static class Shuffler
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

    Vector3 VectorZero = Vector3.zero;

    public float gap = 0f;

    [System.Serializable]
    public class Pool{
        public string tag;
        public GameObject prefab;
        public int size;
        public List<GameObject> obstacles_prefab;
    }

    [System.Serializable]
    public class Palette{
        public Color[] colors = new Color[4];
    }

    #region Singleton

    public static ObjectPooler Instance;

    private void Awake(){
        Instance = this;
    }

    #endregion

    public int number_each_prefab = 10;

    public List<Pool> models;
     
    public List<string> models_tag;  

    public List<Palette> palettes;

    public Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, GameObject> modelsDictionary;

    private int random_palette = 0;
    private int last_random_index = -1;

    void Start()
    {

        // get prefabs from folder;

        for(int i = 0; i < models.Count; i++){
            models[i].prefab = ( Resources.Load("Levels/" + models[i].tag + "/" + models[i].tag) ) as GameObject;

            for(int j = 1; j <= models[i].size; j++){

                string pos = j.ToString();
                if( j == 1 ) pos = "";

                GameObject temp = ( Resources.Load("Levels/" + models[i].tag + "/" + "obstacle_" + models[i].tag + pos + "_prefab" ) ) as GameObject;

                models[i].obstacles_prefab.Add(temp);
            }

        }
        // end


        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        modelsDictionary = new Dictionary<string, GameObject>();

        // Add all gameobjects here, then we will shuffle
        List<GameObject> temp_gameobjects = new List<GameObject>();


        // create for every type of prefab 'number_each_prefab' clones
        foreach(Pool pool in models){
            Queue<GameObject> objectPool = new Queue<GameObject>();


            foreach (GameObject obstacle_prefab in pool.obstacles_prefab){
                
                for(int j = 0; j < number_each_prefab; j++){
                    GameObject obstacle = Instantiate(obstacle_prefab) as GameObject;
                    obstacle.SetActive(false);
                    temp_gameobjects.Add(obstacle);
                }

            }

            // Shuffle, add them to Queue
            temp_gameobjects.Shuffle();
            foreach(GameObject i in temp_gameobjects){
                objectPool.Enqueue(i);
            }
            poolDictionary.Add(pool.tag, objectPool);
            // end

            // Player's model, add tag, add to dictionary
            models_tag.Add(pool.tag);
            GameObject model = Instantiate(pool.prefab) as GameObject;
            modelsDictionary.Add(pool.tag, model);
        }

    }

    private void initialize_object(GameObject objectToSpawn, string tag, Vector3 position, bool model = false){
        
        objectToSpawn.SetActive(false);
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;              

        // set the color, get one random from 3 colors of palette
        RandomS rand = new RandomS();
        
        int random_color_index = last_random_index;
        
        while( random_color_index == last_random_index )
            random_color_index = ThreadSafeRandom.ThisThreadsRandom.Next(3);

        last_random_index = random_color_index;

        Debug.Log(random_color_index);

        Renderer rd = objectToSpawn.GetComponent<Renderer>();
        rd.material = new Material(Shader.Find("Specular"));
        rd.material.SetColor("_Color", palettes[random_palette].colors[random_color_index]);
        //


        // set rotation, for player - quaternion, for obstacle x = -90, then random;
        if( model ){
            objectToSpawn.transform.rotation = Quaternion.identity;
            rd.material.SetColor("_Color", palettes[random_palette].colors[3]);
        }
        else{    
            objectToSpawn.transform.eulerAngles = new Vector3(270f, Random.Range(0f, 360f), Random.Range(0f, 360f));
        }


        // Call the OnObjectSpwan, differs for player and obstacle.
        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
        pooledObj.OnObjectSpawn();
        
    }

    public void SpawnFromPool (string tag, int size = 0){
        
        Debug.Log(tag + " has been spawned");

        // Get random palettes, 3 colours
        RandomS rand = new RandomS();
        random_palette = rand.Next(0, palettes.Count);
        // end


        // player's model
        GameObject model = modelsDictionary[tag];
        initialize_object(model, tag, VectorZero, true);        


        // generate the level with obstacles
        Vector3 gap_between = new Vector3(0f, gap, 0f);
        Vector3 obstacle_position = new Vector3(0f, -20f, 0f);

        for(int i=0; i<size; i++){

            GameObject obstacle = poolDictionary[tag].Dequeue();

            initialize_object(obstacle, tag + "_obstacle", obstacle_position);

            poolDictionary[tag].Enqueue(obstacle);

            obstacle_position += gap_between;

        }    
        
        

    }

    void Update()
    {
        
    }
}

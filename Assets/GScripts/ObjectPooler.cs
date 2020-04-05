using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{

    [System.Serializable]
    public class Pool{
        public string tag;
        public GameObject prefab;
        public List<GameObject> obstacles_prefab;
    }

    #region Singleton

    public static ObjectPooler Instance;

    private void Awake(){
        Instance = this;
    }

    #endregion

    public int number_each_prefab = 10;

    public List<Pool> models;
     
    private List<string> models_tag;  

    public Dictionary<string, Queue<GameObject>> poolDictionary;
    private Dictionary<string, GameObject> modelsDictionary;

    void Start()
    {

        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        modelsDictionary = new Dictionary<string, GameObject>();

        foreach(Pool pool in pools){
            Queue<GameObject> objectPool = new Queue<GameObject>();

            foreach (GameObject obstacle_prefab in pool.obstacles_prefab){
                
                for(int j = 0; j < number_each_prefab; j++){
                    GameObject obstacle = Instantiate(obstacle_prefab) as GameObject;
                    obstacle.SetActive(false);
                    objectPool.Enqueue(obstacle);
                }

            }

            models_tag.Add(pool.tag);
            poolDictionary.Add(pool.tag, objectPool);

            GameObject model = Instantiate(pool.prefab) as GameObject;
            modelsDictionary.Add(pool.tag, model);
        }

    }

    private void initialize_object(GameObject objectToSpawn, string tag, Vector3 position = new Vector3(0f, 0f, 0f)){
        
        objectToSpawn.SetActive(false);
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;              
        objectToSpawn.transform.rotation = Random.rotation; 
        objectToSpawn.transform.rotation.x = -90f;

        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();

        try {
            pooledObj.OnObjectSpawn();
            Debug.Log(tag + " Object has been spawned ");
        } catch (NullReferenceException e) {
            Debug.Log("Exception caught: object is null - " + tag);
        }
        
    }

    public void SpawnFromPool (string tag, int size = 0){
        
        Debug.Log(tag + " has been spawned");

        GameObject model = modelsDictionary[tag];
        initialize_object(model, tag);        

        float gap_between = new Vector3(0f, -5f, 0f);
        Vector3 obstacle_position = new Vector3(0f, 0f, 0f);

        for(int i=0; i<size; i++){

            obstacle_position += gap_between;

            GameObject obstacle = poolDictionary[tag].Dequeue();

            initialize_object(obstacle, tag + "_obstacle", obstacle_position);

            poolDictionary[tag].Enqueue(obstacle);
        }    
        
        

    }

    void Update()
    {
        
    }
}

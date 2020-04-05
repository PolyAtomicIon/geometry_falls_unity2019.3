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
        public int scale;
    }

    #region Singleton

    public static ObjectPooler Instance;

    private void Awake(){
        Instance = this;
    }

    #endregion

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    void Start()
    {

        // poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // foreach(Pool pool in pools){
        //     Queue<GameObject> objectPool = new Queue<GameObject>();

        //     for (int i = 0; i < pool.size; i++ ){

        //         GameObject obj = Instantiate(pool.prefab) as GameObject;
        //         obj.SetActive(false);

        //         objectPool.Enqueue(obj);

        //     }

        //     poolDictionary.Add(pool.tag, objectPool);
        // }

    }

    public GameObject SpawnFromPool (string tag, Vector3 position){
        
        Debug.Log(tag + " what is it ?");

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(false);
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;              
        objectToSpawn.transform.rotation = Quaternion.identity; 

        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
        
        try {
            pooledObj.OnObjectSpawn();
            Debug.Log(tag + " Object has been spawned ");
        } catch (NullReferenceException e) {
            Debug.Log("Exception caught: object is null" + tag);
        } 
        
        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;

    }

    void Update()
    {
        
    }
}

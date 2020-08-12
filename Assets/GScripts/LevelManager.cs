using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine; 
using RandomS=System.Random;
using UnityEngine.SceneManagement; 
using Random=UnityEngine.Random;
using System.Threading;

public class LevelManager : MonoBehaviour
{

    private int random_palette = 0;

    public Manager game_manager;
    ObjectPooler objectPooler;

     private void StartNewLevel(){
        // here We will spawn random object and its obstacles

        // int random_model_index = game_manager.get_next_random_model_index();
        int random_model_index = 0;

        random_model_index = game_manager.get_current_random_model_index();
        
        string random_model_tag = objectPooler.models_tag[random_model_index];

        game_manager.is_level_started = true;
        SpawnFromPool(random_model_tag, objectPooler.object_in_level);

        // object has been spawned with it obstacles, done
    }

    public void Start()
    {
        
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("AdditiveScene"));

        game_manager = FindObjectOfType<Manager>();
        objectPooler = ObjectPooler.Instance;

        StartNewLevel();
        
    }

    private void initialize_object(GameObject objectToSpawn, Vector3 position, bool model = false){
        
        objectToSpawn.SetActive(false);
        objectToSpawn.SetActive(true);

        // // because of transition between scenes
        // if( model )
        //     position.y -= 30f;

        objectToSpawn.transform.position = position;              
            
        Renderer rd = objectToSpawn.GetComponent<Renderer>();
        
        // if Obstacle
        if( !model ){
            // Random rotation
            objectToSpawn.transform.eulerAngles = new Vector3(-90f, ThreadSafeRandom.ThisThreadsRandom.Next(4) * 90f, 0);
                
            rd.material = objectPooler.materials.materials_list[0];;
        }

        // If Model
        if( model ){  
            rd.material = objectPooler.materials.materials_list[1];
            
            objectToSpawn.transform.eulerAngles = new Vector3(0f, 0f, 0);
        }

        // Call the OnObjectSpwan, differs for player and obstacle.
        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
        pooledObj.OnObjectSpawn();

    }

    private void set_materials_color(int palette_index){
        // for obstacles
        objectPooler.materials.materials_list[0].SetColor("_BaseColor", objectPooler.palettes[random_palette].colors[0]); 
    
        // for Player, main objects material
        float intensity = 0.01f;
        
        objectPooler.materials.materials_list[1].SetColor("_BaseColor", objectPooler.palettes[random_palette].colors[1]);    
        objectPooler.materials.materials_list[1].EnableKeyword ("_EMISSION");
        objectPooler.materials.materials_list[1].SetColor("_EmissionColor", objectPooler.palettes[random_palette].emission_colors[1] * intensity);
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
        game_manager.change_tunnel_color( objectPooler.palettes[random_palette].colors[0] );
        
        // player's model
        GameObject model = objectPooler.modelsDictionary[tag];
        initialize_object(model, objectPooler.VectorZero, true);        

        // Place obstacles by positions from GAME MANAGER.cs

        for(int i = 0; i < size; i++)
            initialize_object(game_manager.obstacles_array_shuffled[i], game_manager.obstacle_positions[i]);
        
        // game_manager.progressionColor.material = objectPooler.materials.materials_list[0];

    }

    void Update()
    {
        
    }
}

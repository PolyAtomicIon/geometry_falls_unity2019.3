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

        random_model_index = game_manager.getCurrentModelIndex();
        
        string random_model_tag = objectPooler.models_tag[random_model_index];

        game_manager.is_level_active = true;
        SpawnFromPool(random_model_tag, game_manager.object_in_level());

        // object has been spawned with it obstacles, done
    }

    public void Start()
    {
        
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("AdditiveScene"));

        game_manager = FindObjectOfType<Manager>();
        objectPooler = ObjectPooler.Instance;

        StartNewLevel();
        
    }

    private void set_materials_color(int palette_index){
        
        // for obstacles

        Material obstacleMaterial = objectPooler.materials.getObstacleMaterial();
        Color obstacleColor = objectPooler.palettes[random_palette].getObstacleColor();
        obstacleColor.a = 1f;

        obstacleMaterial.SetColor("_BaseColor", obstacleColor); 

    
        // for Player, main objects material
        float intensity = 0.004f;
        
        Material modelMaterial = objectPooler.materials.getModelMaterial();
        Color modelColor = objectPooler.palettes[random_palette].getModelColor();

        // modelMaterial.SetColor("_BaseColor", modelColor);
        // modelMaterial.EnableKeyword ("_EMISSION");
        // modelMaterial.SetColor("_EmissionColor", modelColor * intensity);

       StartCoroutine( Manager.lerpColorMaterial(modelMaterial, modelMaterial.color, modelColor, 1f) );
    }

    public void SpawnFromPool (string tag, int size = 0){
        
        Debug.Log(tag + " has been spawned");

        // Get random palettes
        random_palette = game_manager.palette;
        Debug.Log("Palette number");
        Debug.Log(random_palette);
        set_materials_color(random_palette);
        // end
        
        // player's model
        GameObject model = objectPooler.modelsDictionary[tag];
        game_manager.initialize_object(model, objectPooler.VectorZero, true, 0);        

        for(int i = 0; i < 2; i++){
            game_manager.nextObstacleToInitialize();
        }

        // game_manager.progressionColor.material = objectPooler.materials.materials_list[0];

    }

    void Update()
    {
    }
}

using UnityEngine;
using System;   
using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;

public class VFXs : MonoBehaviour
{

    public ParticleSystem success;
    private ParticleSystem[] success_childs;
    public ParticleSystem fail;
    public Manager manager;

    int cur_level = 0;

    void Start(){
        manager = FindObjectOfType<Manager>();
        success_childs = success.GetComponentsInChildren<ParticleSystem>();
    }

    void Update() {
        if( cur_level != manager.getCurLevel() ){
            
            var success_var = success.main;
            success_var.startColor = manager.getObstaclesMaterialColor();
            foreach(ParticleSystem child in success_childs ){
                var main = child.main;
                main.startColor = manager.getObstaclesMaterialColor();
            }
            // fail.main.startColor = manager.getModelsMaterialColor();

            cur_level = manager.getCurLevel();
        }
    }

}

using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine;
using System;   

public class Player : MonoBehaviour, IPooledObject
{

    public Rigidbody rb;
    private Transform transform;
    public Renderer render;
    private Manager game_manager;
    CameraScript cameraScript;

    // Components for colorFlashEffect
    private Color init_color; 
    private Material objectMaterial; 
    private float FlashEffectDuration = 0.45f;
    private float HitEffectDuration = 1.2f;

    // Default physics variables
    private float fall_down_speed = -27f;
    private float acceleration = -0.035f;
    public float angular_drag = 0.45f;
   
    // Default values for Rotation Animation 
    private float rotation_duration = 0.2f;
    private float rotation_degree = 120f;
    // end

    // Level variables    
    public bool is_game_over = false;
    private int score = 0;
    
    private void changeObjectColor(Color from, Color to, float duration){
        StartCoroutine( Manager.lerpColorMaterial(objectMaterial, from, to, duration) );
    }

    private void colorFlashEffect(Color toColor){
        changeObjectColor(init_color, toColor, FlashEffectDuration);
        changeObjectColor(toColor, init_color, FlashEffectDuration);
    }

    private void StopObjectAndStartGameOver(){

        changeObjectColor(init_color, Color.black, HitEffectDuration);

        setRigidBodyVelocity(0);
        rb.Sleep();
        
        is_game_over = true;
        game_manager.is_level_active = false;
        
        game_manager.game_over();
    }

    public void increment_score(){
        score += 1;
        if( score < game_manager.object_in_level() )
            colorFlashEffect(Color.red);
        else    
            changeObjectColor(init_color, Color.black, FlashEffectDuration);

    }

    public float get_position_y_axis(){
        return transform.position.y;
    }

    public float get_velocity_y_axis(){
        return rb.velocity.y;
    }

    private void setGravityValue(){
        Physics.gravity = new Vector3(0, acceleration, 0);  
    }

    private void setRigidBodyVelocity(float speed){
        rb.velocity = new Vector3(0, speed, 0);
    }

    private void setRigidBodyInitialParameters(){
        rb.centerOfMass = Vector3.zero;
        rb.inertiaTensorRotation = Quaternion.identity;

        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

        rb.angularDrag = angular_drag;
    }
    
    private IEnumerator SetRotation(Vector3 newRotation, float duration = 1.0f)
   {
       
        Quaternion from = transform.rotation;
        
        Transform target = transform;
        target.eulerAngles = newRotation;
        Quaternion to = target.rotation;

        float elapsed = 0.0f;
        while( elapsed < duration )
        {
        transform.rotation = Quaternion.Slerp(from, to, elapsed / duration );
        elapsed += Time.deltaTime;
        yield return null;
        }
        transform.rotation = to;
   }

    public void Turn(int direction){

        if( is_game_over ) return;

        // direction:
            // -1 -> left -> nextDegree -> -90
            // 0 -> initial -> nextDegree -> 0
            // 1 -> right -> nextDegree -> 90
        
        float nextDegree = direction * rotation_degree;

        Vector3 newRotation = new Vector3(0f, nextDegree, 0f);  
        StartCoroutine( SetRotation( newRotation, rotation_duration ) );   

    }

    void Start(){       
        
        if( rb == null )
            rb = GetComponent<Rigidbody>();

        if( render == null )
            render = GetComponent<Renderer>();
            
        if( transform == null )
            transform = GetComponent<Transform>();

        if( cameraScript == null )
            cameraScript = FindObjectOfType<CameraScript>();

        if( game_manager == null )
            game_manager = FindObjectOfType<Manager>();

        game_manager.player = this;
    }

    public void OnObjectSpawn(){
        
        Start();

        setRigidBodyInitialParameters();
        setGravityValue();  

        score = 0;

        fall_down_speed = game_manager.fall_down_speed;
        setRigidBodyVelocity(fall_down_speed);

        init_color = game_manager.getModelsandTunnelsMaterialColor();
        objectMaterial = game_manager.getModelsandTunnelsMaterial();
    }

    void Update(){

        if( is_game_over ){    
            setRigidBodyVelocity(0f);
        } 

    }
    
     void OnCollisionEnter (Collision col)
    {
        Debug.Log(col.gameObject.name);    

        StopObjectAndStartGameOver();
    }

}

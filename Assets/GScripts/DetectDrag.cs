using UnityEngine;
 using System.Collections;
 
 public class DetectDrag : MonoBehaviour 
 {

    public Player player;


    void Start(){
        player = FindObjectOfType<Player>();
    }

    void Update(){
        Vector3 ps = player.transform.position;
        ps.z -= 0.5f;
        transform.position = ps;
    }

 }
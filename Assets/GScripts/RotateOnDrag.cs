using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine;
using System;   

public class RotateOnDrag : MonoBehaviour
{

    void Update(){

        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        float midPoint = (transform.position - Camera.main.transform.position).magnitude * -0.5f;
        transform.LookAt(mouseRay.origin + mouseRay.direction * midPoint);

    }

}
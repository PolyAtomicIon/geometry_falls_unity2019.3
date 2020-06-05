using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour {

    public int windowNumber = 0;

    public void CloseWindow(){
        Debug.Log("Close window");
    }

    public void GoBack(){
        Debug.Log("Go Back");   
    }

}

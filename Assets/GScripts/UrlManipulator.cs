using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrlManipulator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void openUrl(string url = "https://www.instagram.com/?hl=ru"){
        url = "https://www.instagram.com/?hl=ru";
        Debug.Log(url);
        Application.OpenURL(url);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

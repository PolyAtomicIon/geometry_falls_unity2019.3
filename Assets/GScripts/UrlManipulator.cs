using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrlManipulator : MonoBehaviour
{
    public static void openUrl(string url){
        Debug.Log(url + " SOMETHING URL");
        Application.OpenURL(url);
    }

}

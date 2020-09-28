using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;
using SimpleJSON;
using UnityEngine.UI;

public class Rules : MonoBehaviour {

    public TMP_Text rules_text;

    IEnumerator set_rules_text()
    {
        string url = "http://94.247.128.162/api/core/agreement/";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {   
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("Rules" + www.error);
            }
            else
            {   
                Debug.Log("Request sent! Agreement");
                JSONNode details = JSONNode.Parse(www.downloadHandler.text);
            
                Debug.Log( details["agreement"] );

                rules_text.text = details["agreement"];
            }
        }

    }

    void Start(){
        StartCoroutine(set_rules_text());
    }

}

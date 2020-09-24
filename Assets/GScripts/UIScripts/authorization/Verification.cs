using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;
using TMPro;
using UnityEngine.EventSystems;

public class Verification : MonoBehaviour {

    public authorization auth_manager;
    public MainPage manager;

    public int cur_verification_input_field = 0;
    public List<TMP_InputField> verification_code_fields = new List<TMP_InputField>();
    // public TMP_InputField six_ch_verification_code_field;
    
    string url = "http://94.247.128.162/api/core/email-verify/";

    public GameObject errorField;
    
    IEnumerator Verify_email(string code = "")
    {
        WWWForm form = new WWWForm();
        form.AddField("code", code);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            
            www.SetRequestHeader("Authorization", manager.get_token());
            yield return www.SendWebRequest();

            Debug.Log(www);

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                errorField.SetActive(true);

                auth_manager.Verify();
            }
            else
            {
                Debug.Log("Email verified");
                errorField.SetActive(false);

                manager.Reload();
            }
        }
    }

     public void Get_verification_code(){

        string verification_code = "";

        // 4 integers
        for(int i=0; i<4; i++){
            verification_code += verification_code_fields[i].text;
        }

        Debug.Log(verification_code);

        StartCoroutine( Verify_email(verification_code) );
    }

}


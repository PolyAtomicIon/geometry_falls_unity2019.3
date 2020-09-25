using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;
using TMPro;
using UnityEngine.EventSystems;

public class Login : MonoBehaviour {

    public authorization auth_manager;
    public MainPage manager;
    
    // Login Variables
    public TMP_InputField login_email_field;
    public TMP_InputField login_password_field;
    string url = "http://94.247.128.162/api/core/obtain-token/";
    
    public GameObject errorField;

    void Get_Token(UnityWebRequest res){
        JSONNode token_info = JSONNode.Parse(res.downloadHandler.text);
        string token = token_info["token"];

        
        manager.set_token("Token " + token);
        Debug.Log( manager.get_token() );

        //check is verified
        auth_manager.User_Profile();
    }

    public IEnumerator Login_query(string email = "", string password = "")
    {
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("Login:" + www.error);
                errorField.SetActive(true);
            }
            else
            {
                Debug.Log("Logged in");
                errorField.SetActive(false);
                Get_Token(www);
            }
        }
    }

    public void Login_get()
    {
        string email = login_email_field.text;
        string password = login_password_field.text;

        StartCoroutine( Login_query(email, password) );
    }

}


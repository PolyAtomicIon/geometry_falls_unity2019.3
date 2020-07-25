using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;
using TMPro;

public class authorization : MonoBehaviour {

    public MainPage manager;

    // Signup Variables
    public TMP_InputField signup_email_field;
    public TMP_InputField signup_phone_field;
    public TMP_InputField signup_password_field;
    string base_register_url = "http://94.247.128.162/api/core/register/";

    
    // Login Variables
    public TMP_InputField login_email_field;
    public TMP_InputField login_password_field;
    string base_login_url = "http://94.247.128.162/api/core/obtain-token/";


    IEnumerator SignUp(string email = "", string phone = null, string password = "")
    {
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);
        form.AddField("phone", phone);

        using (UnityWebRequest www = UnityWebRequest.Post(base_register_url, form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                manager.windows[6].SetActive(true);
            }
            else
            {
                Debug.Log("Form upload complete!");
                manager.windows[6].SetActive(false);
                
                // show Login UI
                // manager.showWindow(5, 4);
                manager.showWindow(5);
            }
        }
    }

    public void Registration()
    {

        string email = signup_email_field.text;
        string phone = signup_phone_field.text;
        string password = signup_password_field.text;

        Debug.Log(email);
        Debug.Log(phone);
        Debug.Log(password);

        StartCoroutine( SignUp(email, phone, password) );

    }

    void Get_Token(UnityWebRequest res){
        JSONNode token_info = JSONNode.Parse(res.downloadHandler.text);
        string token = token_info["token"];
        Debug.Log("TOKEN - DELETE THIS LINE");
        Debug.Log(token);
        manager.set_token("Token " + token);
                
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator Login(string email = "", string password = "")
    {
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);

        using (UnityWebRequest www = UnityWebRequest.Post(base_login_url, form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                manager.windows[6].SetActive(true);
            }
            else
            {
                Debug.Log("Form upload complete!");
                manager.windows[6].SetActive(false);

                Get_Token(www);
                // show Menu
                // manager.showWindow(0, 5);
                // manager.showWindow(0);
            }
        }
    }

    public void Login_get()
    {

        string email = login_email_field.text;
        string password = login_password_field.text;

        Debug.Log(email);
        Debug.Log(password);

        StartCoroutine( Login(email, password) );


    }

}


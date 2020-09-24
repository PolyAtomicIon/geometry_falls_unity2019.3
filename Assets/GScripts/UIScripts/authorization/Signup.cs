using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;
using TMPro;
using UnityEngine.EventSystems;

public class Signup : MonoBehaviour {

    public authorization auth_manager;

    // Signup Variables
    public TMP_InputField signup_email_field;
    public TMP_InputField signup_phone_field;
    public TMP_InputField signup_password_field;
    string url = "http://94.247.128.162/api/core/register/";
    
    public GameObject errorField;

    IEnumerator SignUp(string email = "", string phone = null, string password = "")
    {
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);
        form.AddField("phone", phone);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                errorField.SetActive(true);
            }
            else
            {
                Debug.Log("Form upload complete!");
                errorField.SetActive(false);

                // Obtain token
                auth_manager.Login(email, password);

                //send verficiation code to user's email
                auth_manager.Verify();

            }
        }
    }

    public void Signup_get()
    {
        string email = signup_email_field.text;
        string phone = signup_phone_field.text;
        string password = signup_password_field.text;

        StartCoroutine( SignUp(email, phone, password) );
    }


}


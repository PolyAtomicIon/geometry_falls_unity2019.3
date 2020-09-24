using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;
using TMPro;
using UnityEngine.EventSystems;

public class authorization : MonoBehaviour {

    public Login _Login;
    public Signup _Signup;
    public Verification _Verification;

    public MainPage manager;

    string send_verification_code_url = "http://94.247.128.162/api/core/email-verify/";

    string user_profile_url = "http://94.247.128.162/api/core/profile/";

    public void User_Profile(){
        StartCoroutine( Get_User_Profile() );
    }

    IEnumerator Get_User_Profile(){
        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Get(user_profile_url))
        {
            www.SetRequestHeader("Authorization", manager.get_token());
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("Profile:" + www.error);
            }
            else
            {
                JSONNode res = JSONNode.Parse(www.downloadHandler.text);
        
                Debug.Log("User profile");
                Debug.Log(res);

                bool email_verified = res["email_verified"];

                if( email_verified )
                    manager.Reload();
                else{
                    Verify();
                }
            }
        }
    }

    public void Verify(){
        manager.showWindow(7);
        StartCoroutine( Send_verification_code_ie() );
    }

    IEnumerator Send_verification_code_ie()
    {

        using (UnityWebRequest www = UnityWebRequest.Get(send_verification_code_url))
        {
            
            www.SetRequestHeader("Authorization", manager.get_token());
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                // manager.show_error("Проблемы с отправкой кода");
            }
            else
            {
                Debug.Log("Verificatoin code is sent");
                // manager.windows[4].SetActive(false);
            }
        }
    }

    public void Login(string email, string password)
    {
        StartCoroutine( _Login.Login_query(email, password) );
    }

    public void Login()
    {
        _Login.Login_get();
    }

    public void Signup(){
        _Signup.Signup_get();
    }

    public void StartVerification(){
        _Verification.Get_verification_code();
    }

}


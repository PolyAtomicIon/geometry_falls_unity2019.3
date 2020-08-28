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

    public int cur_verification_input_field = 0;
    public List<TMP_InputField> verification_code_fields = new List<TMP_InputField>();
    public TMP_InputField six_ch_verification_code_field;
    
    string send_verification_code_url = "http://94.247.128.162/api/core/email-verify/";
    // string check_verification_code_url = "http://94.247.128.162/api/core/email-confirm/";

    string user_profile_url = "http://94.247.128.162/api/core/profile/";
    
    IEnumerator Verify_email(string code = "")
    {
        WWWForm form = new WWWForm();
        form.AddField("code", code);

        using (UnityWebRequest www = UnityWebRequest.Post(send_verification_code_url, form))
        {
            
            www.SetRequestHeader("Authorization", manager.get_token());
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                manager.show_error("Неправильный код, попробуйте еще раз");
                StartCoroutine(Send_verification_code_ie());
            }
            else
            {
                Debug.Log("Email verified");
                manager.windows[6].SetActive(false);

                SceneManager.LoadScene("MainMenu");

            }
        }
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
                
                manager.show_error("Проблемы с отправкой кода");
            }
            else
            {
                Debug.Log("Verificatoin code is sent");
                manager.windows[6].SetActive(false);
                
                // show Verification UI
                // manager.showWindow(5, 4);
                // manager.showWindow(10);
            }
        }
    }

    public void Resend_Verification_Code(){
        StartCoroutine( Send_verification_code_ie() );
    }

     public void Get_verification_code(){

        string verification_code = "";
        // 4 integers
        // for(int i=0; i<4; i++){
        //     verification_code += verification_code_fields[i].text;
        // }

        // 6 characters
        verification_code = six_ch_verification_code_field.text;

        Debug.Log(verification_code);

        StartCoroutine( Verify_email(verification_code) );
    }

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
                manager.show_error("Проблемы с соединением или данная почта уже используется");
            }
            else
            {
                Debug.Log("Form upload complete!");
                manager.windows[6].SetActive(false);
                
                //send verficiation code to user's email
                
                // Obtain token
                StartCoroutine( Login(email, password) );
                
                // manager.showWindow(10);
                // send verification code
                StartCoroutine( Send_verification_code_ie() );

                // manager.showWindow(5);

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

    IEnumerator Get_User_Profile(){
        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Get(user_profile_url))
        {
            www.SetRequestHeader("Authorization", manager.get_token());
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                
                manager.show_error("Проблемы с соединением или введены неправильные данные");
            }
            else
            {
                manager.windows[6].SetActive(false);

                JSONNode res = JSONNode.Parse(www.downloadHandler.text);
        
                Debug.Log("User profile");
                Debug.Log(res);

                bool email_verified = res["email_verified"];

                if( email_verified )
                    SceneManager.LoadScene("MainMenu");
                else{
                    StartCoroutine( Send_verification_code_ie() );
                    manager.showWindow(10);
                }
            }
        }
    }

    void Get_Token(UnityWebRequest res){
        JSONNode token_info = JSONNode.Parse(res.downloadHandler.text);
        string token = token_info["token"];
        Debug.Log("TOKEN - DELETE THIS LINE");
        Debug.Log(token);
        manager.set_token("Token " + token);
                
        StartCoroutine( Get_User_Profile() );

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
                
                manager.show_error("Проблемы с соединением или введены неправильные данные");
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

    void Update(){

        // cur_verification_input_field = -1;
        // for(int i=0; i<4; i++){
        //     if( verification_code_fields[i].isActiveAndEnabled ){
        //         cur_verification_input_field = i;
        //         break;
        //     }
        // }

    }

}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;
using SimpleJSON;
using UnityEngine.UI;

public class Profile : MonoBehaviour {

    public MainPage manager;
    public TMP_Text nickname_mainPage;
    public TMP_Text nickname;
    public TMP_Text total_coupons;
    public TMP_Text record;
    public TMP_Text ranking;

    public GameObject logout_confirmation_panel;
    public GameObject verification_reminder;
    
    string user_profile_url = "http://94.247.128.162/api/core/profile/";
   
    public void Logout_confirmatoin_open(){
        logout_confirmation_panel.SetActive(true);
    }
    public void Logout_confirmatoin_close(){
        logout_confirmation_panel.SetActive(false);
    }
    public void Logout(){
        manager.set_token("");
        manager.Reload();
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

                SetProfilePage(res);
            }
        }
    }

    void SetProfilePage(JSONNode user_data){

        /*
        {"email":"t1@gmail.com","phone":null,"email_verified":true,"high_score":null,"presents_count":2,"rank":null}
        */

        SetUserVerification(user_data["email_verified"]);

        nickname.text = user_data["email"];
        nickname_mainPage.text = user_data["email"];

        total_coupons.text = user_data["presents_count"];
        if( user_data["high_score"] )
            record.text = user_data["high_score"] + "уровень";
        if( user_data["rank"] )
            ranking.text = "#" + user_data["rank"];

    }

    void SetUserVerification(bool verified){
        if( verified ){
            PlayerPrefs.SetInt("user_verified", 1);
        }
        else{
            PlayerPrefs.SetInt("user_verified", 0);
            verification_reminder.SetActive(true);
        }
    }

    public void GetUserProfile(){
        StartCoroutine( Get_User_Profile() );
    }

    void Start(){

        if( manager.get_token() != "" && manager.get_token() != null ){
            GetUserProfile();
        }

    }

}

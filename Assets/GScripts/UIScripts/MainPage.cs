using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using TMPro;
using SimpleJSON;
using UnityEngine.UI;

public class MainPage : MonoBehaviour {

    public webRequestController controller;
    public authorization auth_manager;

    public Profile profile;

    // public List<GameObject> windows;
    public List<Canvas> windows;
    public int currentWindowIndex = 0;

    public Button EventsButton;

    public Image AudioBackgroundColor; 
    public Sprite audio_on_icon; 
    public Sprite audio_off_icon; 

    public AudioSource BackgroundMusic;
    
    public List<AudioClip> audios;

    public Animator SplashScreenAnimator;

    private int levels = 0;


    public TMP_Text rules_text;

    string user_profile_url = "http://94.247.128.162/api/core/profile/";
    // show window
    // 0 - MainPage
    // 1 - Events
    // 2 - Coupons
    // 3 - Profile
    // 4 - Sign Up
    // 5 - Login
    // 6 - Instructions
    // 7 - verification
    // 8 - Rules
    public void showWindow (int windowIndex = 0){
        windows[ currentWindowIndex ].enabled = false;

        string token = PlayerPrefs.GetString("auth_token");

        // Trying access Events and Profile pages unauthorized
        if( (windowIndex == 1 || windowIndex == 3) && (token == null || token == "") ){
            windows[ 5 ].enabled = true;
            currentWindowIndex = 5;
        }
        else{
            windows[ windowIndex ].enabled = true;

            currentWindowIndex = windowIndex;
        }

    }

    public void Verify(){
        auth_manager.Verify();
    }

    public void set_token(string a){
        PlayerPrefs.SetString("auth_token", a);
    }

    public string get_token(){
        if( !PlayerPrefs.HasKey("auth_token") )
            return "";
        return PlayerPrefs.GetString("auth_token");
    }

    public void Reload(){
        SceneManager.LoadScene("newMainScene");
    }

    public void LoadGame(){
        if( !PlayerPrefs.HasKey("Played practice") )
            PlayerPrefs.SetInt("Played practice", 1);       
        SceneManager.LoadScene("Base");
    }

    public void StartPractice (){
        
        // Player can not go to events, without playing practice at least once
        if( !PlayerPrefs.HasKey("Played practice") ){
            PlayerPrefs.SetInt("Played practice", 1); 
        }

        // EventsButton.enabled = true;

        // set id to -1, to say it is practice
        PlayerPrefs.SetInt("id", -1);
        PlayerPrefs.SetInt("levels", 1000);
        LoadGame();
        
    }

    public void StartEvent(int id){
        // StartCoroutine(TEMPGIVEPRIZE(id));
        // send id of event to player prefs
        PlayerPrefs.SetInt("id", id);
        // load game
        LoadGame();
    }

    public void SetAudio(){

        // BackgroundMusic.enabled = ( BackgroundMusic.enabled ^ true );
        if( BackgroundMusic.volume == 0.6f )
            BackgroundMusic.volume = 0.0f;
        else
            BackgroundMusic.volume = 0.6f;

        if( BackgroundMusic.volume == 0.6f ){
            PlayerPrefs.SetInt("isAudio", 1);
            AudioBackgroundColor.sprite = audio_on_icon;
        }
        else{
            PlayerPrefs.SetInt("isAudio", 0);
            AudioBackgroundColor.sprite = audio_off_icon;            
        }
    }
    IEnumerator set_rules_text()
    {
        string url = "http://94.247.128.162/api/core/agreement/";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {   
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log("Rules" + www.error);
                // windows[6].SetActive(true);
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

    public void LoggedIn(){
        controller.GetData();
        profile.GetUserProfile();
    }

    void Start(){

        showWindow(0);

        // PlayerPrefs.DeleteKey("Played practice"); 

        if( !PlayerPrefs.HasKey("Played practice") ){
            EventsButton.interactable = false;         
        }

        if( get_token() != "" ){
            controller.GetData();
        }

        StartCoroutine( set_rules_text() );

        // Syncing  Data or Loading Animation, no matter what is written as an argument
        int id = ThreadSafeRandom.ThisThreadsRandom.Next(audios.Count);
        PlayerPrefs.SetInt("bgAudioID", id);

        BackgroundMusic.clip = audios[id];

        BackgroundMusic.Play();
        
        // BG Music 
        if( !PlayerPrefs.HasKey("isAudio") )
            PlayerPrefs.SetInt("isAudio", 1);
        else if( PlayerPrefs.GetInt("isAudio") == 0 ){
            BackgroundMusic.volume = 0.0f;
            AudioBackgroundColor.sprite = audio_off_icon;
        }

        // SplashScreenAnimator.Play("Login to Loading");
    }

}

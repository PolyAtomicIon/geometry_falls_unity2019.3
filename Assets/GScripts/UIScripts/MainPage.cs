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
    // 9 - Transition
    public void showWindow (int windowIndex = 0){
        windows[ currentWindowIndex ].enabled = false;

        string token = get_token();

        // Trying access Events and Profile pages unauthorized
        if( (windowIndex == 1 || windowIndex == 3) && (token == null || token == "") ){
            windows[ 5 ].enabled = true;
            currentWindowIndex = 5;
        }
        else{

            if( windowIndex == 1 && !controller.events_initialized ){
                controller.GetEventsData();
            }
            else if( windowIndex == 2 && !controller.coupons_initialized ){
                controller.GetCouponsData();
            }

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
        SplashScreenAnimator.Play("Login to Loading");
        showWindow(9);
        
        if( !PlayerPrefs.HasKey("Played practice") )
            PlayerPrefs.SetInt("Played practice", 1); 

        // SceneManager.LoadScene("Base");
        StartCoroutine(LoadYourAsyncScene("Base"));
    }

    IEnumerator LoadYourAsyncScene(string SceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
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

    public void LoggedIn(){
        profile.GetUserProfile();
    }

    void Start(){

        showWindow(0);

        // PlayerPrefs.DeleteKey("Played practice"); 

        if( !PlayerPrefs.HasKey("Played practice") ){
            EventsButton.interactable = false;         
        }

        int id = ThreadSafeRandom.ThisThreadsRandom.Next(audios.Count);
        BackgroundMusic.clip = audios[id];

        BackgroundMusic.Play();
        
        // BG Music 
        if( !PlayerPrefs.HasKey("isAudio") )
            PlayerPrefs.SetInt("isAudio", 1);
        else if( PlayerPrefs.GetInt("isAudio") == 0 ){
            BackgroundMusic.volume = 0.0f;
            AudioBackgroundColor.sprite = audio_off_icon;
        }

    }

}

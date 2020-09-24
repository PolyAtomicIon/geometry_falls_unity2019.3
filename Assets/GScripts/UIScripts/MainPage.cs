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

    // public List<GameObject> windows;
    public List<Canvas> windows;


    public GameObject couponInformationWindow;
    public List<TMP_Text> coupon_info_texts;
    public GameObject eventInformationWindow;
    public List<TMP_Text> event_info_texts;
    public Button EventStartButton;
    public Button EventsButton;
    public GameObject EventsButton_message;

    public GameObject sideBar;

    public Image AudioBackgroundColor; 
    public Sprite audio_on_icon; 
    public Sprite audio_off_icon; 

    public AudioSource BackgroundMusic;
    public Color BgEnabledColor;
    
    public List<AudioClip> audios;

    public Animator SplashScreenAnimator;

    private int levels = 0;

    public int currentWindowIndex = 0;

    public GameObject login_button, logout_button; 

    public TMP_Text rules_text;

    public TMP_Text error_text_field;

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

    public void set_token(string a){
        PlayerPrefs.SetString("auth_token", a);
    }

    public string get_token(){
        if( !PlayerPrefs.HasKey("auth_token") )
            return "";
        return PlayerPrefs.GetString("auth_token");
    }

    // public void Logout_confirmatoin()

    public void Reload(){
        SceneManager.LoadScene("newMainScene");
    }

    public void LoadGame(){
        LoadGame();
    }

    public void Logout(){
        set_token("");
        
        // controller.coupons_clear();
        // foreach (Transform child in controller.coupon_panel.transform)
        //     GameObject.Destroy(child.gameObject);

        // showWindow(5);
        Reload();
    }

    // coupon information window, xor operatio, if opened close, else open
    // public void CouponInformationWindow (Coupon cur_coupon){
    //     couponInformationWindow.SetActive( true );
    //     coupon_info_texts[0].text = cur_coupon.provider_name;
    //     coupon_info_texts[1].text = cur_coupon.key;
    //     // coupon_info_texts[2].text = cur_coupon.value.ToString() + "%";
    //     coupon_info_texts[2].text = cur_coupon.value.ToString();
    // }

    // public void CloseCouponInformationWindow (){
    //     couponInformationWindow.SetActive( false );
    // }
    
    // public void EventInformationWindow (Event cur_event){

    //     // Level and Value should be added to infomration panel, we have to make request to get details

    //     eventInformationWindow.SetActive( true );
    //     event_info_texts[0].text = cur_event.name;
    //     event_info_texts[1].text = cur_event.description;
        
    //     Debug.Log(cur_event.description);
    //     if( cur_event.description == "" || cur_event.description == null ){
    //         event_info_texts[1].gameObject.SetActive(false);
    //     }
    //     else{
    //         event_info_texts[1].gameObject.SetActive(true);
    //     }

    //     event_info_texts[2].text = cur_event.start_date + " - " + cur_event.end_date;

    //     event_info_texts[3].text = cur_event.presents_left.ToString() + "/" + cur_event.presents_total.ToString();
    //     event_info_texts[4].text = cur_event.levels.ToString();

    //     // Debug.Log("ACTIVE?");
    //     // Debug.Log(cur_event.active);

    //     if( !cur_event.played && cur_event.active ){
    //         EventStartButton.onClick.AddListener(delegate{StartEvent(cur_event.id);});
    //         EventStartButton.enabled = true;
    //     }
    //     // if already played this game, disable button, show an error
    //     else{
    //         EventStartButton.enabled = false;
    //         // if( cur_event.played )
    //         //     windows[7].SetActive(true);
    //         // else
    //         //     // windows[8].SetActive(true);
    //     }

    // }

    // public void CloseEventInformationWindow (){
    //     eventInformationWindow.SetActive( false );
    //     // windows[7].SetActive(false);
    //     // windows[8].SetActive(false);
    // }

    // side bar, with music stuff
    public void showSideBar(bool condition) {
        sideBar.SetActive( condition );
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
                Debug.Log(www.error);
                // windows[6].SetActive(true);
            }
            else
            {   
                Debug.Log("Request sent!");
                JSONNode details = JSONNode.Parse(www.downloadHandler.text);
            
                rules_text.text = details["agreement"];
                // rules_text.text = "agreement";

            }
        }

    }

    IEnumerator Check_User_Verification(){
        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Get(user_profile_url))
        {
            www.SetRequestHeader("Authorization", get_token());
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                
                // show_error();
            }
            else
            {
                // windows[6].SetActive(false);
                // windows[6].enabled  = false;


                JSONNode res = JSONNode.Parse(www.downloadHandler.text);
        
                Debug.Log("User profile");
                Debug.Log(res);

                bool email_verified = res["email_verified"];

                if( !email_verified ){
                    Logout();
                }
            }
        }
    }

    void Start(){

        showWindow(0);

        // PlayerPrefs.DeleteKey("Played practice"); 

        // if( !PlayerPrefs.HasKey("Played practice") ){
        //     EventsButton.interactable = false;
                
        //     EventsButton_message.SetActive(true);
        // }

        if( get_token() != "" ){
            // logout_button.SetActive(true);
            StartCoroutine( Check_User_Verification() );
        }
        // else{
        //     login_button.SetActive(true);
        // }

        StartCoroutine( set_rules_text() );

        // if( !PlayerPrefs.HasKey("Read Agreement") ){
        //     showWindow(9);
        //     PlayerPrefs.SetInt("Read Agreement", 1); 
        // }

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

        controller = FindObjectOfType<webRequestController>();
    }

}

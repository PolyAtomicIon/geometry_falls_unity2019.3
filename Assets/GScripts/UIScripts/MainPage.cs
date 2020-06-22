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

    public List<GameObject> windows;
    public GameObject couponInformationWindow;
    public GameObject eventInformationWindow;
    public GameObject sideBar;

    public Image AudioBackgroundColor; 
    public AudioSource BackgroundMusic;
    public Color BgEnabledColor;

    private int levels = 0;

    public int currentWindowIndex = 0;
    // show window
    // 0 - Menu
    // 1 - Events
    // 2 - Coupons
    // 3 - Authorization
    // 4 - Sign Up
    // 5 - Login
    // 6 - Error
    public void showWindow (int windowIndex = 0){
        windows[ currentWindowIndex ].SetActive(false);
            
        windows[ windowIndex ].SetActive(true);
        currentWindowIndex = windowIndex;

        // close erroe window
        windows[ 6 ].SetActive(false);
    }

    public void set_token(string a){
        PlayerPrefs.SetString("auth_token", a);
    }

    public string get_token(){
        if( !PlayerPrefs.HasKey("auth_token") )
            return "";
        return PlayerPrefs.GetString("auth_token");
    }

    public void Logout(){
        set_token("");
        
        controller.coupons_clear();
        foreach (Transform child in controller.coupon_panel.transform)
            GameObject.Destroy(child.gameObject);

        showWindow(0);
    }

    // coupon information window, xor operatio, if opened close, else open
    public void CouponInformationWindow (Coupon cur_coupon){
        couponInformationWindow.SetActive( true );
        couponInformationWindow.GetComponentsInChildren<TMP_Text>()[0].text = cur_coupon.provider_name;
        couponInformationWindow.GetComponentsInChildren<TMP_Text>()[1].text = cur_coupon.key;
        couponInformationWindow.GetComponentsInChildren<TMP_Text>()[2].text = cur_coupon.value.ToString() + "%";
    }

    public void CloseCouponInformationWindow (){
        couponInformationWindow.SetActive( false );
    }
    
    public void EventInformationWindow (Event cur_event){

        // Level and Value should be added to infomration panel, we have to make request to get details

        eventInformationWindow.SetActive( true );
        eventInformationWindow.GetComponentsInChildren<TMP_Text>()[0].text = cur_event.name;
        eventInformationWindow.GetComponentsInChildren<TMP_Text>()[1].text = cur_event.description;


        eventInformationWindow.GetComponentsInChildren<TMP_Text>()[2].text = cur_event.start_date + " - " + cur_event.end_date;

        eventInformationWindow.GetComponentsInChildren<TMP_Text>()[3].text = cur_event.presents_left.ToString() + "/" + cur_event.presents_total.ToString() + " coupons left";
        eventInformationWindow.GetComponentsInChildren<TMP_Text>()[4].text = "Levels " + cur_event.levels.ToString();

        if( !cur_event.played ){
            eventInformationWindow.GetComponentsInChildren<Button>()[0].onClick.AddListener(delegate{StartEvent(cur_event.id);});
            eventInformationWindow.GetComponentsInChildren<Button>()[0].enabled = true;
        }
        // if already played this game, disable button, show an error
        else{
            eventInformationWindow.GetComponentsInChildren<Button>()[0].enabled = false;
            windows[7].SetActive(true);
        }

    }

    public void CloseEventInformationWindow (){
        eventInformationWindow.SetActive( false );
        windows[7].SetActive(false);
    }

    // side bar, with music stuff
    public void showSideBar(bool condition) {
        sideBar.SetActive( condition );
    }

    public void StartPractice (){
        // set id to -1, to say it is practice
        PlayerPrefs.SetInt("id", -1);
        SceneManager.LoadScene("Base");
    }

    IEnumerator TEMPGIVEPRIZE(int id)
    {
        WWWForm form = new WWWForm();
        form.AddField("level", 4); 
            
        using (UnityWebRequest www = UnityWebRequest.Post("http://94.247.128.162/api/game/events/1/present/", form))
        {
              
            www.SetRequestHeader("Authorization", get_token());
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                windows[6].SetActive(true);
            }
            else
            {
                Debug.Log("Request sent!");
            }
        }
    }

    public void StartEvent(int id){
        // StartCoroutine(TEMPGIVEPRIZE(id));
        // send id of event to player prefs
        PlayerPrefs.SetInt("id", id);
        // load game
        SceneManager.LoadScene("Base");
    }

    public void SetAudio(){
        // BackgroundMusic.enabled = ( BackgroundMusic.enabled ^ true );
        if( BackgroundMusic.volume == 0.6f )
            BackgroundMusic.volume = 0.0f;
        else
            BackgroundMusic.volume = 0.6f;

        if( BackgroundMusic.volume == 0.6f ){
            PlayerPrefs.SetInt("isAudio", 1);
            AudioBackgroundColor.color = BgEnabledColor;
        }
        else{
            PlayerPrefs.SetInt("isAudio", 0);
            AudioBackgroundColor.color = new Color(0, 0, 0, 255);            
        }
    }

    void Start(){
        
        // BG Music 
        if( !PlayerPrefs.HasKey("isAudio") )
            PlayerPrefs.SetInt("isAudio", 1);
        else if( PlayerPrefs.GetInt("isAudio") == 0 ){
            BackgroundMusic.volume = 0.0f;
            AudioBackgroundColor.color = new Color(0, 0, 0, 255);           
        }

        controller = FindObjectOfType<webRequestController>();
    }

}

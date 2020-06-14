using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainPage : MonoBehaviour {

    public List<GameObject> windows;
    public GameObject coupunInformationWindow;
    public GameObject sideBar;

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
    }

    public void set_token(string a){
        PlayerPrefs.SetString("auth_token", a);
    }

    public string get_token(){
        if( !PlayerPrefs.HasKey("auth_token") )
            return null;
        return PlayerPrefs.GetString("auth_token");
    }

    // coupon information window, xor operatio, if opened close, else open
    public void informationWindow (){
        coupunInformationWindow.SetActive( true );
    }

    public void CloseInformationWindow (){
        coupunInformationWindow.SetActive( false );
    }

    // side bar, with music stuff
    public void showSideBar(bool condition) {
        sideBar.SetActive( condition );
    }

    public void StartPractice (){
        SceneManager.LoadScene("Base");
    }

    public void StartEvents (){
        return;
    }


}

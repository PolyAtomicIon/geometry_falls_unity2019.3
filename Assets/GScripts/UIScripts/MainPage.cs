using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainPage : MonoBehaviour {

    public List<GameObject> windows;
    public GameObject coupunInformationWindow;
    public GameObject sideBar;

    private int currentWindowIndex = 0;

    private string auth_token;

    public void set_token(string a){
        auth_token = a;
    }

    public string get_token(){
        return auth_token;
    }

    // show window
    // 0 - Menu
    // 1 - Events
    // 2 - Coupons
    // 3 - Authorization
    // 4 - Sign Up
    // 5 - Login
    // 6 - Error
    public void showWindow (int windowIndex = 0, int closeWindow = -1){
       
        if( closeWindow != -1 )
            windows[ closeWindow ].SetActive(false);
        else
            windows[ currentWindowIndex ].SetActive(true);
            
        windows[ windowIndex ].SetActive(true);
        currentWindowIndex = windowIndex;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainPage : MonoBehaviour {

    public List<GameObject> windows;
    public GameObject coupunInformationWindow;
    public GameObject sideBar;

    private int currentWindowIndex = 0;

    // show window
    public void showWindow (int windowIndex = 0){
        windows[ currentWindowIndex ].SetActive(false);
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

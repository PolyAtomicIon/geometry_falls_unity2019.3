using System.Collections;
using UnityEngine.SceneManagement; 
using System.Collections.Generic;
using UnityEngine;
using Random=System.Random;
using Random2=UnityEngine.Random;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;
using System;   

public class GameOver : MonoBehaviour
{

    public Manager game_manager;

    // buttons: restart, game_over
    public GameObject section;
    public GameObject section_buttons;

    public GameObject Pause_button;
    public GameObject Blur;


    public GameObject spinning_object;
    public FortuneWheelManager FortuneWheelManager;

    public GameObject couponInformation;
    public GameObject couponInformation2;

    public void UI_changer(int score, bool isEvent = false){
        
        Time.timeScale = 1f;

        if( isEvent )
            spinning_object.SetActive(true);
        else
            section_buttons.SetActive(true);
        
        Blur.SetActive(true);
        Pause_button.SetActive(false);
    }

    public void GiveAward(bool is_present){
        spinning_object.SetActive(false);
        if( is_present )
            couponInformation.SetActive(true);
        else
            couponInformation2.SetActive(true);
    }

    IEnumerator GetPrize(int id, int levels_done)
    {
        WWWForm form = new WWWForm();
        form.AddField("level", levels_done); 

        string url = "http://94.247.128.162/api/game/events/" + id.ToString() + "/present/";

        // Debug.Log( "LEVELS done " + levels_done.ToString());
        // Debug.Log( url );

        using (UnityWebRequest www = UnityWebRequest.Post(url, form ))
        {
            Debug.Log(Manager.get_token());
            
            www.SetRequestHeader("Authorization", Manager.get_token());
            
            yield return www.SendWebRequest();

            Debug.Log("Prizes: Request sent!");
        
            int present_value = 0;
            int present_id = -1;
            bool is_present = false;
            List<int> values_randomizer = new List<int>();

            Debug.Log(www.isNetworkError);      
            Debug.Log(www.isHttpError); 
            
            // Retrieave Data is no errors have been found
            // Else just show empty randomizer
            if ( !www.isNetworkError && !www.isHttpError ){

                Debug.Log("env");
                JSONNode details = JSONNode.Parse(www.downloadHandler.text);
                Debug.Log(details);

                // 1. GET LIST OF PRESENTS
                // 2. Get index of present
                // 3. SEND INFO TO FORTUNE WHEEL


                for(int i=0; i<details["presents"].Count; i++)
                    if( details["presents"][i]["win"] ){
                        present_value = details["presents"][i]["value"];
                        values_randomizer.Add( details["presents"][i]["value"] );
                        is_present = true;
                        break;
                    }

                for(int i=0; i<details["presents"].Count; i++){
                    if( values_randomizer.Count >= 11 ) break;
                    if( details["presents"][i]["win"] ){
                        continue;
                    }
                    values_randomizer.Add( details["presents"][i]["value"] );
                }
                    
                if( details["present"] != null ){
                    // couponInformation.GetComponentsInChildren<TMP_Text>()[3].text = details["present"]["provider"]["name"];
                    // couponInformation.GetComponentsInChildren<TMP_Text>()[5].text = details["present"]["value"];
                }

            }
            else{
                game_manager.ServerError();
            }

            values_randomizer.Add(0);

            int tmp_id = 0;

            // number of Present coupon value is repeated, it should be less
            int pr_sz = 12 / values_randomizer.Count;
            if( values_randomizer.Count > 1 ){
                pr_sz --;
            }
            int pr_cnt = 1; // number of Present coupon inserted

            while( values_randomizer.Count < 12 ){

                if( values_randomizer[tmp_id] == present_value && pr_cnt < pr_sz ){
                    values_randomizer.Add(values_randomizer[tmp_id]);
                    pr_cnt++;
                }
                else if( values_randomizer[tmp_id] != present_value ){
                    values_randomizer.Add(values_randomizer[tmp_id]);
                }

                tmp_id++;
                tmp_id %= values_randomizer.Count;
            }
            // Debug.Log(values_randomizer);
            values_randomizer.Shuffle();

            for(int i=0; i<12; i++){
                if( present_value == values_randomizer[i] ){
                    present_id = i;
                    break;
                }
            }

            FortuneWheelManager.values = values_randomizer;
            FortuneWheelManager.p_id = present_id;
            FortuneWheelManager.is_present = is_present;

            FortuneWheelManager.ParsedData();

            UI_changer(levels_done, true);

        }
    }

    public void game_over(int level)
    {
        
        // get ID of Event
        int id = PlayerPrefs.GetInt("id");
        int levels = PlayerPrefs.GetInt("levels");
     
        Highscore.setHighscore(level-1);

        if( id != -1 && level > 1 ){
            Debug.Log("Getting Prize");
            // Debug.Log(id);
            Debug.Log(level);
            section.SetActive(true);
            StartCoroutine(GetPrize(id, level-1));
        }
        else{
            Debug.Log("Chill, just practice");
            UI_changer(level, false);
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Base"));

    }

    void Start()
    {
        game_manager = FindObjectOfType<Manager>();
    }

}

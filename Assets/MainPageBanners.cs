using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using TMPro;
using UnityEngine.UI;
using System;
public class MainPageBanners : MonoBehaviour
{

    public List<Image> banners = new List<Image>();
    
    IEnumerator GetTextureBanner (int id, string url) {

        Debug.Log(url);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
            Debug.Log("Banner sprite = ERROR?");
        }
        else {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Sprite bg = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);   
            banners[id].sprite = bg;
        }

    }  

 
    IEnumerator GetBannersURL(){
        string url  = "http://94.247.128.162/api/game/banners/";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {   
            // www.SetRequestHeader("Authorization", Manager.get_token());
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error + "Buttons url");
            }
            else
            {   
                Debug.Log("Request sent!");

                JSONNode banners_url = JSONNode.Parse(www.downloadHandler.text);

                for(int i=0; i<banners_url["results"].Count; i++){
                    StartCoroutine( GetTextureBanner(i, banners_url["results"][i]["image"]) );
                }

            }
        }

    }

    void Start()
    {
        StartCoroutine( GetBannersURL() );
    }

}

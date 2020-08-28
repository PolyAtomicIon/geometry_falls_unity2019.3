using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using TMPro;
using UnityEngine.UI;
using System;

public class Event{
    public int id;
    public string name;
    public string start_date, end_date;
    public bool active;
    public bool played;

    public Sprite background;

    public int value = -1;
    public int levels = -1;
    public string provider_name = "NONE";

    public string description = "Жду данные";

    public int presents_total;
    public int presents_left;

    public int days_left;

    public string date_information;

    public Event() { }

    public Event(int ID, string e_name, string start, string end, bool is_active)
    {
        id = ID;
        name = e_name;
        
        // Debug.Log(start);
        // Debug.Log(end); 

        // Date
        // 2020-06-01T00:00:00
        // 2020-08-31T00:00:00
        // 1. split by T
        // 2. Split by -
        // 3. Get year
        // 4. get dd and mm 1
        // 5. get dd and mm 2
        // 6. get time 1 and 2
        // 7. Deadline = dd2 mm2 yyyy
        // 8. Details = dd1.mm1 - dd2.mm2.yyyy 

        DateTime localDate = DateTime.Now;

        start_date = start;
        end_date = end;
        
        string[] st_d = start_date.Split('T');
        string[] st_1 = st_d[0].Split('-');

        DateTime date1 = DateTime.ParseExact(st_d[0], "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

        string dd1 = st_1[2];
        string mm1 = st_1[1];

        st_d = end_date.Split('T');
        string[] st_2 = st_d[0].Split('-');

        DateTime date2 = DateTime.ParseExact(st_d[0], "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

        string dd2 = st_2[2];
        string mm2 = st_2[1];
        string yyyy = st_2[0];

        start_date = dd1 + '.' + mm1;
        end_date = dd2 + '.' + mm2 + '.' + yyyy;

        int st_days_left = date1.Subtract(localDate).Days;

        days_left = date2.Subtract(localDate).Days;
        // Debug.Log("DAYS BT");
        // Debug.Log(days_left);

        if( st_days_left > 0 )
            date_information = "До старта\n'" + st_days_left.ToString() + " дня";
        else
            date_information = "До конца\n" + days_left.ToString() + " дней";

        active = is_active;
    }


}

public class Coupon{
    
    // id : 1   
    // key :"0EUjD9h7Juu9Qd8VtuusPsL4ycJWNW6z\"
    // value : 50         
    // provider_id : 
    // provider_name : "KFC"

    public int id;
    public string key;
    public int value;
    public int provider_id;
    public string provider_name;

    public Sprite background;

    public Coupon() { }

    public Coupon(int ID, string e_key, int Value, int provider_ID, string provider_Name)
    {
        id = ID;
        key = e_key;
        value = Value;
        provider_id = provider_ID;
        provider_name = provider_Name;
    }


}

public class webRequestController : MonoBehaviour
{

    public MainPage manager;
    // to check if events information parsed and initialized
    bool events_initialized = false;

    Dictionary<int, Event> events = new Dictionary<int, Event>();
    public GameObject event_panel;
    public Button event_prefab;
    public string base_event_url = "http://94.247.128.162/api/game/events/";

    string cur_token = "";


    IEnumerator GetEventDetails(int id)
    {
        
        // if not logged in, go to Authorization UI 
        // Debug.Log(manager.get_token());
        if( manager.get_token() == "" ){
            manager.showWindow(3);
            yield return false;
        }   

        using (UnityWebRequest www = UnityWebRequest.Get(base_event_url + id.ToString() + "/?"))
        {   

            www.SetRequestHeader("Authorization", manager.get_token());
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                manager.windows[6].SetActive(true);
                events_initialized = false;
            }
            else
            {   
                Debug.Log("Request sent!");

                JSONNode details = JSONNode.Parse(www.downloadHandler.text);
               
                events[id].levels = details["levels"];
                events[id].played = details["played"];
                events[id].presents_total = details["presents_total"];
                events[id].presents_left = details["presents_left"];
                    
                string description = details["description"];

                if( description != null && description.Length > 128 ){
                    description = description.Substring(0, 128) + "..."; 
                }
                
                events[id].description = description;

                manager.EventInformationWindow(events[id]);
                PlayerPrefs.SetInt("levels", events[id].levels);
            }

        }

    }

    public void EventButtonClicked(int id){
        // Debug.Log("clicked");
        // Debug.Log(id);
        StartCoroutine(GetEventDetails(id));
        return;
    }

    Sprite BG;

    void instantiateEventButton(Event cur_event, Vector3 position){

        event_prefab.GetComponentsInChildren<TMP_Text>()[0].text = cur_event.name;
        event_prefab.GetComponentsInChildren<TMP_Text>()[2].text = cur_event.date_information;

        Button event_prefab_go = Instantiate(event_prefab) as Button;
        event_prefab_go.transform.parent = event_panel.transform;

        event_prefab_go.onClick.AddListener(delegate{EventButtonClicked(cur_event.id);});

        event_prefab_go.GetComponent<RectTransform>().anchoredPosition = position;
        event_prefab_go.GetComponent<RectTransform>().localScale  = new Vector3(1, 1, 1);

        event_prefab_go.GetComponent<RectTransform>().SetLeft(16f);
        event_prefab_go.GetComponent<RectTransform>().SetRight(16f);
        
        if( cur_event.background != null )
            event_prefab_go.transform.Find("Content/Background").GetComponent<Image>().sprite = cur_event.background;

    }

    void CreateEventButtons(){

        Vector3 cur_position = new Vector3(0, -96, 0);
        Vector3 gap_between = new Vector3(0, -176-16, 0);

        // Debug.Log(events.Count);

        foreach (KeyValuePair<int, Event> e in events){
            instantiateEventButton(e.Value, cur_position);
            cur_position += gap_between;
        }

    }

    IEnumerator GetTextureEvent(int id, string url) {
        Debug.Log(url);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
            Debug.Log("WHAT?");
        }
        else {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Sprite bg = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            
            events[id].background = bg;
        }
    }

    IEnumerator CreateEvent(UnityWebRequest res){

        JSONNode events_info = JSONNode.Parse(res.downloadHandler.text);
        // Debug.Log(events_info);
        for(int i = 0; i < events_info["count"]; i++){

            // get data
            int ID = events_info["results"][i]["id"];
            string e_name = events_info["results"][i]["name"];
            string start = events_info["results"][i]["start_date"];
            string end = events_info["results"][i]["end_date"];
            bool is_active = events_info["results"][i]["active"];
            
            string url = events_info["results"][i]["image"];

            // Debug.Log(ID);

            // create Button
            Event cur_event = new Event(ID, e_name, start, end, is_active);

            events[ID] = cur_event;

            StartCoroutine( GetTextureEvent(ID, url) );

        }

        yield return new WaitForSeconds(1f);
        
        CreateEventButtons();
    }

    IEnumerator GetEvents()
    {
        
        // if not logged in, go to Authorization UI 
        // Debug.Log(manager.get_token());
        if( manager.get_token() == "" ){
            manager.showWindow(3);
            yield return false;
        }

        using (UnityWebRequest www = UnityWebRequest.Get(base_event_url))
        {   

            www.SetRequestHeader("Authorization", manager.get_token());
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                manager.windows[6].SetActive(true);
                events_initialized = false;
            }
            else
            {   
                Debug.Log("Request sent!");
                StartCoroutine( CreateEvent(www) );
            }
        }

    }

    // to check if coupons information parsed and initialized
    bool coupons_initialized = false;
    
    Dictionary<int, Coupon> coupons = new Dictionary<int, Coupon>();

    public GameObject coupon_panel;
    public Button coupon_prefab;
    string base_coupon_url = "http://94.247.128.162/api/game/presents/";

    public void coupons_clear(){
        coupons.Clear();
    }

    public void CouponButtonClicked(int id){
        // Debug.Log("clicked");
        // Debug.Log(id);
        manager.CouponInformationWindow(coupons[id]);
        return;
    }

    void instantiateCouponButton(Coupon cur_coupon, Vector3 position){
        coupon_prefab.GetComponentsInChildren<TMP_Text>()[0].text = cur_coupon.provider_name.ToString();
        Button coupon_prefab_go = Instantiate(coupon_prefab) as Button;
        coupon_prefab_go.transform.parent = coupon_panel.transform;

        coupon_prefab_go.onClick.AddListener(delegate{CouponButtonClicked(cur_coupon.id);});

        coupon_prefab_go.GetComponent<RectTransform>().anchoredPosition = position;
        coupon_prefab_go.GetComponent<RectTransform>().localScale  = new Vector3(1, 1, 1);

        coupon_prefab_go.GetComponent<RectTransform>().SetLeft(16f);
        coupon_prefab_go.GetComponent<RectTransform>().SetRight(16f);

        if( cur_coupon.background != null )
            coupon_prefab_go.transform.Find("Content/Background").GetComponent<Image>().sprite = cur_coupon.background;

    }


    void CreateCouponButtons(){

        Vector3 cur_position = new Vector3(0, -96, 0);
        Vector3 gap_between = new Vector3(0, -160, 0);

        // Debug.Log(coupons.Count);

        foreach (KeyValuePair<int, Coupon> c in coupons){
            instantiateCouponButton(c.Value, cur_position);
            cur_position += gap_between;
        }

    }
    IEnumerator GetTextureCoupon (int id, string url) {
        Debug.Log(url);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
            Debug.Log("WHAT?");
        }
        else {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Sprite bg = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            
            coupons[id].background = bg;
        }
    }    
    
    IEnumerator CreateCoupon(UnityWebRequest res){

        JSONNode coupons_info = JSONNode.Parse(res.downloadHandler.text);
        
        for(int i = 0; i < coupons_info["count"]; i++){

            // get data
            int ID = coupons_info["results"][i]["id"];
            string e_key = coupons_info["results"][i]["key"];
            int Value = coupons_info["results"][i]["value"];
            int provider_ID = coupons_info["results"][i]["provider"]["id"];
            string provider_Name = coupons_info["results"][i]["provider"]["name"];
            
            // string url = events_info["results"][i]["image"];
            string url = "http://94.247.128.162/media/events/images/jake-the-dog.png";

            // Debug.Log(provider_Name);

            // create Button
            Coupon cur_coupon = new Coupon(ID, e_key, Value, provider_ID, provider_Name);

            // StartCoroutine( GetTextureCoupon(ID, url) );

            coupons[ID] = cur_coupon;
        }

        yield return new WaitForSeconds(1f);

        CreateCouponButtons();

    }

    IEnumerator GetCoupons()
    {
        
        // if not logged in, go to Authorization UI 
        if( manager.get_token() == "" ){
            manager.showWindow(3);
            yield return false;
        }

        using (UnityWebRequest www = UnityWebRequest.Get(base_coupon_url))
        {   
            // Debug.Log(manager.get_token());
            www.SetRequestHeader("Authorization", manager.get_token());
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                manager.windows[6].SetActive(true);
                coupons_initialized = false;
            }
            else
            {   
                Debug.Log("Request sent!");
                StartCoroutine( CreateCoupon(www) );
            }
        }

    }

    
    public void GetData(){
        events_initialized = true;
        StartCoroutine( GetEvents() );
        coupons_initialized = true;
        StartCoroutine( GetCoupons() );  
        cur_token = PlayerPrefs.GetString("auth_token");
    }

    void Start(){

        // if( PlayerPrefs.GetString("auth_token") == null || PlayerPrefs.GetString("auth_token") == "" ){
        //     manager.showWindow(3);
        // }
        // else{
        //     GetData();
        // }

        // if authorized get data, else nothing
        if( !( PlayerPrefs.GetString("auth_token") == null || PlayerPrefs.GetString("auth_token") == "" ) ){
            GetData();
        }

    }

    void LateUpdate()
    {
    
        // // if current Window on Event UI
        // if( !events_initialized ){
        //     events_initialized = true;
        //     StartCoroutine( GetEvents() );
        // }

        // // if current Window on Coupon UI
        // if(!coupons_initialized ){
        //     coupons_initialized = true;
        //     StartCoroutine( GetCoupons() );
        // }


    }
}

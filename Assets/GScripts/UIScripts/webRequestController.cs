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
            date_information = "Дней до старта: " + st_days_left.ToString();
        else
            if( days_left < 0 ){
                date_information = "Прошло";
            }
            else
                date_information = "Дней до конца: " + days_left.ToString();

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

    public string measurement;
    public string instagram_url;
    public string twogis_url;

    public string description;

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
    Dictionary<string, string> Measurements = new Dictionary<string, string>();

    // to check if events information parsed and initialized
    bool events_initialized = false;

    Dictionary<int, Event> events = new Dictionary<int, Event>();
    public GameObject event_panel;
    public GameObject event_prefab;
    public string base_event_url = "http://94.247.128.162/api/game/events/";

    string cur_token = "";

    public void EventButtonClicked(int id){
        Debug.Log("clicked" + id);
        PlayerPrefs.SetInt("levels", events[id].levels);
        if( !PlayerPrefs.HasKey("user_verified") || PlayerPrefs.GetInt("user_verified") == 0 ){
            manager.Verify();
        }
        else if( manager.get_token() == "" || manager.get_token() == null ){
            manager.showWindow(5); // login
        }
        else{
            manager.StartEvent(id);
        }
    }

    Sprite BG;

    void instantiateEventButton(Event cur_event, Vector3 position){

        GameObject event_prefab_go = Instantiate(event_prefab) as GameObject;
        event_prefab_go.transform.parent = event_panel.transform;

        TMP_Text[] text_fields = event_prefab_go.GetComponentsInChildren<TMP_Text>();

        text_fields[0].text = cur_event.name;

        text_fields[1].text = cur_event.description;

        text_fields[2].text = cur_event.presents_left.ToString() + "/" + cur_event.presents_total.ToString();
        text_fields[4].text = cur_event.levels.ToString();

        text_fields[6].text = cur_event.date_information;

        // Debug.Log(event_prefab_go.GetComponentsInChildren<Button>()[0]);
        if( cur_event.active && !cur_event.played )
            event_prefab_go.GetComponentsInChildren<Button>()[0].onClick.AddListener(delegate{EventButtonClicked(cur_event.id);});
        else{
            event_prefab_go.GetComponentsInChildren<Button>()[0].interactable = false;
            if( cur_event.played )
                text_fields[7].text = "Вы уже играли";
            else
                text_fields[7].text = "Не активно";
        }

        event_prefab_go.GetComponent<RectTransform>().anchoredPosition = position;
        event_prefab_go.GetComponent<RectTransform>().localScale  = new Vector3(1, 1, 1);

        if( cur_event.background != null )
            event_prefab_go.transform.Find("ImagePanel/Image").GetComponent<Image>().sprite = cur_event.background;

    }

    Vector3 cur_event_position = new Vector3(352, 0, 0);
    Vector3 gap_between = new Vector3(768, 0, 0);

    void setEventsPanel(int events_number){

        RectTransform panel_rect_transform = event_panel.GetComponent<RectTransform>();

        float width = 800f * events_number;
        Vector3 tmp_position = new Vector3(width / 2, 0f, 0f);
        Vector2 tmp_size = new Vector2(width, panel_rect_transform.sizeDelta.y); 

        panel_rect_transform.sizeDelta = tmp_size;
        panel_rect_transform.anchoredPosition = tmp_position;
    }

    IEnumerator GetTextureEvent(int id, string url) {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
            Debug.Log(url);
            Debug.Log("WHAT?");
        }
        else {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Sprite bg = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            
            events[id].background = bg;
        }
    
        instantiateEventButton(events[id], cur_event_position);
        cur_event_position += gap_between;
    }

    void CreateEvent(UnityWebRequest res){

        JSONNode events_info = JSONNode.Parse(res.downloadHandler.text);
        Debug.Log(events_info);

        setEventsPanel(events_info["results"].Count);

        for(int i = 0; i < events_info["count"]; i++){

            // get data
            int ID = events_info["results"][i]["id"];
            string e_name = events_info["results"][i]["name"];
            string start = events_info["results"][i]["start_date"];
            string end = events_info["results"][i]["end_date"];
            bool is_active = events_info["results"][i]["active"];
            
            string url = events_info["results"][i]["image"];

            int levels = events_info["results"][i]["levels"];
            bool played = events_info["results"][i]["played"];
            int presents_total = events_info["results"][i]["presents_total"];
            int presents_left = events_info["results"][i]["presents_left"];
                
            string description = events_info["results"][i]["description"];

            if( description != null && description.Length > 96 ){
                description = description.Substring(0, 96) + "..."; 
            }


            // Debug.Log(ID);

            // create Button
            Event cur_event = new Event(ID, e_name, start, end, is_active);

            events[ID] = cur_event;
            events[ID].levels = levels;
            events[ID].played = played;
            events[ID].presents_total = presents_total;
            events[ID].presents_left = presents_left;
            events[ID].description = description;

            StartCoroutine( GetTextureEvent(ID, url) );

        }
    }

    IEnumerator GetEvents()
    {
        
        // if not logged in, go to Authorization UI 
        // Debug.Log(manager.get_token());
        if( manager.get_token() == "" ){
            manager.showWindow(3);
            yield return false;
        }

        using (UnityWebRequest www = UnityWebRequest.Get("http://94.247.128.162/api/game/events/?limit=18"))
        {   

            www.SetRequestHeader("Authorization", Manager.get_token());
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                // manager.windows[6].SetActive(true);
                events_initialized = false;
            }
            else
            {   
                Debug.Log("Request sent!");
                // StartCoroutine( CreateEvent(www) )
                CreateEvent(www);
            }
        }

    }

    // to check if coupons information parsed and initialized
    bool coupons_initialized = false;
    
    Dictionary<int, Coupon> coupons = new Dictionary<int, Coupon>();

    public GameObject coupon_panel;
    public GameObject coupon_prefab;
    string base_coupon_url = "http://94.247.128.162/api/game/presents/";

    Vector3 cur_coupon_position = new Vector3(352, 0, 0);

    void setCouponsPanel(int coupons_number){

        RectTransform panel_rect_transform = coupon_panel.GetComponent<RectTransform>();

        float width = 800f * coupons_number;
        Vector3 tmp_position = new Vector3(width / 2, 0f, 0f);
        Vector2 tmp_size = new Vector2(width, panel_rect_transform.sizeDelta.y); 

        panel_rect_transform.sizeDelta = tmp_size;
        panel_rect_transform.anchoredPosition = tmp_position;
    }
    
    public void openUrl(string URL){
        Debug.Log(URL);
        Application.OpenURL(URL);
    }

    void instantiateCouponButton(Coupon cur_coupon, Vector3 position){

        TMP_Text[] text_fields = coupon_prefab.GetComponentsInChildren<TMP_Text>();

        text_fields[0].text = cur_coupon.provider_name;

        text_fields[1].text = cur_coupon.description;

        text_fields[3].text = cur_coupon.key;
        text_fields[5].text = cur_coupon.value.ToString() + cur_coupon.measurement;

        Button[] buttons = coupon_prefab.GetComponentsInChildren<Button>();

        GameObject coupon_prefab_go = Instantiate(coupon_prefab) as GameObject;
        coupon_prefab_go.transform.parent = coupon_panel.transform;
        
        // Debug.Log( cur_coupon.instagram_url );

        // set url for instagram and 2gis
        coupon_prefab_go.GetComponentsInChildren<Button>()[0].onClick.AddListener(delegate{openUrl(cur_coupon.instagram_url);});
        coupon_prefab_go.GetComponentsInChildren<Button>()[1].onClick.AddListener(delegate{openUrl(cur_coupon.twogis_url);});

        coupon_prefab_go.GetComponent<RectTransform>().anchoredPosition = position;
        coupon_prefab_go.GetComponent<RectTransform>().localScale  = new Vector3(1, 1, 1);

        if( cur_coupon.background != null )
            coupon_prefab_go.transform.Find("ImagePanel/Image").GetComponent<Image>().sprite = cur_coupon.background;


    }

    IEnumerator GetTextureCoupon (int id, string url) {
        Debug.Log(url);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
            Debug.Log("Coupon Texture error");
        }
        else {
            Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Sprite bg = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);
            
            coupons[id].background = bg;
        }

        instantiateCouponButton(coupons[id], cur_coupon_position);
        cur_coupon_position += gap_between;

    }    
    
    void CreateCoupon(UnityWebRequest res){

        JSONNode coupons_info = JSONNode.Parse(res.downloadHandler.text);
        
        setCouponsPanel( coupons_info["results"].Count );

        Debug.Log(coupons_info["results"]);

        for(int i = 0; i < coupons_info["count"]; i++){

            // get data
            int ID = coupons_info["results"][i]["id"];
            string e_key = coupons_info["results"][i]["key"];
            int Value = coupons_info["results"][i]["value"];
            string measurement = coupons_info["results"][i]["measurement"];

            int provider_ID = coupons_info["results"][i]["provider"]["id"];
            string provider_Name = coupons_info["results"][i]["provider"]["name"];
            
            string description = coupons_info["results"][i]["description"];

            string url = coupons_info["results"][i]["image"];
            // string url = "http://94.247.128.162/media/events/images/jake-the-dog.png";

            string instagram_url = coupons_info["results"][i]["instagram_url"];
            string twogis_url = coupons_info["results"][i]["double_gis_url"];

            // Debug.Log(provider_Name);
            Coupon cur_coupon = new Coupon(ID, e_key, Value, provider_ID, provider_Name);
            
            cur_coupon.measurement = measurement;
            cur_coupon.instagram_url = instagram_url;
            cur_coupon.twogis_url = twogis_url;
            cur_coupon.description = description;

            coupons[ID] = cur_coupon;

            StartCoroutine( GetTextureCoupon(ID, url) );
        }

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
            // Debug.Log(Manager.get_token());
            www.SetRequestHeader("Authorization", Manager.get_token());
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                // manager.windows[6].SetActive(true);
                coupons_initialized = false;
            }
            else
            {   
                Debug.Log("Request sent!");
                CreateCoupon(www);
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
        Measurements.Add("MEASUREMENT_PERCENT", "%");
    }

}

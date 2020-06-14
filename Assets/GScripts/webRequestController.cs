using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using TMPro;
using UnityEngine.UI;

public class Event{
    public int id;
    public string name;
    public string start_date, end_date;
    public bool active;

    public Event() { }

    public Event(int ID, string e_name, string start, string end, bool is_active)
    {
        id = ID;
        name = e_name;
        start_date = start;
        end_date = end;
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

    List<Event> events = new List<Event>();
    public GameObject event_panel;
    public Button event_prefab;
    string base_event_url = "http://94.247.128.162/api/game/events/";

    public void EventButtonClicked(int id){
        Debug.Log("clicked");
        Debug.Log(id);
        return;
    }

    void instantiateEventButton(Event cur_event, Vector3 position){
        event_prefab.GetComponentsInChildren<TMP_Text>()[0].text = cur_event.name;
        Button event_prefab_go = Instantiate(event_prefab) as Button;
        event_prefab_go.transform.parent = event_panel.transform;

        event_prefab_go.onClick.AddListener(delegate{EventButtonClicked(cur_event.id);});

        event_prefab_go.GetComponent<RectTransform>().anchoredPosition = position;
    }

    void CreateEventButtons(){

        Vector3 cur_position = new Vector3(0, -96, 0);
        Vector3 gap_between = new Vector3(0, -160, 0);

        Debug.Log(events.Count);

        foreach(Event e in events){
            instantiateEventButton(e, cur_position);
            cur_position += gap_between;
        }

    }

    void CreateEvent(UnityWebRequest res){

        JSONNode events_info = JSONNode.Parse(res.downloadHandler.text);
        
        for(int i = 0; i < events_info.Count; i++){

            // get data
            int ID = events_info[i]["id"];
            string e_name = events_info[i]["name"];
            string start = events_info[i]["start_date"];
            string end = events_info[i]["end_date"];
            bool is_active = events_info[i]["end_date"];
            
            // Debug.Log(ID);

            // create Button
            Event cur_event = new Event(ID, e_name, start, end, is_active);

            events.Add(cur_event);
        }

        CreateEventButtons();

    }

    IEnumerator GetEvents()
    {
        
        // if not logged in, go to Authorization UI 
        if( manager.get_token() == null ){
            manager.showWindow(3);
            yield return false;
        }

        using (UnityWebRequest www = UnityWebRequest.Get(base_event_url))
        {   
            Debug.Log(manager.get_token());
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
                CreateEvent(www);
            }
        }

    }

    // to check if coupons information parsed and initialized
    bool coupons_initialized = false;
    
    List<Coupon> coupons = new List<Coupon>();

    public GameObject coupon_panel;
    public Button coupon_prefab;
    string base_coupon_url = "http://94.247.128.162//api/game/presents/";

    public void CouponButtonClicked(int id){
        Debug.Log("clicked");
        Debug.Log(id);
        return;
    }

    void instantiateCouponButton(Coupon cur_coupon, Vector3 position){
        coupon_prefab.GetComponentsInChildren<TMP_Text>()[0].text = cur_coupon.provider_name;
        Button coupon_prefab_go = Instantiate(coupon_prefab) as Button;

        coupon_prefab_go.transform.parent = coupon_panel.transform;

        coupon_prefab_go.onClick.AddListener(delegate{CouponButtonClicked(cur_coupon.id);});

        coupon_prefab_go.GetComponent<RectTransform>().anchoredPosition = position;
    }

    void CreateCouponButtons(){

        Vector3 cur_position = new Vector3(0, -96, 0);
        Vector3 gap_between = new Vector3(0, -160, 0);

        Debug.Log(coupons.Count);

        foreach(Coupon c in coupons){
            instantiateCouponButton(c, cur_position);
            cur_position += gap_between;
        }

    }

    void CreateCoupon(UnityWebRequest res){

        JSONNode coupons_info = JSONNode.Parse(res.downloadHandler.text);

        for(int i = 0; i < coupons_info.Count; i++){

            // get data
            int ID = coupons_info[i]["id"];
            string e_key = coupons_info[i]["key"];
            int Value = coupons_info[i]["value"];
            int provider_ID = coupons_info[i]["provider"]["id"];
            string provider_Name = coupons_info[i]["end_date"]["name"];
            
            // Debug.Log(provider_Name);

            // create Button
            Coupon cur_coupon = new Coupon(ID, e_key, Value, provider_ID, provider_Name);

            coupons.Add(cur_coupon);
        }

        CreateCouponButtons();

    }

    IEnumerator GetCoupons()
    {
        
        // if not logged in, go to Authorization UI 
        if( manager.get_token() == null ){
            manager.showWindow(3);
            yield return false;
        }

        using (UnityWebRequest www = UnityWebRequest.Get(base_coupon_url))
        {   
            Debug.Log(manager.get_token());
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
                CreateCoupon(www);
            }
        }

    }

    void Start(){

    }

    void Update()
    {
        
        // if current Window on Event UI
        if( manager.currentWindowIndex == 1 && !events_initialized ){
            events_initialized = true;
            StartCoroutine( GetEvents() );
        }

        // if current Window on Coupon UI
        if( manager.currentWindowIndex == 2 && !coupons_initialized ){
            coupons_initialized = true;
            StartCoroutine( GetCoupons() );
        }

    }
}

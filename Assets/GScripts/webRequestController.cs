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

public class webRequestController : MonoBehaviour
{

    List<Event> events = new List<Event>();

    public MainPage manager;
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
            Debug.Log("yeah?");
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
            Debug.Log(ID);
            // create Button
            Event cur_event = new Event(ID, e_name, start, end, is_active);

            events.Add(cur_event);
        }

        CreateEventButtons();

    }

    IEnumerator GetEvents()
    {

        using (UnityWebRequest www = UnityWebRequest.Get(base_event_url))
        {   
            Debug.Log(manager.get_token());
            www.SetRequestHeader("Authorization", "Token 4a12221986fb0d9e9e7528d067520377efb6df9b");
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                manager.windows[6].SetActive(true);
            }
            else
            {   
                Debug.Log("Request sent!");
                CreateEvent(www);
            }
        }
    }

    void Start(){

        StartCoroutine( GetEvents() );

    }

    void Update()
    {

        

    }
}

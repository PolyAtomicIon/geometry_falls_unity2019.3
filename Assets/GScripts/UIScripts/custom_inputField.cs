using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;
using TMPro;
using UnityEngine.EventSystems;

public class custom_inputField : MonoBehaviour {

    public int index = -1;
    public authorization authorization_script;
    public TMP_InputField mainInputField;

    // int cur_verification_input_field = 0;
    // public List<TMP_InputField> verification_code_fields = new List<TMP_InputField>();
   
    void Start(){
        
        mainInputField = GetComponent<TMP_InputField>();

        authorization_script = FindObjectOfType<authorization>(); 
        mainInputField.onValueChanged.AddListener(delegate {ValueChangeCheck(); });
    }

    public void ValueChangeCheck()
    {        
        Debug.Log("Value Changed " + index.ToString());
        authorization_script.cur_verification_input_field = index;

        if( authorization_script.verification_code_fields[authorization_script.cur_verification_input_field].text.Length == 1 ){
            authorization_script.verification_code_fields[authorization_script.cur_verification_input_field].DeactivateInputField();
            authorization_script.cur_verification_input_field++;
            if( authorization_script.cur_verification_input_field >= 4 ) 
                authorization_script.cur_verification_input_field = -1;
            else
                authorization_script.verification_code_fields[authorization_script.cur_verification_input_field].ActivateInputField();
        }

    }


}


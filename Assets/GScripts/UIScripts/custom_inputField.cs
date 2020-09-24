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
    public Verification verification;
    public TMP_InputField mainInputField;

    // int cur_verification_input_field = 0;
    // public List<TMP_InputField> verification_code_fields = new List<TMP_InputField>();
   
    void Start(){
        
        mainInputField = GetComponent<TMP_InputField>();

        verification = FindObjectOfType<Verification>(); 
        mainInputField.onValueChanged.AddListener(delegate {ValueChangeCheck(); });
    }

    public void ValueChangeCheck()
    {        
        Debug.Log("Value Changed " + index.ToString());
        verification.cur_verification_input_field = index;

        int cur_field = verification.cur_verification_input_field;

        if( verification.verification_code_fields[cur_field].text.Length == 1 ){

            verification.verification_code_fields[cur_field].DeactivateInputField();
            
            cur_field++;
            if( cur_field >= 4 ) 
                cur_field = -1;

            verification.cur_verification_input_field = cur_field;
            
            if( cur_field > -1 )
                verification.verification_code_fields[cur_field].ActivateInputField();
           
        }

    }


}


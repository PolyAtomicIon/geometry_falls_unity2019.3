using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class FortuneWheelManager : MonoBehaviour
{
	public GameObject spinning_object;

    private bool _isStarted;
    private float[] _sectorsAngles;
    private float _finalAngle;
    private float _startAngle = 0;
    private float _currentLerpRotationTime;
    public Button TurnButton;
    public GameObject Circle; 			// Rotatable Object with rewards
    // public Text CoinsDeltaText; 		// Pop-up text with wasted or rewarded coins amount
    // public Text CurrentCoinsText; 		// Pop-up text with wasted or rewarded coins amount
    public int TurnCost = 300;			// How much coins user waste when turn whe wheel
	// public int CurrentCoinsAmount = 1000;	// Started coins amount. In your project it can be set up from CoinsManager or from PlayerPrefs and so on
    // public int PreviousCoinsAmount;		// For wasted coins animation

	public Manager game_manager;

	public List<Text> text_values;

    private void Awake ()
    {
        //PreviousCoinsAmount = CurrentCoinsAmount;
        //CurrentCoinsText.text = CurrentCoinsAmount.ToString ();
    }

	public void closeWheel(){
		spinning_object.SetActive(false);
	}

	public void ParsedData(){
		
		for(int i=0; i<12; i++){
			// add unit
			text_values[i].text = game_manager.values_randomizer[i].ToString();
		}

	}

    public void TurnWheel ()
    {
		
		// Player has enough money to turn the wheel

		_currentLerpRotationTime = 0f;
	
		// Fill the necessary angles (for example if you want to have 12 sectors you need to fill the angles with 30 degrees step)
		_sectorsAngles = new float[] { 30, 60, 90, 120, 150, 180, 210, 240, 270, 300, 330, 360 };

		Debug.Log(game_manager.present_id);
		int p_id = game_manager.present_id;

		int fullCircles = 5;
		float randomFinalAngle = _sectorsAngles [(12 - p_id) % 12];
	
		Debug.Log(randomFinalAngle);

		// Here we set up how many circles our wheel should rotate before stop
		_finalAngle = -(fullCircles * 360 + randomFinalAngle);
		_isStarted = true;
	
    }

	IEnumerator GiveAward(){
		yield return new WaitForSeconds(1.5f);
		game_manager.GiveAward();
	}

    void Update ()
    {
		TurnButton.interactable = true;
		TurnButton.GetComponent<Image>().color = new Color(255, 255, 255, 1);

    	if (!_isStarted)
    	    return;

    	float maxLerpRotationTime = 6f;
    
    	// increment timer once per frame
    	_currentLerpRotationTime += Time.deltaTime;
    	if (_currentLerpRotationTime > maxLerpRotationTime || Circle.transform.eulerAngles.z == _finalAngle) {
    	    _currentLerpRotationTime = maxLerpRotationTime;
    	    _isStarted = false;
    	    _startAngle = _finalAngle % 360;
    
			// GIVE AWARD!!!!!!!! BEKA

			StartCoroutine( GiveAward() );

    	    // GiveAwardByAngle ();
			//StartCoroutine(HideCoinsDelta ());
    	}
    
    	// Calculate current position using linear interpolation
    	float t = _currentLerpRotationTime / maxLerpRotationTime;
    
    	// This formulae allows to speed up at start and speed down at the end of rotation.
    	// Try to change this values to customize the speed
    	t = t * t * t * (t * (6f * t - 15f) + 10f);
    
    	float angle = Mathf.Lerp (_startAngle, _finalAngle, t);
    	Circle.transform.eulerAngles = new Vector3 (0, 0, angle);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {
    //the time
    public decimal time;
    //A simple timer for the player
    private Text timerText;

	// Use this for initialization
	void Awake () {
        timerText = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        time = System.Math.Round((decimal)Time.timeSinceLevelLoad, 2);//get the time for the run
        timerText.text = time.ToString();//set the time of the run
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerName : MonoBehaviour {
    private InputField input;//the place where a user enters a name

	// Use this for initialization
	void Start () {
        input = GetComponent<InputField>();
        //listen for changes
        input.onValueChanged.AddListener(SavePlayerName);
        string saveName = PlayerPrefs.GetString("PlayerName");//name from previous session
        //check if name doesnt exist
        if (!string.IsNullOrEmpty(saveName))
        {
            input.text = saveName;
            GameManager.instance.playerName = saveName;
        }
	}
	
    //save the player name when the input text is changed
    private void SavePlayerName(string playerName)
    {
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();
        GameManager.instance.playerName = playerName;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    //player info
    public string playerName;
    //this is the manager for the game, it is singleton
    public static GameManager instance;
    //a button prefab for each level found
    public GameObject buttonPrefab;

    //the name of the currently selected level
    private string selectedLevel;
    //the private coroutine to use
    private Coroutine inst;
	// Use this for initialization
	void Awake () {
		if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != this)
        {
            Destroy(gameObject);//there can only be one
            return;
        }
	}
    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoad;//add additional things to do when scene loads
        //find all json files
        DiscoverLevels();
    }
    private void Update()
    {
        //return to main menu with esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Menu");
        }
    }

    //do stuff when new scene loads
    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        //load specific level
        if (!string.IsNullOrEmpty(selectedLevel) && scene.name == "Level_template")
        {
            Debug.Log("Loading level data for: " + selectedLevel);
            LoadLevelContent();
            DisplayPreviousTimes();
        }
        if(scene.name == "Menu")
        {
            DiscoverLevels();
            if(inst != null)
            {
                StopCoroutine(inst);//stop the level from restarting
            }

        }
    }

    public void RestartLevel(float delay)
    {
        inst = StartCoroutine(RestartLevelDelay(delay));
    }
    private IEnumerator RestartLevelDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("Level_template");
    }
    //get previous fastest times
    public List<PlayerTime> LoadPreviousTimes()
    {
        //try opening the score file
        try
        {
            var levelName = Path.GetFileName(selectedLevel);
            var scoreFile = Application.persistentDataPath + "/" + playerName + "_" + levelName + "_times.dat";
            //get the times from the file
            using (var stream = File.Open(scoreFile, FileMode.Open))
            {
                var bin = new BinaryFormatter();//get formatter
                var times = (List<PlayerTime>)bin.Deserialize(stream);//get the player times
                return times;
            }

        }
        catch(Exception ex)
        {
            Debug.Log("Could not find previous times for " + playerName + ". Exception: " + ex.Message);
            return new List<PlayerTime>();
        }
    }
    //save new time to file
    public void SaveTime(decimal time)
    {
        //get previous times
        var times = LoadPreviousTimes();
        PlayerTime newTime = new PlayerTime
        {
            //set the new time
            time = time,
            entryDate = DateTime.Now
        };
        //open file to load times
        var bin = new BinaryFormatter();
        var levelName = Path.GetFileName(selectedLevel);
        string scoreFile = Application.persistentDataPath + "/" + playerName + "_"+ levelName + "_times.dat";
        Debug.Log("Saving to: " + scoreFile);
        //create file if it doesnt exist yet; should probably catch io exception here
        using (var stream = File.Open(scoreFile, FileMode.Create))
        {
            times.Add(newTime);
            bin.Serialize(stream, times);
        }
        
    }

    //display the previous best three times
    public void DisplayPreviousTimes()
    {

        var times = LoadPreviousTimes();
        //get top three times
        var topThree = times.OrderBy(time => time.time).Take(3);
        //get the place to display the times
        var levelName = Path.GetFileName(selectedLevel);
        if(levelName != null)
        {
            levelName = levelName.Replace(".json", "");
        }
        var displayText = GameObject.Find("PreviousTimes").GetComponent<Text>();
        //build the text
        displayText.text = levelName + "\n";
        displayText.text += "BEST TIMES\n";
        foreach(PlayerTime time in topThree)
        {
            displayText.text += time.entryDate.ToShortDateString() + ": " + time.time + "\n";
        }
    }
    //set the level name to whatever is currently selected
    private void SetLevelname(string levelFilePath)
    {
        selectedLevel = levelFilePath;
        SceneManager.LoadScene("Level_template");
    }
    //set the name but don't load the scene
    public void SetNameDontLoad(string levelFileName)
    {
        selectedLevel = levelFileName + ".json";
    }
    //find all level files
    private void DiscoverLevels()
    {
        var levelPanelRectTransform = GameObject.Find("LevelItemPanel").
            GetComponent<RectTransform>();//figure out where to display things
        var levelFiles = Directory.GetFiles(Application.dataPath, "*.json");//get all json files there
        //render the levels in the panel
        float yOffset = 0f;
        for(int i = 0; i < levelFiles.Length; ++i)
        {
            if(i == 0)
            {
                yOffset = -30f;
            }
            else
            {
                yOffset -= 60f;//move render location down
            }
            var levelFile = levelFiles[i];
            var levelName = Path.GetFileName(levelFile);
            //make new button
            GameObject levelButtonObj = (GameObject)Instantiate(buttonPrefab, Vector2.zero, Quaternion.identity);
            RectTransform buttonRect = levelButtonObj.GetComponent<RectTransform>();
            //set the button parent to the panel
            buttonRect.SetParent(levelPanelRectTransform, true);
            //set the position
            buttonRect.anchoredPosition = new Vector2(0f, yOffset);
            var levelButtonText = levelButtonObj.transform.GetChild(0).GetComponent<Text>();//this should be the text of the button
            levelButtonText.text = levelName;
            //add a listener to figure out if we need to load the level
            var levelButton = levelButtonObj.GetComponent<Button>();
            levelButton.onClick.AddListener(delegate { SetLevelname(levelFile); });
            //expand the panel to accomodate levels
            levelPanelRectTransform.sizeDelta = new Vector2(levelPanelRectTransform.sizeDelta.x, 60f * i);
        }
        //clamp the limit to the panel
        levelPanelRectTransform.offsetMax = new Vector2(levelPanelRectTransform.offsetMax.x, 0f);
    }
    //need to be able to load level content
    public void LoadLevelContent()
    {
        //get the template's level root
        var existingLevelRoot = GameObject.Find("Level");
        Destroy(existingLevelRoot);
        GameObject levelRoot = new GameObject("Level");//clean house prior to load
        //get the level info
        var levelFileJsonContent = File.ReadAllText(selectedLevel);
        var levelData = JsonUtility.FromJson<LevelDataRepresentation>(levelFileJsonContent);
        //loop through the items
        foreach(var li in levelData.levelsItems)
        {
            var levelResource = Resources.Load("Prefabs/" + li.prefabName);
            if(levelResource == null)
            {
                Debug.Log("Could not find prefab: " + li.prefabName);
            }
            GameObject levelObj = (GameObject)Instantiate(levelResource, li.position, Quaternion.identity);
            //get the sprite 
            var objSprite = levelObj.GetComponent<SpriteRenderer>();
            if(objSprite != null)
            {
                objSprite.sortingOrder = li.spriteOrder;
                objSprite.sortingLayerName = li.spriteLayer;
                objSprite.color = li.spriteColor;
            }
            //set the levelroot as parent
            levelObj.transform.SetParent(transform.parent, false);
            levelObj.transform.position = li.position;
            levelObj.transform.rotation = Quaternion.Euler(li.rotation.x, li.rotation.y, li.rotation.z);
            levelObj.transform.localScale = li.scale;
        }
        //set soyboys position
        GameObject soyBoy = GameObject.Find("SoyBoy");
        soyBoy.transform.position = levelData.playerStartLocation;
        //set camera position
        Camera.main.transform.position = new Vector3(soyBoy.transform.position.x, 
                soyBoy.transform.position.y, Camera.main.transform.position.z);
        CameraLerpToTransform cameraSettings = FindObjectOfType<CameraLerpToTransform>();
        if(cameraSettings != null)
        {
            cameraSettings.minX = levelData.cameraSettings.minX;
            cameraSettings.maxX = levelData.cameraSettings.maxX;
            cameraSettings.minY = levelData.cameraSettings.minY;
            cameraSettings.maxY = levelData.cameraSettings.maxY;
            cameraSettings.trackingSpeed = levelData.cameraSettings.trackingSpeed;
            cameraSettings.cameraZDepth = levelData.cameraSettings.cameraZDepth;
            cameraSettings.camTarget = GameObject.Find(levelData.cameraSettings.camTarget).transform;
        }
    }
    //quit game on exit button press
    public void QuitButton()
    {
        Application.Quit();
        Debug.Log("Quitting...");
    }


}

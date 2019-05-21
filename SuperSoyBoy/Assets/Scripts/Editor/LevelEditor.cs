using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
//custom level editor

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor {
    public override void OnInspectorGUI()
    {
        //change the way the inspectro behaves
        DrawDefaultInspector();
        //add a save level button
        if(GUILayout.Button("Save Level"))
        {
            //get level info, make sure level root is at origin
            Level level = (Level)target;
            level.transform.position = Vector3.zero;
            level.transform.rotation = Quaternion.identity;
            var levelRoot = GameObject.Find("Level");//get the root
                                                     //setup the structure to save info
            var ldr = new LevelDataRepresentation();
            var levelItems = new List<LevelItemRepresentation>();
            //loop through the items to get the needed info
            foreach(Transform t in levelRoot.transform)
            {
                //get a reference to sprite renderer
                SpriteRenderer sr = t.GetComponent<SpriteRenderer>();
                LevelItemRepresentation li = new LevelItemRepresentation()
                {
                    position = t.position,
                    rotation = t.rotation.eulerAngles,
                    scale = t.localScale
                };
                //get the prefab name, try to hedge for spaces in duplicates
                if(t.name.Contains(" "))
                {
                    //get the name before the space
                    li.prefabName = t.name.Substring(0, t.name.IndexOf(" ", System.StringComparison.CurrentCulture));//very progressive
                }
                else if (t.name.Contains("(Clone)"))
                {
                    //make sure we aren't saving clones
                    li.prefabName = t.name.Substring(0, t.name.IndexOf("(", System.StringComparison.CurrentCulture));
                }
                else
                {
                    //if no spaces just get name
                    li.prefabName = t.name;
                }
                //get the sprite info if there is a sprite renderer
                if(sr != null)
                {
                    li.spriteColor = sr.color;
                    li.spriteLayer = sr.sortingLayerName;
                    li.spriteOrder = sr.sortingOrder;
                }
                //add item to list
                levelItems.Add(li);
            }
            //now we should have all the levelitems, we just need the player position and cam settings
            ldr.levelsItems = levelItems.ToArray();
            //player info
            ldr.playerStartLocation = GameObject.Find("SoyBoy").transform.position;
            //camera settings
            var currentCamSettings = FindObjectOfType<CameraLerpToTransform>();
            if(currentCamSettings != null)
            {
                ldr.cameraSettings = new CameraSettingsRepresentation()
                {
                    minX = currentCamSettings.minX,
                    maxX = currentCamSettings.maxX,
                    minY = currentCamSettings.minY,
                    maxY = currentCamSettings.maxY,
                    trackingSpeed = currentCamSettings.trackingSpeed,
                    cameraZDepth = currentCamSettings.cameraZDepth,
                    camTarget = currentCamSettings.camTarget.name
                };
            }
            //finally save the level to file
            var levelDataToJSON = JsonUtility.ToJson(ldr);
            var savePath = System.IO.Path.Combine(Application.dataPath, level.levelName + ".json");
            //write out
            System.IO.File.WriteAllText(savePath, levelDataToJSON);
            Debug.Log("Level saved to " + savePath);
        }
        //Add a function to load a previosuly saved level
        if(GUILayout.Button("Load Level"))
        {
            //set the name of the file to load
            Level level = (Level)target;
            //get a string for the file load to reload it in the new Level script
            string loadedLevelName = level.SavedLevelName;
            //since there isnt a game manager in the levl template scene we load manually
            string fileName = Application.dataPath + "/" + level.SavedLevelName + ".json";
            //get the template's level root
            var existingLevelRoot = GameObject.Find("Level");
            DestroyImmediate(existingLevelRoot);
            GameObject levelRoot = new GameObject("Level");//clean house prior to load
            //add level to new levelRoot
            Level newLevel = levelRoot.AddComponent<Level>();
            newLevel.SavedLevelName = loadedLevelName;
            newLevel.levelName = loadedLevelName;
            //get the level info
            var levelFileJsonContent = File.ReadAllText(fileName);
            var levelData = JsonUtility.FromJson<LevelDataRepresentation>(levelFileJsonContent);
            //loop through the items
            foreach (var li in levelData.levelsItems)
            {
                //make sure the prefab name doesnt contain (Clone)
                if (li.prefabName.Contains("(Clone)"))
                {
                    li.prefabName = li.prefabName.Substring(0, li.prefabName.IndexOf("(", System.StringComparison.CurrentCulture));
                }
                var levelResource = Resources.Load("Prefabs/" + li.prefabName);
                if (levelResource == null)
                {
                    Debug.Log("Could not find prefab: " + li.prefabName);
                }
                GameObject levelObj = (GameObject)Instantiate(levelResource, li.position, Quaternion.identity);
                //get the sprite 
                var objSprite = levelObj.GetComponent<SpriteRenderer>();
                if (objSprite != null)
                {
                    objSprite.sortingOrder = li.spriteOrder;
                    objSprite.sortingLayerName = li.spriteLayer;
                    objSprite.color = li.spriteColor;
                }
                //set the levelroot as parent
                levelObj.transform.SetParent(levelRoot.transform, false);
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
            if (cameraSettings != null)
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

    }
}

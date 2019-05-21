using System;
using UnityEngine;

[Serializable]
public class LevelDataRepresentation {
    //representation of a level in SSB
    public Vector2 playerStartLocation;
    public CameraSettingsRepresentation cameraSettings;
    public LevelItemRepresentation[] levelsItems;//the game objects
}

using System;

[Serializable]
public class CameraSettingsRepresentation {
    //the camera settings for a level in SSB
    public float trackingSpeed;
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    public float cameraZDepth;
    public string camTarget;
}

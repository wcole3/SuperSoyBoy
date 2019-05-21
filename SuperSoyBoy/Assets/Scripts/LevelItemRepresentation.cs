using System;
using UnityEngine;

[Serializable]
public class LevelItemRepresentation {
    //a base game object in SSB
    public Vector3 rotation;
    public Vector2 position;
    public Vector3 scale;
    public string prefabName;
    public int spriteOrder;
    public string spriteLayer;
    public Color spriteColor;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLerpToTransform : MonoBehaviour {
    //make the camera smoothly follow the player
    public Transform camTarget;
    public float trackingSpeed;
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    public float cameraZDepth = -10f;

    private void FixedUpdate()
    {
        if(camTarget != null)
        {
            var newPos = Vector2.Lerp(transform.position, camTarget.position, Time.deltaTime * trackingSpeed);
            //rmbr the camera is in 3D
            var camPos = new Vector2(newPos.x, newPos.y);
            //make sure the cam is in range
            var clampX = Mathf.Clamp(camPos.x, minX, maxX);
            var clampY = Mathf.Clamp(camPos.y, minY, maxY);
            transform.position = new Vector3(clampX, clampY, cameraZDepth);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSaw : MonoBehaviour {
    //Rotate the buzzsaw
    public float rotationsPerMinute = 640f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(0, 0, rotationsPerMinute * Time.deltaTime, Space.Self);
	}
}

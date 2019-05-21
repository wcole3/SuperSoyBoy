using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour {

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(GameManager.instance.QuitButton);
    }

}

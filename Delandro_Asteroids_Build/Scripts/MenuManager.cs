using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
	public static bool endless;
    // Start is called before the first frame update
    void Start()
    {
		//Makes sure other classes
		DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
		//set game mode to points
		if (Input.GetKeyDown(KeyCode.P)){
			endless = false;
			SceneManager.LoadScene(1);
		}
		//set game mode to endless
		if (Input.GetKeyDown(KeyCode.E)) {
			endless = true;
			SceneManager.LoadScene(1);
		}
	}

	public bool getEndless() {
		return endless;
	}
}

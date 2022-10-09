using UnityEngine;
using System.Collections;
using GlobalInfo;

public class LoadingBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
        MainGameInfo.levelCompleted = false;
        MainGameInfo.levelFailed = false;
		MainGameInfo.patriotFlag = false;
		Application.LoadLevel (3);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

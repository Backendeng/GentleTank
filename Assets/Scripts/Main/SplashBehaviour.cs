using UnityEngine;
using System.Collections;
using GlobalInfo;

public class SplashBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GlobalInfo.MainGameInfo.maxLevelIndex = PlayerPrefs.GetInt ("MaxLevel");
		GlobalInfo.MainGameInfo.maxLevelIndex = Mathf.Clamp (GlobalInfo.MainGameInfo.maxLevelIndex, 0, 6);
		Application.LoadLevel (1);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;

public class MiniMapArrowBehaviour : MonoBehaviour {
	private GameObject mainPlayer;
	// Use this for initialization
	void Start () {
		mainPlayer = GameObject.Find ("MainPlayer");
		transform.position = new Vector3 (mainPlayer.transform.position.x, transform.position.y, mainPlayer.transform.position.z);
		transform.rotation = new Quaternion (0, mainPlayer.transform.rotation.y, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3 (mainPlayer.transform.position.x, transform.position.y, mainPlayer.transform.position.z);
		transform.rotation = mainPlayer.transform.rotation;

	}
}

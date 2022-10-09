using UnityEngine;
using System.Collections;

public class AIDestroyedTurret : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody>().AddForceAtPosition (Vector3.up * 100f, transform.position);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

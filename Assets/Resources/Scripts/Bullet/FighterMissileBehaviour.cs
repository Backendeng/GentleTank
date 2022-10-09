using UnityEngine;
using System.Collections;
using GlobalInfo;

public class FighterMissileBehaviour : MonoBehaviour {
	private bool calcFlag = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(calcFlag || Time.timeScale == 0){
			return;
		}
		RaycastHit hitInfo;
		float tf = Time.deltaTime;
		if (tf == 0) {
			tf = 0.0001f;
		}
		if (Physics.Raycast (transform.position, transform.forward, out hitInfo, tf * 8000f)) {
			hitInfo.collider.gameObject.SendMessage("OnHealthCalc",SendMessageOptions.DontRequireReceiver);
			calcFlag = true;
		}
	}
}

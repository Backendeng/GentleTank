using UnityEngine;
using System.Collections;

public class AutoDestruct : MonoBehaviour {
	public float lifeCycle = 3.0f;
	// Use this for initialization
	void Start() {
		StartCoroutine("SelfDestroy");
	}	
	
	IEnumerator SelfDestroy(){
		yield return new WaitForSeconds(lifeCycle);
		Destroy(gameObject);
	}
}

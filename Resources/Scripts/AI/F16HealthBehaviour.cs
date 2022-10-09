using UnityEngine;
using System.Collections;
using GlobalInfo;

public class F16HealthBehaviour : MonoBehaviour {
	public GameObject destroyed_F16;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Damage(float val){
		StartCoroutine (DeathAction ());
	}

	IEnumerator DeathAction(){
		GlobalInfo.MainGameInfo.score += 1;
		GameObject gb = (GameObject)Instantiate (destroyed_F16, transform.position, Quaternion.identity);
		yield return new WaitForSeconds(0.1f);
		Destroy (this.gameObject);
	}
}

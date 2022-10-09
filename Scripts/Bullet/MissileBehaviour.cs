using UnityEngine;
using System.Collections;
using GlobalInfo;

public class MissileBehaviour : MonoBehaviour {
	private Transform mainPlayer;

	public GameObject explosion;
	public float jerky = 0.5f;
	// Use this for initialization
	void Start () {	
		mainPlayer = GameObject.Find ("MainPlayer").transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (GlobalInfo.MainGameInfo.pauseFlag) {
			return;
		}
		transform.Translate (Vector3.forward * Time.deltaTime * 20f, Space.Self);
		transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (mainPlayer.position - transform.position), Time.deltaTime * jerky);
	}

	void OnCollisionEnter(Collision col){
		GameObject gb = (GameObject)Instantiate (explosion, col.contacts [0].point, transform.rotation);
		if (col.collider.CompareTag ("Player")) {
			mainPlayer.SendMessage("OnDamage",10f,SendMessageOptions.DontRequireReceiver);
		}
		Destroy (this.gameObject);
		Resources.UnloadUnusedAssets ();
	}
}

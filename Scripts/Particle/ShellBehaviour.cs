using UnityEngine;
using System.Collections;
using GlobalInfo;

public class ShellBehaviour : MonoBehaviour {
	public bool isPlayerShell = false;
	public float speed = 1000f;
	public GameObject detonator_base;
	public GameObject detonator_crazySparks;
	// Use this for initialization
	void Start () {
//		rigidbody.AddForce (transform.forward *  speed);
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	void SetFlag(bool bFlag){
		isPlayerShell = bFlag;
	}
	void FixedUpdate(){
		if (GlobalInfo.MainGameInfo.pauseFlag) {
			return;
		}
		GetComponent<Rigidbody>().velocity = transform.forward * Time.fixedDeltaTime * speed;
	}

	void OnCollisionEnter(Collision col){
		if (isPlayerShell) {
			Collider[] objects = Physics.OverlapSphere(col.contacts[0].point,10f);
			foreach(Collider cols in objects){
				if(!cols.gameObject.CompareTag("Player") && !cols.gameObject.CompareTag("Terrain") && !cols.gameObject.CompareTag("Border")){
					cols.gameObject.SendMessage("OnDamage",10f,SendMessageOptions.DontRequireReceiver);
				}
			}
			if(!col.gameObject.CompareTag("Border")){
			 	GameObject exlosion = (GameObject)Instantiate (detonator_base, col.contacts[0].point, Quaternion.identity);
			}
		}else{
			Collider[] objects = Physics.OverlapSphere(col.contacts[0].point,3f);
			foreach(Collider cols in objects){
				if(cols.gameObject.CompareTag("Player")){
					cols.gameObject.SendMessage("OnDamage",10f,SendMessageOptions.DontRequireReceiver);
				}

			}
			GameObject exlosion = (GameObject)Instantiate (detonator_crazySparks, col.contacts[0].point, Quaternion.identity);
		}
		Destroy (this.gameObject);
		Resources.UnloadUnusedAssets();
		
	}
}

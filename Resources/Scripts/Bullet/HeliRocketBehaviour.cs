using UnityEngine;
using System.Collections;
using GlobalInfo;

public class HeliRocketBehaviour : MonoBehaviour {
	private bool bflag = false;
	public GameObject explosionParticle;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (transform.forward * Time.deltaTime * 100f, Space.World);	

		RaycastHit hitInfo;
		if (Time.timeScale == 0) {
			return;
		}
		if (bflag) {
			return;
		}
		float tf = Time.deltaTime;
		if (tf == 0) {
			tf = 0.0001f;
		}
		if (Physics.Raycast (transform.position, transform.forward, out hitInfo, tf * 100f)) {
			if(hitInfo.collider.CompareTag("Player")){
				bflag = true;
				hitInfo.collider.gameObject.SendMessage("OnDamage",2f,SendMessageOptions.DontRequireReceiver);
  				GameObject gb = (GameObject)Instantiate(explosionParticle,hitInfo.point,Quaternion.identity);
				Destroy (this.gameObject);
			}else if(hitInfo.collider.CompareTag("Terrain")){
				bflag = true;
				GameObject gb = (GameObject)Instantiate(explosionParticle,hitInfo.point,Quaternion.identity);
				Destroy (this.gameObject,0.2f);
			}

		}
	}
}

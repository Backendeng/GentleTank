using UnityEngine;
using System.Collections;
using GlobalInfo;

public class SoldierBehaviour : MonoBehaviour {
	private Transform mainPlayer;
	private float shotTimeCounter = 0;
	private float originHealth;

	public GameObject missile;
	public float shotTime = 3;
	public float health = 9f;
	public Texture2D healthBar;
	public Transform cannonSP;
	public AIType currentType; 
	public GameObject explosionFlame;
	public GameObject death;
	// Use this for initialization
	void Start () {
		mainPlayer = GameObject.Find ("MainPlayer").transform;
		originHealth = health;
	}
	
	// Update is called once per frame
	void Update () {
		switch (currentType) {
			case AIType.Idle:
				break;
			case AIType.Aggressive:
				break;
			case AIType.Guarder:
				//When Player In shot Range
				if(Vector3.Distance(transform.position,mainPlayer.position) < 50f){
					Vector3 vt = new Vector3(mainPlayer.position.x,transform.position.y,mainPlayer.position.z);
					transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(vt - transform.position),Time.deltaTime);
					shotTimeCounter += Time.deltaTime;
					if(shotTimeCounter >= shotTime){
						RaycastHit hitInfo;	
						if(Physics.Raycast(cannonSP.position,(mainPlayer.position - cannonSP.position).normalized,out hitInfo,Mathf.Infinity)){
							//When RPG can see player
							if(!hitInfo.collider.CompareTag("Terrain")){
								GameObject shell = (GameObject)Instantiate(missile,cannonSP.position,cannonSP.rotation);
								GameObject flame = (GameObject)Instantiate(explosionFlame,cannonSP.position,cannonSP.rotation);
								shotTimeCounter = 0;
							}
						}
					}
				}
				break;
			case AIType.Patrol:
				break;
			default:
				break;
		}
	}

	void OnDamage(float val){
		health -= val;
		if(health < 0){
			GlobalInfo.MainGameInfo.score += 10f;
			GameObject gb = (GameObject)Instantiate(death,transform.position,transform.rotation);
			Destroy(this.gameObject);
			Resources.UnloadUnusedAssets();
		}
	}

	void OnCollisionEnter(Collision col){
		GameObject gb = (GameObject)Instantiate(death,transform.position,transform.rotation);
		if(col.collider.CompareTag("Player")){
			GlobalInfo.MainGameInfo.score += 10f;
			GameObject deathBody = (GameObject)Instantiate(death,transform.position,transform.rotation);
			Destroy (this.gameObject);
			Resources.UnloadUnusedAssets();
		}
	}
	void OnGUI(){
		if (GlobalInfo.MainGameInfo.pauseFlag) {
			return;
		}
		if(Vector3.Dot (Camera.main.transform.forward, (transform.position - Camera.main.transform.position).normalized) > 0.7f){
			RaycastHit hitInfo;
			if(!Physics.Raycast(cannonSP.position,(Camera.main.transform.position - transform.position).normalized,out hitInfo,(Camera.main.transform.position - transform.position).magnitude)){
				Vector3 vt = Camera.main.WorldToScreenPoint(transform.position);
				GUI.DrawTexture (new Rect (vt.x - 20f, Screen.height - vt.y - 15f, 40f * (health / originHealth), 4f), healthBar);
			}
		}
	}
}

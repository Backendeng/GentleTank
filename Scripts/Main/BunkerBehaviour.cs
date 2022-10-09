using UnityEngine;
using System.Collections;
using GlobalInfo;

public class BunkerBehaviour : MonoBehaviour {
	private GameObject mainPlayer;
	private float originHealth = 0;
	private int interpolation = 0;
	private bool aggressiveFlag = false;
	private Vector3 offset = Vector3.zero;

	public enum BunkerType{
		Idle = 0,
		Aggressive = 1,
	}
	public BunkerType currentType = BunkerType.Idle;
	public float randomOffset = 2f;
	public float health = 100f;
	public float sightAngle = 45f;
	public float shotRange = 40f;
	public Transform cannon;
	public Transform cannonSP;
	public Texture2D healthBar;
	public GameObject shotFlame;
	public GameObject guardShell;
	public GameObject explosion;
	public GameObject ruins;

	float currentAngle = 0;
	float currentDistance = 0;
	float shotTimeCounter = 0;
	// Use this for initialization
	void Start () {
		offset = new Vector3 (Random.Range (-randomOffset, randomOffset), 0, Random.Range (-randomOffset, randomOffset));
		mainPlayer = GameObject.Find ("MainPlayer");	
		originHealth = health;
	}

	// Update is called once per frame
	void Update () {
		if (GlobalInfo.MainGameInfo.pauseFlag) {
			return;
		}

		switch (currentType) {
			case BunkerType.Idle:
				break;
			case BunkerType.Aggressive:
				shotTimeCounter += Time.deltaTime;
				currentDistance = Vector3.Distance(transform.position,mainPlayer.transform.position);
				if(currentDistance < shotRange){
					if(Vector3.Angle(transform.forward,(mainPlayer.transform.position - transform.position).normalized) < sightAngle){
						aggressiveFlag = true;
					}
					Vector3 vt = new Vector3(mainPlayer.transform.position.x,cannon.transform.position.y,mainPlayer.transform.position.z);
					cannon.transform.rotation = Quaternion.Slerp(cannon.transform.rotation,Quaternion.LookRotation(vt - cannon.transform.position),Time.deltaTime);
					//When Enemy is in my range and ready to fire	
					if(shotTimeCounter >= 10f && aggressiveFlag){
						RaycastHit hitInfo;
						if(Physics.Raycast(transform.position,(mainPlayer.transform.position - transform.position).normalized,out hitInfo,Mathf.Infinity)){
							if(hitInfo.collider.CompareTag("Player")){
								shotTimeCounter = 0;
								aggressiveFlag = false;
								GameObject flame = (GameObject)Instantiate(shotFlame,cannonSP.position,cannonSP.rotation);	
								GameObject shell = (GameObject)Instantiate(guardShell,cannonSP.position,cannonSP.rotation);
								shell.SendMessage("SetFlag",false,SendMessageOptions.DontRequireReceiver);
								shell.transform.LookAt(mainPlayer.transform.position + offset);
								offset = new Vector3(Random.Range(-randomOffset,randomOffset),0,Random.Range(-randomOffset,randomOffset));
							}
						}
					}
				}
				break;
			default:
				break;
		}
	}

	void OnDamage(float val){
		health -= val;

		if (health < 0) {
			GlobalInfo.MainGameInfo.score += 30f;
			GlobalInfo.MainGameInfo.enemyProgress -= 1f;
			GameObject selfExplosion = (GameObject)Instantiate(explosion,transform.position,Quaternion.identity);
			GameObject ruin = (GameObject)Instantiate(ruins,transform.position,Quaternion.identity);
			Destroy (this.gameObject);
			Resources.UnloadUnusedAssets();
		}
	}

	void OnGUI(){
		if (GlobalInfo.MainGameInfo.pauseFlag) {
			return;
		}
		interpolation ++;
		if (interpolation >= 0) {
			interpolation = 0;
			if(Vector3.Dot (Camera.main.transform.forward, (transform.position - Camera.main.transform.position).normalized) > 0.7f){
				//RaycastHit hitInfo;
				//if(Physics.Raycast(transform.position,(Camera.main.transform.position - transform.position).normalized,out hitInfo,(Camera.main.transform.position - transform.position).magnitude)){
				//	if(!hitInfo.collider.CompareTag("Terrain")){
						Vector3 vt = Camera.main.WorldToScreenPoint(transform.position);
						GUI.DrawTexture (new Rect (vt.x - 25f, Screen.height - vt.y - 30f, 50f * (health / originHealth), 4f), healthBar);
				//	}
				//}
			}
		}
	}
}

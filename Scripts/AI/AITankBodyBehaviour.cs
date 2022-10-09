using UnityEngine;
using System.Collections;
using GlobalInfo;

public class AITankBodyBehaviour : MonoBehaviour {
	private UnityEngine.AI.NavMeshAgent myAgent;
	private ParticleEmitter flameEmitter;
	private Transform mainPlayer;
	private float originHealth;
	private Vector3 currentWaypoint;
	private int currentIndex = 0;

	public Texture2D healthBar;
	public Transform cannonSP;
	public GameObject destroyedTank;
	public GameObject tank_Explosion;
	public GameObject smokeParticle;
	public GameObject flameParticle;
	public float health = 200f;
	public WheelCollider[] wheelColliders;
	public AIType currentType;
	public Transform centerOfMass;
	public Transform[] waypoints;
	// Use this for initialization
	void Start () {
		originHealth = health;
		mainPlayer = GameObject.Find ("MainPlayer").transform;
		foreach (WheelCollider col in wheelColliders) {
			col.brakeTorque = Mathf.Infinity;
		}
		flameEmitter = flameParticle.GetComponent<ParticleEmitter> ();
		GetComponent<Rigidbody>().centerOfMass = centerOfMass.localPosition;
		if(currentType == AIType.Aggressive){
			currentWaypoint = waypoints [currentIndex].position;
		}
	}
	
	// Update is called once per frame
	void Update () {
		switch (currentType) {
			case AIType.Idle:
				break;
			case AIType.Patrol:
				break;
			case AIType.Aggressive:
				if(Vector3.Distance(transform.position,mainPlayer.position) > 40f){
					transform.Translate (transform.forward * Time.deltaTime * 1.5f,Space.World);
					//		turret.localRotation = Quaternion.Slerp (turret.localRotation, Quaternion.identity, Time.deltaTime);
					Vector3 lookAtVector = new Vector3 (currentWaypoint.x, transform.position.y, currentWaypoint.z);
					float angle = Vector3.Angle (transform.forward, (lookAtVector - transform.position).normalized);
					if (angle > 10f) {
						if(Vector3.Angle(transform.right,(lookAtVector - transform.position).normalized) < 90f){
							transform.RotateAround(transform.position,transform.up,Time.deltaTime * 15f);
						}else{
							transform.RotateAround(transform.position,transform.up,-Time.deltaTime * 15f);
						}
					}
					
					//Within Next Waypoint Range
					if (Vector3.Distance (transform.position, currentWaypoint) < 3f){
						currentIndex ++;
						if(currentIndex >= waypoints.Length){
							currentIndex = 0;
						}
						currentWaypoint = waypoints[currentIndex].position;
					}
					
				}
				break;
			case AIType.Guarder:
				break;
			default:
				break;
		}

		flameEmitter.minSize -= Time.deltaTime * 0.1f;
		flameEmitter.minSize = Mathf.Clamp (flameEmitter.minSize, 0, 1f);
		flameEmitter.maxSize -= Time.deltaTime * 0.1f;
		flameEmitter.maxSize = Mathf.Clamp (flameEmitter.maxSize, 0, 1.5f);
	}
	
	public void OnDamage(float val){
		if (health < 0) {
			return;
		}
		transform.Find ("Head").SendMessage ("OnDamage", SendMessageOptions.DontRequireReceiver);
		health -= val;
		smokeParticle.SetActive (true);
		flameParticle.SetActive (true);
		flameEmitter.minSize = 1f;
		flameEmitter.maxSize = 1.5f;
		if (health < 0) {
			GameObject destroyed = (GameObject)Instantiate(destroyedTank,transform.position,transform.rotation);
			GameObject selfExplosion = (GameObject)Instantiate(tank_Explosion,transform.position,Quaternion.identity);
			GlobalInfo.MainGameInfo.score += 50f;
			GlobalInfo.MainGameInfo.enemyProgress -= 1f;
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
				GUI.DrawTexture (new Rect (vt.x - 20f, Screen.height - vt.y - 25f, 40f * (health / originHealth), 4f), healthBar);
			}
		}
	}
}

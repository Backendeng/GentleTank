using UnityEngine;
using System.Collections;
using GlobalInfo;

public class PlayerTankBehaviour : MonoBehaviour {
	private int leftSkidmarkIndex = 1;
	private int rightSkidmarkIndex = 1;
	private float skidMarkTimeCounter = 0;
	private GameObject gameManager;

	public GameObject smokeParticle;
	public GameObject flameParticle;
	public GameObject patriotMissile;
	public float bodyRotInterval;
	public float skidmarkInterval = 0.5f;
	public Skidmarks skidmarks;
	public Transform leftSkidmarkPos;
	public Transform rightSkidmarkPos;
	public Transform body;
	public Transform cannon;
	public Transform turret;
	public Material attacked_Mat;
	public Transform centerOfMass;
	public Transform patriotCamRotPos;
	public Transform patriotMissilePos;

	ParticleEmitter flameEmitter;
	// Use this for initialization
	void Start () {
		flameEmitter = flameParticle.GetComponent<ParticleEmitter> ();
		gameManager = GameObject.Find("GameManager");
	}
	
	// Update is called once per frame
	void Update () {
		if (GlobalInfo.MainGameInfo.pauseFlag) {
			return;
		}
		transform.RotateAroundLocal (Vector3.up, Time.deltaTime * GlobalInfo.MainGameInfo.str * bodyRotInterval);
		skidMarkTimeCounter += Time.deltaTime;
		if (skidMarkTimeCounter >= skidmarkInterval) {
			CreateSkidMark();
			skidMarkTimeCounter = 0 ;
		}

		flameEmitter.minSize -= Time.deltaTime * 0.1f;
		flameEmitter.minSize = Mathf.Clamp (flameEmitter.minSize, 0, 1f);
		flameEmitter.maxSize -= Time.deltaTime * 0.1f;
		flameEmitter.maxSize = Mathf.Clamp (flameEmitter.maxSize, 0, 1.5f);
 	}

	//When Player Tank Attacked
	public void OnDamage(float val){
		if (GlobalInfo.MainGameInfo.health < 70f) {
			body.GetComponent<MeshRenderer>().material = attacked_Mat;
			turret.GetComponent<MeshRenderer>().material = attacked_Mat;
			cannon.GetComponent<MeshRenderer>().material = attacked_Mat;
		}
		GlobalInfo.MainGameInfo.health -= val;
		if(GlobalInfo.MainGameInfo.health < 50f){
			smokeParticle.SetActive (true);
		}
		flameParticle.SetActive (true);
		flameEmitter.minSize = 1f;
		flameEmitter.maxSize = 1.5f;
//		if(MainGameInfo.currentMission < 5){
//			rigidbody.AddForceAtPosition (transform.up * 2000f, centerOfMass.position, ForceMode.Impulse);
//		}
		gameManager.SendMessage ("OnDamage", SendMessageOptions.DontRequireReceiver);
	}

	void CreateSkidMark(){
		RaycastHit leftHit;
		if(Physics.Raycast(leftSkidmarkPos.position,-leftSkidmarkPos.up,out leftHit))
		{
			leftSkidmarkIndex = skidmarks.AddSkidMark(leftHit.point,leftHit.normal,1.0f,leftSkidmarkIndex);
		}
		RaycastHit rightHit;
		if(Physics.Raycast(rightSkidmarkPos.position,-rightSkidmarkPos.up,out rightHit))
		{
			rightSkidmarkIndex = skidmarks.AddSkidMark(rightHit.point,rightHit.normal,1.0f,rightSkidmarkIndex);
		}
	}

	void OnShot(){
		if(MainGameInfo.patriotFlag){
			GameObject gb = (GameObject)Instantiate (patriotMissile, patriotMissilePos.position, patriotMissilePos.rotation);
		}
	}
}
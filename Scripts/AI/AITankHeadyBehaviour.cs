using UnityEngine;
using System.Collections;
using GlobalInfo;

public class AITankHeadyBehaviour : MonoBehaviour {
	private bool h_Aimed = false;
	private bool v_Aimed = false;
	private float shotTimeCounter = 0;
	private Transform mainPlayer;
	private Vector3 target;
	private Vector3 offset = Vector3.zero;

	public float shotRange = 40f;
	public float rotInterpolation = 10f;
	public float shotPeroid = 5f;
	public float randomOffset = 2f;
	public Transform canonSP;
	public Transform canon;
	public AIType currentType;
	public Transform gyroBase;
	public Transform h_Gyro;
	public Transform v_Gyro;
	public Transform gyro;
	public GameObject shotFlame;
	public GameObject tankShell;
	public GameObject selfExplosion;

	Vector3 tpVector = Vector3.zero;
	// Use this for initialization
	void Start () {
		mainPlayer = GameObject.Find ("MainPlayer").transform;
		offset = new Vector3 (Random.Range (-randomOffset, randomOffset), 0, Random.Range (-randomOffset, randomOffset));
	}
	
	// Update is called once per frame
	void Update () {
		float ang, ang1,tmp1;
		Vector3 tmp;
		switch (currentType) {
			case AIType.Idle:
				
				target = mainPlayer.position + offset;
				h_Aimed = false;v_Aimed = false;
				tmp = Vector3.zero;
				tmp = target - transform.position;
				tmp.Normalize();
				
				ang = Vector3.Angle(transform.up,tmp);
				ang1 = Vector3.Angle(transform.up,canonSP.forward);
				ang = Mathf.Clamp(ang,70.0f,100.0f);
				ang1 = (ang - ang1) * Mathf.PI / 180;
				tmp1 = rotInterpolation * Time.deltaTime;
				if(Mathf.Abs(ang1) > tmp1) ang1 = Mathf.Sign(ang1) * tmp1; else v_Aimed = true;
				canon.RotateAroundLocal(Vector3.right,ang1 * Time.deltaTime * rotInterpolation * 0.5f);
				tmp = target - gyroBase.position;
				
				ang = Vector3.Angle(transform.up,tmp);
				ang = tmp.magnitude * Mathf.Cos (ang * Mathf.PI / 180);
				tmp = transform.up * ang;
				gyro.position = gyroBase.position + tmp;
				gyro.LookAt(target,transform.up);
				
				h_Gyro.position = gyroBase.position;
				h_Gyro.rotation = gyro.rotation;
				
				ang = Vector3.Angle(transform.forward,gyro.forward);
				ang = ang * Mathf.PI / 180;
				ang1 = rotInterpolation * Time.deltaTime;
				if(ang > ang1) ang = ang1; else h_Aimed = true;
				
				ang1 = Vector3.Angle(transform.right,gyro.forward);
				if(ang1 != 90.0f) {
					if(ang1 > 90.0f)ang = -ang;
					transform.RotateAroundLocal(Vector3.up,ang * Time.deltaTime * rotInterpolation);
				}		
				shotTimeCounter += Time.deltaTime;
				if(shotTimeCounter >= shotPeroid && h_Aimed && v_Aimed){
					//ready to fire; check if enemy is in my range
					float distance = Vector3.Distance(transform.position,mainPlayer.transform.position);
					if(distance < shotRange){
						RaycastHit hitInfo;	
						if(Physics.Raycast(canonSP.position,(mainPlayer.position - transform.position).normalized,out hitInfo,distance)){
							if(!hitInfo.collider.CompareTag("Terrain")){
								shotTimeCounter = 0;
								offset = new Vector3(Random.Range(-randomOffset,randomOffset),0,Random.Range(-randomOffset,randomOffset));
								GameObject shotFire = (GameObject)Instantiate(shotFlame,canonSP.position,canonSP.rotation);
								GameObject shell = (GameObject)Instantiate(tankShell,canonSP.position,canonSP.rotation);
								shell.SendMessage("SetFlag",false,SendMessageOptions.DontRequireReceiver);
							}
						}
					}
				}
				break;
			case AIType.Aggressive:
				target = mainPlayer.position + offset;
				h_Aimed = false;v_Aimed = false;
				tmp = Vector3.zero;
				tmp = target - transform.position;
				tmp.Normalize();
				
				ang = Vector3.Angle(transform.up,tmp);
				ang1 = Vector3.Angle(transform.up,canonSP.forward);
				ang = Mathf.Clamp(ang,70.0f,100.0f);
				ang1 = (ang - ang1) * Mathf.PI / 180;
				tmp1 = rotInterpolation * Time.deltaTime;
				if(Mathf.Abs(ang1) > tmp1) ang1 = Mathf.Sign(ang1) * tmp1; else v_Aimed = true;
				canon.RotateAroundLocal(Vector3.right,ang1 * Time.deltaTime * rotInterpolation * 0.5f);
				tmp = target - gyroBase.position;
				
				ang = Vector3.Angle(transform.up,tmp);
				ang = tmp.magnitude * Mathf.Cos (ang * Mathf.PI / 180);
				tmp = transform.up * ang;
				gyro.position = gyroBase.position + tmp;
				gyro.LookAt(target,transform.up);
				
				h_Gyro.position = gyroBase.position;
				h_Gyro.rotation = gyro.rotation;
				
				ang = Vector3.Angle(transform.forward,gyro.forward);
				ang = ang * Mathf.PI / 180;
				ang1 = rotInterpolation * Time.deltaTime;
				if(ang > ang1) ang = ang1; else h_Aimed = true;
				
				ang1 = Vector3.Angle(transform.right,gyro.forward);
				if(ang1 != 90.0f) {
					if(ang1 > 90.0f)ang = -ang;
					transform.RotateAroundLocal(Vector3.up,ang * Time.deltaTime * rotInterpolation);
				}		
				shotTimeCounter += Time.deltaTime;
				if(shotTimeCounter >= shotPeroid && h_Aimed && v_Aimed){
					shotTimeCounter = 0;
					offset = new Vector3(Random.Range(-randomOffset,randomOffset),0,Random.Range(-randomOffset,randomOffset));
					GameObject shotFire = (GameObject)Instantiate(shotFlame,canonSP.position,canonSP.rotation);
					GameObject shell = (GameObject)Instantiate(tankShell,canonSP.position,canonSP.rotation);
					shell.SendMessage("SetFlag",false,SendMessageOptions.DontRequireReceiver);
				}
				break;
			case AIType.Guarder:
				
				target = mainPlayer.position + offset;
				h_Aimed = false;v_Aimed = false;
				tmp = Vector3.zero;
				tmp = target - transform.position;
				tmp.Normalize();
				
				ang = Vector3.Angle(transform.up,tmp);
				ang1 = Vector3.Angle(transform.up,canonSP.forward);
				ang = Mathf.Clamp(ang,70.0f,100.0f);
				ang1 = (ang - ang1) * Mathf.PI / 180;
				tmp1 = rotInterpolation * Time.deltaTime;
				if(Mathf.Abs(ang1) > tmp1) ang1 = Mathf.Sign(ang1) * tmp1; else v_Aimed = true;
				canon.RotateAroundLocal(Vector3.right,ang1 * Time.deltaTime * rotInterpolation * 0.5f);
				tmp = target - gyroBase.position;
				
				ang = Vector3.Angle(transform.up,tmp);
				ang = tmp.magnitude * Mathf.Cos (ang * Mathf.PI / 180);
				tmp = transform.up * ang;
				gyro.position = gyroBase.position + tmp;
				gyro.LookAt(target,transform.up);
				
				h_Gyro.position = gyroBase.position;
				h_Gyro.rotation = gyro.rotation;
				
				ang = Vector3.Angle(transform.forward,gyro.forward);
				ang = ang * Mathf.PI / 180;
				ang1 = rotInterpolation * Time.deltaTime;
				if(ang > ang1) ang = ang1; else h_Aimed = true;
				
				ang1 = Vector3.Angle(transform.right,gyro.forward);
				if(ang1 != 90.0f) {
					if(ang1 > 90.0f)ang = -ang;
					transform.RotateAroundLocal(Vector3.up,ang * Time.deltaTime * rotInterpolation);
				}		
				shotTimeCounter += Time.deltaTime;
				if(shotTimeCounter >= shotPeroid && h_Aimed && v_Aimed){
					//ready to fire; check if enemy is in my range
					RaycastHit hitInfo;	
					if(Physics.Raycast(canonSP.position,(mainPlayer.position - transform.position).normalized,out hitInfo,Mathf.Infinity)){
						if(!hitInfo.collider.CompareTag("Terrain")){
							shotTimeCounter = 0;
							offset = new Vector3(Random.Range(-randomOffset,randomOffset),0,Random.Range(-randomOffset,randomOffset));
							GameObject shotFire = (GameObject)Instantiate(shotFlame,canonSP.position,canonSP.rotation);
							GameObject shell = (GameObject)Instantiate(tankShell,canonSP.position,canonSP.rotation);
							shell.SendMessage("SetFlag",false,SendMessageOptions.DontRequireReceiver);
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

	void OnDamage(){
		currentType = AIType.Guarder;
	}
}

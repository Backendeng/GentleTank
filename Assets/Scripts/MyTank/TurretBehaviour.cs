using UnityEngine;
using System.Collections;
using GlobalInfo;

public class TurretBehaviour : MonoBehaviour {
	private Transform myTransform;
	private Vector2 prevPos = Vector2.zero;
	private Vector3 target;
	private Ray myRay;
	private RaycastHit hitInfo;
	private Quaternion prevRot;
	private bool h_Aimed;
	private bool v_Aimed;
	private bool aimReady = false;

	public float rotInterpolation = 10f;
	public Transform canonSP;
	public Transform canon;
	public Transform gyro;
	public Transform gyroBase;
	public Transform h_Gyro;
	public Transform v_Gyro;
	public Transform forcePos;
	public GameObject tankShootFlame;
	public GameObject bullet;
	// Use this for initialization
	void Start () {
		myTransform = transform;
		transform.localRotation = Quaternion.identity;
		canon.localRotation = Quaternion.identity;
		GlobalInfo.MainGameInfo.aimReady = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (GlobalInfo.MainGameInfo.pauseFlag || MainGameInfo.patriotFlag) {
			return;
		}
		if (GlobalInfo.MainGameInfo.specialCam && !GlobalInfo.MainGameInfo.autoAim) {
			transform.localRotation = Quaternion.identity;
//			canon.localRotation = Quaternion.identity;
		}
		//When Cross Hair Pos Changed
		myRay = Camera.main.ScreenPointToRay(new Vector3(GlobalInfo.MainGameInfo.crossHair_Pos.x,Screen.height - GlobalInfo.MainGameInfo.crossHair_Pos.y,0));

		//Find the Aimed Target
		if(Physics.Raycast(myRay,out hitInfo,Mathf.Infinity)){
			GlobalInfo.MainGameInfo.distanceToTarget = Mathf.FloorToInt(Vector3.Distance(hitInfo.point,transform.position));
			target = myRay.GetPoint(500f);
		}else{
			GlobalInfo.MainGameInfo.distanceToTarget = Random.Range(500,1000);
			target = myRay.GetPoint(500f);
		}

		//Canon Gyroscope
		float ang, ang1;
		h_Aimed = false;v_Aimed = false;
		Vector3 tmp = Vector3.zero;
		tmp = target - transform.position;
		tmp.Normalize();

		ang = Vector3.Angle(transform.up,tmp);
		ang1 = Vector3.Angle(transform.up,canonSP.forward);
		ang = Mathf.Clamp(ang,70.0f,100.0f);
		ang1 = (ang - ang1) * Mathf.PI / 180;
		float tmp1 = rotInterpolation * Time.deltaTime;
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
			
		//Determine if enemy in my shot Range
		if(hitInfo.collider != null){
			if(hitInfo.collider.CompareTag("EnemyTank") || hitInfo.collider.CompareTag("RPG") || hitInfo.collider.CompareTag("Bunker")){
				if(h_Aimed && v_Aimed){
					GlobalInfo.MainGameInfo.aimReady = true;
				}else{
					GlobalInfo.MainGameInfo.aimReady = false;
				}
			}else{
				GlobalInfo.MainGameInfo.aimReady = false;
			}
		}

		ang1 = Vector3.Angle(transform.right,gyro.forward);
		if(ang1 != 90.0f) {
			if(ang1 > 90.0f)ang = -ang;
			transform.RotateAroundLocal(Vector3.up,ang * Time.deltaTime * rotInterpolation);
		}
	}

	public void OnShot(){
		if (MainGameInfo.patriotFlag) {
			return;
		}
		GameObject shotFlame = (GameObject)Instantiate(tankShootFlame,canonSP.position,canonSP.rotation);
		GameObject tankShell = (GameObject)Instantiate (bullet, canonSP.position, canonSP.rotation);
		tankShell.SendMessage("SetFlag",true,SendMessageOptions.DontRequireReceiver);

		transform.parent.GetComponent<Rigidbody>().AddForceAtPosition (transform.up * 1500f, forcePos.position, ForceMode.Impulse);
	}
}

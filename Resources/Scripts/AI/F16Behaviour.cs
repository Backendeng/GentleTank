using UnityEngine;
using System.Collections;
using GlobalInfo;

public class F16Behaviour : MonoBehaviour {
	public Transform playerPos;
	public float speed = 20f;
	public Transform leftMissileLanuchPos;
	public Transform rightMissileLaunchPos;
	public GameObject bullet_MiniRocket;
	public float limitDistance = 300f;
	public float shootRange = 100f;
	public float fighter_Height = 45f;
	public Texture2D rangeTexture;

	private bool shootRangeReachedFlag = false;
	private bool raiseUpFlag = false;
	private bool shootFlag = false;
	private float timeCounter = 0;
	// Use this for initialization
	void Start () {
		transform.LookAt (playerPos);
		transform.rotation = new Quaternion (0, transform.rotation.y, 0,transform.rotation.w);
		transform.position = new Vector3 (transform.position.x, fighter_Height, transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (transform.forward * Time.deltaTime * speed,Space.World);

		CheckDistance ();

		//Action for raise down and raise up
		if(shootRangeReachedFlag){
			Vector3 lookAtPos = new Vector3(playerPos.position.x,transform.position.y,playerPos.position.z);
			float distance = Vector3.Distance(lookAtPos,transform.position);

			if((distance > 15f && !raiseUpFlag ||transform.position.y > 40f)){
				transform.RotateAround(transform.position,transform.right,Time.deltaTime * speed * 0.3f);
			}else{
				raiseUpFlag = true;
				transform.RotateAround(transform.position,transform.right,-Time.deltaTime * speed * 0.3f);
			}

			if(transform.position.y < 40f){
				raiseUpFlag = true;
				transform.RotateAround(transform.position,transform.right,-Time.deltaTime * speed * 0.3f);
			}
			if(!shootFlag){
				leftMissileLanuchPos.LookAt(playerPos.position);
				rightMissileLaunchPos.LookAt(playerPos.position);
				StartCoroutine(LaunchMissile());
				shootFlag = true;
			}
			//	transform.RotateAround(transform.position,transform.right,Time.deltaTime * 30f);
			//	transform.rotation = Quaternion.Slerp(transform.rotation,new Quaternion(0,transform.rotation.y,0,0),Time.deltaTime);
		}
	}
	//Bullet Behaviour
	IEnumerator LaunchMissile(){
		yield return new WaitForSeconds (1f);
		leftMissileLanuchPos.LookAt(playerPos.position);
		rightMissileLaunchPos.LookAt(playerPos.position);
		GameObject leftMissile_1 = (GameObject)Instantiate (bullet_MiniRocket, leftMissileLanuchPos.position, leftMissileLanuchPos.rotation);
//		leftMissile_1.transform.parent = transform;
		yield return new WaitForSeconds (0.7f);
		leftMissileLanuchPos.LookAt(playerPos.position);
		rightMissileLaunchPos.LookAt(playerPos.position);
		GameObject rightMissile_1 = (GameObject)Instantiate (bullet_MiniRocket, rightMissileLaunchPos.position, rightMissileLaunchPos.rotation);
//		rightMissile_1.transform.parent = transform;
		yield return new WaitForSeconds (0.7f);
		leftMissileLanuchPos.LookAt(playerPos.position);
		rightMissileLaunchPos.LookAt(playerPos.position);
		GameObject leftMissile_2 = (GameObject)Instantiate (bullet_MiniRocket, leftMissileLanuchPos.position, leftMissileLanuchPos.rotation);
//		leftMissile_2.transform.parent = transform;
		yield return new WaitForSeconds (0.7f);
		leftMissileLanuchPos.LookAt(playerPos.position);
		rightMissileLaunchPos.LookAt(playerPos.position);
		GameObject rightMissile_2 = (GameObject)Instantiate (bullet_MiniRocket, rightMissileLaunchPos.position, rightMissileLaunchPos.rotation);
//		rightMissile_2.transform.parent = transform;

		yield return new WaitForSeconds (2f);

	}

	void CheckDistance(){
		Vector3 temp = new Vector3 (playerPos.position.x, transform.position.y, playerPos.position.z);
		Vector3 lookPos = new Vector3(playerPos.transform.position.x,transform.position.y,playerPos.transform.position.z);
		
		if (Vector3.Distance (transform.position, temp) > limitDistance) {
			if(Vector3.Angle(transform.forward,(transform.position - playerPos.position).normalized) < 90f){
				//Must Reset; Air Fighter Must turn back
				transform.rotation = new Quaternion(0,transform.rotation.y,0,0);
				transform.RotateAroundLocal(Vector3.up,180f * Mathf.Deg2Rad);
				transform.LookAt(lookPos);
				transform.position = new Vector3 (transform.position.x, fighter_Height, transform.position.z);
				shootFlag = false;
			}
		}

		//Air fighter reaches effective range
		if (Vector3.Distance (transform.position, temp) < shootRange) {
			shootRangeReachedFlag = true;
			timeCounter = 0;
		}else{
			shootRangeReachedFlag = false;
			raiseUpFlag = false;
		}
	}

	void OnGUI(){
		
		float val = Vector3.Dot (Camera.main.transform.forward, (transform.position - Camera.main.transform.position).normalized);
		if (val < 0.9f || GlobalInfo.MainGameInfo.pauseFlag || !MainGameInfo.patriotFlag) {
			return;
		}
		
		Vector3 vt = Camera.main.WorldToScreenPoint (transform.position);
		GUI.DrawTexture (new Rect (vt.x - 40f, Screen.height - vt.y - 40f, 80f, 80f), rangeTexture);
	}

}

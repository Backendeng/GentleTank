using UnityEngine;
using System.Collections;
using GlobalInfo;

public class HeliBehaviour : MonoBehaviour {
	private Vector3 currentTarget = Vector3.zero;
	private int currentIndex = 0;
	private bool fightActionFlag = false;
	private bool shotFlag = false;

	public Texture2D rangeTexture;
	public Transform propeller;
	public Transform backPropeller;
	public Transform playerPos;
	public Transform left_Shooot_Pos;
	public Transform right_Shoot_Pos;
	public float speed;
	public Transform[] wayPoints;
	public GameObject heliMissile;
	// Use this for initialization
	void Start () {
		currentIndex = Random.Range (0, 2);
		currentTarget = wayPoints [currentIndex].position;
	}
	
	// Update is called once per frame
	void Update () {

		propeller.RotateAroundLocal (Vector3.up, Time.deltaTime * 25f);
		backPropeller.RotateAroundLocal (Vector3.right, Time.deltaTime * 25f);

		//When Heli Arrives Effective Shooting Area
		if (fightActionFlag) {
			Vector3 temp = new Vector3(playerPos.position.x,transform.position.y,playerPos.position.z);
			float angleToTarget = Vector3.Angle(transform.forward,(temp - transform.position).normalized);
			if(angleToTarget > 3f){
				if(Vector3.Angle(transform.right,(temp - transform.position).normalized) < 90f){
					transform.RotateAround(transform.position,transform.up,Time.deltaTime * 40f);
				}else{
					transform.RotateAround(transform.position,transform.up,-Time.deltaTime * 40f);
				}
			}else{
				transform.Translate(transform.forward * Time.deltaTime * speed,Space.World);
				Vector3 tpVector = new Vector3(transform.position.x,playerPos.transform.position.y,transform.position.z);
				if(Vector3.Distance(tpVector,playerPos.position) < 40f){
					//Shooting Behaviour
					if(!shotFlag){
						StartCoroutine(ShootBehaviour());
						shotFlag = true;
					}
				}
			}
			return;
		}

		//Follows Waypoint
		currentTarget = new Vector3 (wayPoints [currentIndex].position.x, transform.position.y, wayPoints [currentIndex].position.z);
		float angle = Vector3.Angle (transform.forward, (currentTarget - transform.position).normalized);
		if(angle > 5f){
			if(Vector3.Angle(transform.right,(currentTarget - transform.position).normalized) < 90f){
				transform.RotateAround(transform.position,transform.up,Time.deltaTime * 30f);
			}else{
				transform.RotateAround(transform.position,transform.up,-Time.deltaTime * 30f);
			}
		}

		transform.Translate(transform.forward * Time.deltaTime * speed,Space.World);

		if(Vector3.Distance(transform.position,currentTarget) < 15f){
			currentIndex ++;
			if(currentIndex == wayPoints.Length){
				currentIndex = 0;
			}
			currentTarget = new Vector3(wayPoints[currentIndex].position.x,transform.position.y,wayPoints[currentIndex].position.z);
			fightActionFlag = true;
		}
	}

	IEnumerator ShootBehaviour(){
		Vector3 lookPos = Vector3.zero;
		GameObject l_missile_1 = (GameObject)Instantiate (heliMissile, left_Shooot_Pos.position, Quaternion.identity);
		GameObject r_missile_1 = (GameObject)Instantiate (heliMissile, right_Shoot_Pos.position, Quaternion.identity);
		lookPos = CalcRandomPos ();
		l_missile_1.transform.LookAt (lookPos);
		r_missile_1.transform.LookAt (lookPos);
		yield return new WaitForSeconds(0.2f);
		GameObject l_missile_2 = (GameObject)Instantiate (heliMissile, left_Shooot_Pos.position, Quaternion.identity);
		GameObject r_missile_2 = (GameObject)Instantiate (heliMissile, right_Shoot_Pos.position, Quaternion.identity);
		lookPos = CalcRandomPos ();
		l_missile_2.transform.LookAt (lookPos);
		r_missile_2.transform.LookAt (lookPos);
		yield return new WaitForSeconds(0.2f);
		GameObject l_missile_3 = (GameObject)Instantiate (heliMissile, left_Shooot_Pos.position, Quaternion.identity);
		GameObject r_missile_3 = (GameObject)Instantiate (heliMissile, right_Shoot_Pos.position, Quaternion.identity);
		lookPos = CalcRandomPos ();
		l_missile_3.transform.LookAt (lookPos);
		r_missile_3.transform.LookAt (lookPos);
		yield return new WaitForSeconds(0.2f);
		GameObject l_missile_4 = (GameObject)Instantiate (heliMissile, left_Shooot_Pos.position, Quaternion.identity);
		GameObject r_missile_4 = (GameObject)Instantiate (heliMissile, right_Shoot_Pos.position, Quaternion.identity);
		lookPos = CalcRandomPos ();
		l_missile_4.transform.LookAt (lookPos);
		r_missile_4.transform.LookAt (lookPos);
		yield return new WaitForSeconds(0.2f);
		GameObject l_missile_5 = (GameObject)Instantiate (heliMissile, left_Shooot_Pos.position, Quaternion.identity);
		GameObject r_missile_5 = (GameObject)Instantiate (heliMissile, right_Shoot_Pos.position, Quaternion.identity);
		lookPos = CalcRandomPos ();
		l_missile_5.transform.LookAt (lookPos);
		r_missile_5.transform.LookAt (lookPos);
		yield return new WaitForSeconds(0.2f);
		GameObject l_missile_6 = (GameObject)Instantiate (heliMissile, left_Shooot_Pos.position, Quaternion.identity);
		GameObject r_missile_6 = (GameObject)Instantiate (heliMissile, right_Shoot_Pos.position, Quaternion.identity);
		lookPos = CalcRandomPos ();
		l_missile_6.transform.LookAt (lookPos);
		r_missile_6.transform.LookAt (lookPos);
		yield return new WaitForSeconds(0.2f);
		GameObject l_missile_7 = (GameObject)Instantiate (heliMissile, left_Shooot_Pos.position, Quaternion.identity);
		GameObject r_missile_7 = (GameObject)Instantiate (heliMissile, right_Shoot_Pos.position, Quaternion.identity);
		lookPos = CalcRandomPos ();
		l_missile_7.transform.LookAt (lookPos);
		r_missile_7.transform.LookAt (lookPos);
		yield return new WaitForSeconds(0.2f);
		GameObject l_missile_8 = (GameObject)Instantiate (heliMissile, left_Shooot_Pos.position, Quaternion.identity);
		GameObject r_missile_8 = (GameObject)Instantiate (heliMissile, right_Shoot_Pos.position, Quaternion.identity);
		lookPos = CalcRandomPos ();
		l_missile_8.transform.LookAt (lookPos);
		r_missile_8.transform.LookAt (lookPos);
		yield return new WaitForSeconds(0.2f);
		GameObject l_missile_9 = (GameObject)Instantiate (heliMissile, left_Shooot_Pos.position, Quaternion.identity);
		GameObject r_missile_9 = (GameObject)Instantiate (heliMissile, right_Shoot_Pos.position, Quaternion.identity);
		lookPos = CalcRandomPos ();
		l_missile_9.transform.LookAt (lookPos);
		r_missile_9.transform.LookAt (lookPos);
		yield return new WaitForSeconds(0.2f);
		GameObject l_missile_10 = (GameObject)Instantiate (heliMissile, left_Shooot_Pos.position, Quaternion.identity);
		GameObject r_missile_10 = (GameObject)Instantiate (heliMissile, right_Shoot_Pos.position, Quaternion.identity);
		lookPos = CalcRandomPos ();
		l_missile_10.transform.LookAt (lookPos);
		r_missile_10.transform.LookAt (lookPos);

		fightActionFlag = false;
		shotFlag = false;
	}

	Vector3 CalcRandomPos(){
		float xRand = Random.Range (-2f, 2f);
		float zRand = Random.Range (-2f, 2f);
		Vector3 retVal = new Vector3(playerPos.position.x + xRand,playerPos.position.y,playerPos.position.z + zRand);
		return retVal;
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

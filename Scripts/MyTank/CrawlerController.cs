using UnityEngine;
using System.Collections;
using GlobalInfo;

public class CrawlerController : MonoBehaviour {
	public Transform wheelParent;
	public Transform wheelColliderParent;
	public float suspensionDistance;
	public ParticleEmitter smokeTrail;

	private int wheelCount = 0;
	private Transform[] leftWheel;
	private Transform[] rightWheel;
	private WheelCollider[] leftWheelCollider;
	private WheelCollider[] rightWheelCollider;
	private bool flag1,flag2 = false;
	public float speed = 30f;
	// Use this for initialization
	void Start () {
		wheelCount = wheelParent.childCount / 2;
		leftWheel = new Transform[wheelCount];
		rightWheel = new Transform[wheelCount];
		leftWheelCollider = new WheelCollider[wheelCount];
		rightWheelCollider = new WheelCollider[wheelCount];

		foreach(Transform obj in wheelParent) {
			string n = obj.name;
			n = n.Substring(5);
			if(n.Substring(0,1) == "L"){
				leftWheel[System.Int32.Parse(n.Substring(1)) - 1] = obj;

			}else{
				rightWheel[System.Int32.Parse(n.Substring(1)) - 1] = obj;
			}
		}
		for(int i = 0;i < wheelCount;i++) {
			GameObject a = new GameObject("WheelCollider");
			a.transform.parent = wheelColliderParent;
			a.transform.localRotation = Quaternion.identity;
			a.transform.localPosition = leftWheel[i].localPosition;
			leftWheelCollider[i] = a.AddComponent<WheelCollider>();
			leftWheelCollider[i].mass = 1;
			leftWheelCollider[i].radius = 0.2f;
			leftWheelCollider[i].suspensionDistance = suspensionDistance;
			JointSpring js = new JointSpring();
			js.spring = 5000;
			js.damper = 45;
			js.targetPosition = 0.1f;
			leftWheelCollider[i].suspensionSpring = js;
			leftWheelCollider[i].brakeTorque = Mathf.Infinity;
			a = new GameObject("WheelCollider");
			a.transform.parent = wheelColliderParent;
			a.transform.localRotation = Quaternion.identity;
			a.transform.localPosition = rightWheel[i].localPosition;
			rightWheelCollider[i] = a.AddComponent<WheelCollider>();
			rightWheelCollider[i].mass = 1;
			rightWheelCollider[i].radius = 0.2f;
			rightWheelCollider[i].suspensionDistance = suspensionDistance;
			js = new JointSpring();
			js.spring = 5000;
			js.damper = 45;
			js.targetPosition = 0.1f;
			rightWheelCollider[i].suspensionSpring = js;
			rightWheelCollider[i].brakeTorque = Mathf.Infinity;
		}
	}
	
	// Update is called once per frame
	void Update () {

		RaycastHit hit = new RaycastHit ();
		float rpm = 0.0f;

		WheelFrictionCurve wf = new WheelFrictionCurve ();
		float leftAverRpm = 0.0f;
		float rightAverRpm = 0.0f;
		for(int i = 0;i < wheelCount;i++){
			if(i != 0 && i != wheelCount - 1);leftAverRpm += leftWheelCollider[i].rpm / 60.0f;
			if(leftWheelCollider[i].rpm == 0.0f) rpm = 1; else rpm = leftWheelCollider[i].rpm;
			rpm = rpm / 60.0f;
			wf.extremumValue = 5000 / Mathf.Pow(rpm , 3);
			wf.asymptoteValue = 5000 * 0.5f / Mathf.Pow(rpm , 3);
			leftWheelCollider[i].forwardFriction = wf;
			Vector3 cp = leftWheelCollider[i].transform.TransformPoint(leftWheelCollider[i].center);
			if(Physics.Raycast (cp,-leftWheelCollider[i].transform.up,out hit,leftWheelCollider[i].suspensionDistance + leftWheelCollider[i].radius)){
				leftWheel[i].position = hit.point + (leftWheelCollider[i].transform.up * leftWheelCollider[i].radius);
			}else{
				leftWheel[i].position = cp - (leftWheelCollider[i].transform.up * leftWheelCollider[i].suspensionDistance);
			}
			leftWheel[i].Rotate(leftWheelCollider[i].rpm * 6 * Time.deltaTime,0,0);
		}
		leftAverRpm = -leftAverRpm / (wheelCount * 2);
		for(int i = 0;i < wheelCount;i++){
			if(i != 0 && i != wheelCount - 1)rightAverRpm += rightWheelCollider[i].rpm / 60.0f;
			if(rightWheelCollider[i].rpm == 0.0f) rpm = 1; else rpm = rightWheelCollider[i].rpm;
			rpm = rpm / 60.0f;
			wf.extremumValue = 5000 / Mathf.Pow(rpm , 3);
			wf.asymptoteValue = 5000 * 0.5f / Mathf.Pow(rpm , 3);
			rightWheelCollider[i].forwardFriction = wf;
			Vector3 cp = rightWheelCollider[i].transform.TransformPoint(rightWheelCollider[i].center);
			if(Physics.Raycast (cp,-rightWheelCollider[i].transform.up,out hit,rightWheelCollider[i].suspensionDistance + rightWheelCollider[i].radius)){
				rightWheel[i].position = hit.point + (rightWheelCollider[i].transform.up * rightWheelCollider[i].radius);
			}else{
				rightWheel[i].position = cp - (rightWheelCollider[i].transform.up * rightWheelCollider[i].suspensionDistance);
			}
			rightWheel[i].Rotate(rightWheelCollider[i].rpm * 6 * Time.deltaTime,0,0);
		}
		float vert = GlobalInfo.MainGameInfo.spd;
		if(vert > 0) {
			smokeTrail.minSize = 1f;
			for(int i = 0;i < wheelCount;i++) {
				leftWheelCollider[i].brakeTorque = 0;
				rightWheelCollider[i].brakeTorque = 0;
				leftWheelCollider[i].motorTorque = vert * (speed+20f);
				rightWheelCollider[i].motorTorque = vert * (speed+20f);
				float rpm1 = leftWheelCollider[i].rpm;
				float rpm2 = rightWheelCollider[i].rpm;
				if(rpm1 == 0) rpm1 = 1;
				if(rpm2 ==0)rpm2 =1;
				rpm1 = rpm1 / 60.0f;
				rpm2 = rpm2/60f;
				WheelFrictionCurve wf1 = new WheelFrictionCurve();
				wf1.asymptoteSlip = 2.0f;
				wf1.extremumSlip = 1.0f;
				wf1.stiffness = 1.0f;
				wf1.extremumValue = 5000 / Mathf.Pow(rpm1 , 3);
				wf1.asymptoteValue = 5000 * 0.5f / Mathf.Pow(rpm1 , 3);
				WheelFrictionCurve wf2 = new WheelFrictionCurve();
				wf2.asymptoteSlip = 2.0f;
				wf2.extremumSlip = 1.0f;
				wf2.stiffness = 1.0f;
				wf2.extremumValue = 5000 / Mathf.Pow(rpm2 , 3);
				wf2.asymptoteValue = 5000 * 0.5f / Mathf.Pow(rpm2 , 3);
				leftWheelCollider[i].forwardFriction = wf1;
				rightWheelCollider[i].forwardFriction = wf2;
			}
		}else if(vert<0)
		{
			smokeTrail.minSize = 1f;
			for(int i = 0;i < wheelCount;i++) {
				leftWheelCollider[i].brakeTorque = 0;
				rightWheelCollider[i].brakeTorque = 0;
				leftWheelCollider[i].motorTorque = vert * (speed+20f);
				rightWheelCollider[i].motorTorque = vert * (speed+20f);
				float rpm1 = leftWheelCollider[i].rpm;
				float rpm2 = rightWheelCollider[i].rpm;
				if(rpm1 == 0) rpm1 = 1;
				if(rpm2 ==0)rpm2 =1;
				rpm1 = rpm1 / 60.0f;
				rpm2 = rpm2/60f;
				WheelFrictionCurve wf1 = new WheelFrictionCurve();
				wf1.asymptoteSlip = 2.0f;
				wf1.extremumSlip = 1.0f;
				wf1.stiffness = 1.0f;
				wf1.extremumValue = 5000 / Mathf.Pow(rpm1 , 3);
				wf1.asymptoteValue = 5000 * 0.5f / Mathf.Pow(rpm1 , 3);
				WheelFrictionCurve wf2 = new WheelFrictionCurve();
				wf2.asymptoteSlip = 2.0f;
				wf2.extremumSlip = 1.0f;
				wf2.stiffness = 1.0f;
				wf2.extremumValue = 5000 / Mathf.Pow(rpm2 , 3);
				wf2.asymptoteValue = 5000 * 0.5f / Mathf.Pow(rpm2 , 3);
				leftWheelCollider[i].forwardFriction = wf1;
				rightWheelCollider[i].forwardFriction = wf2;
			}
		}
		else{
			smokeTrail.minSize = 0;
			for(int i = 0;i < wheelCount;i++) {
				leftWheelCollider[i].motorTorque = 0;
				rightWheelCollider[i].motorTorque = 0;
				leftWheelCollider[i].brakeTorque = 250f;
				rightWheelCollider[i].brakeTorque = 250f;
				WheelFrictionCurve wf1 = new WheelFrictionCurve();
				wf1.asymptoteSlip = 2.0f;
				wf1.extremumSlip = 1.0f;
				wf1.stiffness = 1.0f;
				wf1.extremumValue = 5000;
				wf1.asymptoteValue = 5000 * 0.5f;
				leftWheelCollider[i].forwardFriction = wf1;
				rightWheelCollider[i].forwardFriction = wf1;
			}
		}
	}

}

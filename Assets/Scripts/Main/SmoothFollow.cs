using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour {
	public Transform target;
	public float distance = 10.0f;

	public float height = 5.0f;

	public float heightDamping = 2.0f;
	public float rotationDamping = 3.0f;
	private float wantedRotationAngle = 0.0f;
	private float wantedHeight = 0.0f;
	private float currentRotationAngle = 0.0f;
	private float currentHeight = 0.0f;
	private Quaternion currentRotation = Quaternion.identity;
	private float targetDistance = 0.0f;
	
// Place the script in the Camera-Control group in the component menu

	void LateUpdate () {
		// Early out if we don't have a target
		if (!target)
			return;
		
		// Calculate the current rotation angles
		float ang = 90.0f - Vector3.Angle(Vector3.up,target.forward);
		ang = ang * Mathf.PI / 180.0f;
		wantedRotationAngle = target.eulerAngles.y;
		wantedHeight = target.position.y - distance * Mathf.Sin(ang) + height * Mathf.Cos(ang);
		
		targetDistance = distance * Mathf.Cos(ang) + height * Mathf.Sin(ang);
		currentRotationAngle = transform.eulerAngles.y;
		currentHeight = transform.position.y;
		
		// Damp the rotation around the y-axis
		currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
	
		// Damp the height
		currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
	
		// Convert the angle into a rotation
		currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
		
		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		transform.position = target.position;
		transform.position -= currentRotation * Vector3.forward * targetDistance;
	
		// Set the height of the camera
		transform.position = new Vector3(transform.position.x,currentHeight,transform.position.z);
		
		// Always look at the target
		transform.LookAt (target.position + new Vector3(0,1f,0));
	}
		// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TankWaypointEditor : MonoBehaviour {

	private List<Vector3> wayPoints = new List<Vector3>();
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.RotateAroundLocal (Vector3.up, Time.deltaTime * 0.02f);
	}
	
	void OnDrawGizmos(){
		foreach (Transform tf in transform) {
			wayPoints.Add(tf.position);
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(tf.position,1f);
		}

	}
}

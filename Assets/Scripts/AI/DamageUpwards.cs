using UnityEngine;
using System.Collections;

public class DamageUpwards : MonoBehaviour {
	public Transform body;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDamage(float val){
		body.SendMessage ("OnDamage", val, SendMessageOptions.DontRequireReceiver);
	}
}

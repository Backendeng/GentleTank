using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light))]

public class FlashLight : MonoBehaviour {

	public float LightMult = 2;
	void Update () {
		if(!this.GetComponent<Light>())
			return;
		
		this.GetComponent<Light>().intensity -= LightMult*Time.deltaTime;
	}
}

#pragma strict
private var myLight : NoiseAndGrain;
function Start () {
    myLight = GetComponent(NoiseAndGrain);
}

function Update () {
	
}

function Play(){
	myLight.enabled = !myLight.enabled;
}

function DisableNoise(){
	myLight.enabled = false;
}

function EnableNoise(){
	myLight.enabled = true;
}
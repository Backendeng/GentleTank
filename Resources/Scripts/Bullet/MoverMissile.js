public var target : GameObject;
public var targettag : String;
public var damping = 3;
public var Speed = 80;
public var SpeedMax = 80;
public var SpeedMult:float = 1;
public var Noise:Vector3 = new Vector3(20,20,20);
private var locked:boolean;
public var distanceLock:int = 70;
public var DulationLock:int = 40;
private var timetorock:int;
public var Seeker:boolean;
public var LifeTime:float = 5.0;
private var timeCount:float = 0;
private var lookAtPos:Vector3 = new Vector3(0,0,0);
public var targetlockdirection:float = 0.5f;

function Start(){
	target = GameObject.Find("MainPlayer");
	timeCount = Time.time;
	Destroy(gameObject, LifeTime);

	lookAtPos = new Vector3(target.transform.position.x + Random.Range(-1f,1f),target.transform.position.y,target.transform.position.z + Random.Range(-1f,1f));
	transform.LookAt(lookAtPos);
}

function Update () {

	if(Time.time>= (timeCount + LifeTime) -0.5f){
		if(this.GetComponent(Damage)){
			this.GetComponent(Damage).Active();
		}
	}
	/*
	if(Seeker){
		if(timetorock>DulationLock){
		if(!locked && !target){
			var distance = int.MaxValue;
			if(GameObject.FindGameObjectsWithTag(targettag).Length>0){
				var objs:Array = GameObject.FindGameObjectsWithTag(targettag);

				for(var i=0;i<objs.length;i++){
					if(objs[i]){
						var dis = Vector3.Distance(objs[i].transform.position, transform.position);
						if(distanceLock > dis){
							if(distance>dis){
								distance = dis;
								target = objs[i];
							}
							locked = true;
							}
						}
					}
				}
			}
		}else{
			timetorock+=1;
		}
		
		if(target){
			Debug.Log("Target OK");
	    	damping+= 0.9f;
	    	var rotation = Quaternion.LookRotation(target.transform.position - transform.transform.position);
	    	transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * damping);
	    	var dir	= (target.transform.position - this.transform.position).normalized;
			var direction = Vector3.Dot(dir,this.transform.forward);
			if(direction < targetlockdirection)
			{
				target = null;
			}
		}else{
			Debug.Log("No Target");
			locked = false;
			
		}
		
	}*/
	if(Speed < SpeedMax){
		Speed += SpeedMult;
	}
	
	
	
	GetComponent.<Rigidbody>().velocity.x = transform.forward.x * Speed * Time.deltaTime;
    GetComponent.<Rigidbody>().velocity.z = transform.forward.z * Speed * Time.deltaTime;
    GetComponent.<Rigidbody>().velocity.y = transform.forward.y * Speed * Time.deltaTime;

    GetComponent.<Rigidbody>().velocity.x += Random.Range(-Noise.x,Noise.x);
    GetComponent.<Rigidbody>().velocity.z += Random.Range(-Noise.z,Noise.z);
    GetComponent.<Rigidbody>().velocity.y += Random.Range(-Noise.y,Noise.y);

}



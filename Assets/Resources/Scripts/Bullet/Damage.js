var effect:GameObject;
var Owner:GameObject;
var Damage:int = 20;
var Explosive:boolean;
var ExplosionRadius:float = 20;
var ExplosionForce:float = 1000;
function Start () {
	var tf : Transform;
	tf = transform;
	while(tf.parent != null){
		tf = tf.parent;
		if(tf.GetComponent(BoxCollider) != null){
			Owner = tf.gameObject;		
		}
	}
	
	if(Owner){
		if(Owner.GetComponent.<Collider>()){
			Physics.IgnoreCollision(this.GetComponent.<Collider>(),Owner.GetComponent.<Collider>());
		}
	}
	
	transform.parent = null;
}
function Update(){
	
}
function Active(){
	if(effect){
   		var obj:GameObject = Instantiate(effect,this.transform.position,this.transform.rotation);
   		GameObject.Destroy(obj,3);
   	}
   	
   	if(Explosive)
   	ExplosionDamage();
   	  	
   	Destroy(this.gameObject,0.17f);
}

function ExplosionDamage(){


}


function NormalDamage(collision : Collision){
	/*
	if(collision.gameObject.GetComponent(DamageManager)){
		collision.gameObject.GetComponent(DamageManager).ApplyDamage(Damage);
	}*/	
}

function OnCollisionEnter(collision : Collision) {
	if(collision.gameObject.tag!="Particle" && collision.gameObject.tag!="Bullet"){
		if(!Explosive)
		NormalDamage(collision);
		Active();
	}
   	
}
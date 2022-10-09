// prod : Batching Tools v 1.4
// name : BTD.js
// auth : Ippokratis Bournells
// 
// 
//
#pragma strict
//@script ExecuteInEditMode()
//@HideInInspector
public var tris : int[];
//@HideInInspector
var verts : Vector3[];
//@HideInInspector
var cols : Color[];
//@HideInInspector
var idxStart: int;
//@HideInInspector
var uvs : Vector2[];

//@HideInInspector
var parent : Transform;
//@HideInInspector
var position : Vector3;
//@HideInInspector
var rotation : Quaternion;

var BTDdata : BTDstruct;
function Awake()
{
	//Debug.Log(gameObject.name + " has awaked");
}

function SetData (btd:BTDstruct)
{

	tris = btd.tris;
	verts = btd.verts;
	cols = btd.cols;
	idxStart = btd.idxStart;
	uvs = btd.uvs;
	parent = btd.parent;
	position = btd.position;
	rotation = btd.rotation;
}

function GoToParent()
{
	
//	transform.position = position;
//	transform.rotation = rotation;
	//transform.parent = parent;
	//if (parent == null)
	//Debug.Log(gameObject.name + " parent is null");
}


// prod : Batching Tools v 1.4
// name : OriginalObjects.js
// auth : Ippokratis Bournells
// 
// 

#if UNITY_EDITOR
#pragma strict
@script ExecuteInEditMode()

var combinedObjectsID : int;
var IDs : List.<int>;
var parents:List.<Transform>;
var children:List.<Transform>;
var BTDs:List.<BTDstruct>;
var isDynamic:boolean;
var counter : int; 
var topLevel:List.<Transform>;
var TopLevels: TopLevel[];
var COTopLevel : List.<Transform>;

function Awake () 
{
	TopLevels= GetComponentsInChildren.<TopLevel>();
	
	topLevel = new List.<Transform>();
	
	for(var t : TopLevel in TopLevels )
	{
		topLevel.Add(t.gameObject.transform);
	}
	//Deactivation is performed by Combined Objects	
}

//Reset() overlaps with MonoBehaviour.Reset()  
function DoReset()
{
	if(gameObject.activeSelf  == false)
	{
		gameObject.SetActive(true);
	}
	
	var item  : MeshRenderer;

	for ( item in gameObject.GetComponentsInChildren.<MeshRenderer>())
	{
		item.enabled = true;
	}
	
	var t : Transform;
	var i=0;
	
	for ( t in topLevel )
	{
		if( t.gameObject.GetComponent.<TopLevel>().parentS != null )
		{
			t.parent =  t.gameObject.GetComponent.<TopLevel>().parentS;
		}
		else
		{ 
			t.parent = null;
		}	
		DestroyImmediate( t.GetComponent.<TopLevel>() );
	}
	
	var _transforms : Transform[] = FindObjectsOfType(Transform) as Transform[];
	
	for ( t in _transforms) 
	{
		var k : int = t.gameObject.GetInstanceID();

		if (combinedObjectsID == k )
		{
			DestroyImmediate(t.gameObject);
		}
	}
	
	for (  t in COTopLevel )
	{
		if(t!=null)
		{
			DestroyImmediate ( t.gameObject );
		}	
	}
	
		
	DestroyImmediate(gameObject);
}

	
#endif
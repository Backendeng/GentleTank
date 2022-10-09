// prod : Batching Tools v 1.4
// name : CombinedObjects.js
// auth : Ippokratis Bournells
// 
// 
//

#pragma strict
import System.IO;
import System.Collections.Generic;
import System.Linq;

#if UNITY_EDITOR

@script ExecuteInEditMode()

var originalsID : int;
var BT : GameObject;
var mrlength:int;
var batchIndex:int;
var g_mr:List.<MeshRenderer>;
var g_mf:List.<MeshFilter>;


var dynamicGOs:List.<GameObject>;
var isDynamic:boolean; 
var originalObjects:GameObject;

var topLevel:List.<Transform>;
var top2:List.<Transform>;
var top1:List.<Transform>;

var TopLevels: TopLevel[];
var TRs : List.<Transform>;
var meshes : List.<Mesh>;

function Awake () 
{
	var _transforms : Transform[] = FindObjectsOfType(Transform) as Transform[];
	var i : int = 0;
	var tt :Transform;
	var trans : Transform;
	
	for ( tt in _transforms) 
	{
		if (tt.gameObject.GetInstanceID() == originalsID ) 
		{
			tt.gameObject.GetComponent.<OriginalObjects>().combinedObjectsID = gameObject.GetInstanceID();
		}
	}
	
	if(originalsID==0)
	{
		TRs = new  List.<Transform>();
		TRs = RootSolver.GatherAllChildren(transform);
		meshes = new List.<Mesh>();
		for (var tr : Transform in TRs )
		{
			var mesh : Mesh = new Mesh();
			
			if(tr.gameObject.GetComponent(BTD)!=null)
			{
				mesh = 	tr.gameObject.GetComponent(MeshFilter).sharedMesh;
			}	
			meshes.Add( mesh );		
		}
	
	}
//	Debug.Log(originalsID);
//	for (var mes: MeshFilter in gameObject.GetComponentsInChildren(MeshFilter) )
//	{
//		Debug.Log(mes.sharedMesh.name);
//	}
// When originalsID = 0, meshes exist.

	
	if(originalsID!=0)
	{
		var k : int = 0;
		for (var tr : Transform in TRs )
		{
			if(meshes[k]!=null)
			{
				//Debug.Log(meshes[k].name +" "+ tr.gameObject.name );
				tr.gameObject.GetComponent(MeshFilter).sharedMesh = meshes[k];
			}
			++k;
		}
		
		TopLevels= GetComponentsInChildren.<TopLevel>();
		
		topLevel = new List.<Transform>();
		
		for(var t : TopLevel in TopLevels )
		{
			topLevel.Add(t.gameObject.transform);
			
		}
		
		for ( tt in _transforms) 
		{
			if (tt.gameObject.GetInstanceID() == originalsID ) 
			{
				top1 = new List.<Transform>();	
				top1 = tt.gameObject.GetComponent.<OriginalObjects>().topLevel;
				
				top2 = new List.<Transform>();
				for ( trans in top1)
				{
					if( trans.gameObject.GetComponent.<TopLevel>().parentS != null )
					{
						top2.Add(  trans.gameObject.GetComponent.<TopLevel>().parentS);
					}
					else
					{ 
						top2.Add( null);
					}
				}
			}
		}
		
		i = 0;
		for (  trans in topLevel)
		{
			if( top2[i] != null )
			{
				  trans.gameObject.GetComponent.<TopLevel>().parentS = top2[i];
			}
			else
			{ 
				trans.gameObject.GetComponent.<TopLevel>().parentS = null;
			} 
		}

		PlaceBack();	
	}	
}

function Batch()
{
		
	if(batchIndex == 1)
	{
		for (var mmr : MeshRenderer in g_mr )
		{
			mmr.enabled  = false;
		}
		BT.GetComponent.<Renderer>().enabled = true;
		
//		for (var mmf : MeshFilter in g_mf )
//		{
//			Debug.Log(mmf.sharedMesh.name);
//		}
		
	}			

	if(batchIndex == 0)
	{	
		for (var mmr : MeshRenderer in g_mr )
		{
			mmr.enabled  = true;
		}
		BT.GetComponent.<Renderer>().enabled = false;
	}
}

function Bake()
{
	if(batchIndex == 1)
	{
		for (var mmr : MeshRenderer in g_mr )
		{
			DestroyImmediate ( mmr);
		}
		for (var mmf : MeshFilter in g_mf )
		{
			DestroyImmediate ( mmf );
		}
		DestroyImmediate ( this );
	}			

	if(batchIndex == 0)
	{	
		DestroyImmediate ( gameObject );
	}
	
}

function PlaceBack()
{
	g_mr = new List.<MeshRenderer>();
	g_mf = new List.<MeshFilter>();
	
	var tt :Transform;
	for (  tt in topLevel )
	{
		
		tt.gameObject.GetComponent.<TopLevel>().originalObjectsID = originalsID ;

		if( tt.gameObject.GetComponent.<TopLevel>().parentS != null )
		{
			tt.parent =  tt.gameObject.GetComponent.<TopLevel>().parentS;
		}
		else
		{ 
			tt.parent = null;
		}
		
		var mr: MeshRenderer[] = tt.gameObject.GetComponentsInChildren.< MeshRenderer >();
		
		for (var mmr : MeshRenderer in mr )
		{
			g_mr.Add(mmr);
		}
		
		var mf: MeshFilter[] = tt.gameObject.GetComponentsInChildren.< MeshFilter >();
		
		for (var mmf : MeshFilter in mf )
		{
//			if( meshes.ContainsKey(mmf.gameObject))
//			{
//				Debug.Log("tsiu");
//			}
			g_mf.Add(mmf);
			
			
		} 		
	}
	
	var _transforms : Transform[] = FindObjectsOfType(Transform) as Transform[];
	var i : int = 0;
	for ( tt in _transforms) 
	{
		if (tt.gameObject.GetInstanceID() == originalsID ) 
		{
			tt.gameObject.GetComponent.<OriginalObjects>().COTopLevel = topLevel;
			tt.gameObject.SetActive( false );
		}
	}

	batchIndex = 1;
	Batch();
	
}

function KillEm()
{
	var tt :Transform;
	for (  tt in topLevel )
	{
		DestroyImmediate ( tt.gameObject );
	}
}	

#endif

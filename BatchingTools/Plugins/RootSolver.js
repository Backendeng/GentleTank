#pragma strict
#pragma downcast
#if UNITY_EDITOR
@script ExecuteInEditMode()

@SerializeField
var childrenT : List.<Transform>;

@SerializeField
var TopLevel : List.<Transform>;



static function GatherAllChildren(t:Transform):List.<Transform>
{
	var levels :List.< List.<Transform> > = RootSolver.GatherChildren( t );
	var children : List.<Transform> = new  List.<Transform>();
	var child : List.<Transform>;
	var i:int=0;
	for ( child in levels )
	{
		for (var t1:Transform in child)
		{	
			children.Add(t1);
		}
	}
	return children;
}

static function ContainsChild (t:Transform, childT:Transform):boolean
{
	var isChild:boolean = false;
	var levels :List.< List.<Transform> > = RootSolver.GatherChildren( t );
	var child : List.<Transform>;
	var children : List.<Transform> = new  List.<Transform>();
	var i:int=0;
	
	for ( child in levels )
	{
		for (var t1:Transform in child)
		{	
			children.Add(t1);
		}
	}
	
	if (children.Contains(childT))
	{
		isChild = true;
		return isChild;
	}
	else
	{
		return isChild;
	}
}

static function AddChildren( tr : Transform, childL : List.<Transform>):List.<Transform>
{
	var tt : Transform;
	
	for ( tt in tr ) 
	{
		childL.Add(tt) ;
	}
	
	return childL;
}

static function AddLevelChildren( level0 : List.<Transform>, level1: List.<Transform> ):List.<Transform>
{
	var tt : Transform;
	
	for ( tt in level0 )
	{
		level1 = AddChildren(tt, level1);
	}
	
	return level1;
}

static function CountChildren(tr : Transform):int
{ 
	var i : int  = 0;
	var t:Transform;
	
	for ( t in tr )
	{ 
		i += CountChildren(t) + 1; 
	}
	
	return i; 
}

static function AddLevel( curLevel :List.<Transform>, levels : List.< List.<Transform> > ):List.<Transform>
{
	var newLevel : List.<Transform> = new List.<Transform>();
	newLevel = AddLevelChildren(curLevel,newLevel);
	levels.Add( newLevel);
	return newLevel;
}

static function GatherChildren(t : Transform):List.< List.<Transform> >
{
	var counter : int;
	counter = CountChildren(t);
	//Debug.Log(counter);
	var levels :List.< List.<Transform> > = new List.< List.<Transform> >();
	
	var childH: List.<Transform> = new List.<Transform>();
	childH.Add(t);

	while(counter>0)
	{
		childH = AddLevel( childH, levels);
		counter -= childH.Count;
	}
	
	return levels;
}



static function GatherGOsRoots( GOs : GameObject[] ):GameObject[]
{
	var GORoots : Dictionary.< int, GameObject > = new Dictionary.< int, GameObject >();
	var go : GameObject;
	
	for ( go in GOs )
	{
		if (go.transform.parent==null)
		{
			if( ! GORoots.ContainsKey(go.GetInstanceID()) )
			{
				GORoots.Add(go.GetInstanceID(), go);
			}	
			continue;
		}
		else
		{
			if( ! GORoots.ContainsKey(go.transform.root.GetInstanceID()) )
			{
				GORoots.Add(go.transform.root.gameObject.GetInstanceID(), go.transform.root.gameObject);
			}
		}
	}
	var GORootsAr : GameObject[] = new GameObject[GORoots.Count];
	GORoots.Values.CopyTo(GORootsAr, 0);
	return GORootsAr;
}



#endif
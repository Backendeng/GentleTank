// prod : Batching Tools v 1.4
// name : BatchingTools.js
// auth : Ippokratis Bournells
// 
// 
#pragma strict
#pragma downcast
import System.IO;
import System.Collections.Generic;
import System.Linq;

class BatchingTools extends EditorWindow 
{
	var gs_mf:MeshFilter[];
	var gs_mr:MeshRenderer[];		
 	var shaderName : String;
 	var packedTextureBump: Texture2D;
	var packedTexture: Texture2D;
	var uvList : List.<Vector2>;
	var uv2List : List.<Vector2>;
	var parents:List.<Transform>;
	var originalObjectsID:int;
	var originalObjects:GameObject;
	var combinedObjects:GameObject;
	var IDs : List.<int>;
	var mat : Material;
	var _BTShader : BTShader;
	var BTDs:List.<BTDstruct>;
	//flags
	var meshesStatic :boolean=false;
	var hasMainTexture:boolean=false;
	var combineBump : boolean = false;
	var bakeColorsToVertex : boolean = false;
	var composeStatic : boolean = false;
	var composeDynamic : boolean = false;
	var sentinelID : int;

	//Divide in Static and dynamic
	function SeperateStaticDynamic( srcT:Transform[], topSrc : Transform[] )
	{

		var srcGO : GameObject[] = new GameObject[srcT.Length];
		
		var tt : Transform;
		var i : int =0;
		for ( tt in srcT )
		{
			if ( !tt.gameObject.activeSelf )
			{
				EditorUtility.DisplayDialog("Inactive GameObjects found!", "Please check your gameObjects, there is at least one inactive, called " +tt.gameObject.name+". You should only provide active gameObjects. Thanks", "");
	    		return;
	    	}	
	    	
	    	if ( tt.gameObject.GetComponent.<MeshRenderer>() != null && tt.gameObject.GetComponent.<MeshFilter>() == null )
	    	{
	    		EditorUtility.DisplayDialog("No Mesh Filter found!", "Please check your gameObjects, there is at least one that contains a MeshRenderer but not MeshFilter, called " +tt.gameObject.name+". You should only provide gameObjects that contain both MeshFilters and MeshRenderers. Thanks", "");
	    		return;
	    	}
	    	
	    	if ( tt.gameObject.GetComponent.<MeshFilter>() !=null &&  tt.gameObject.GetComponent.<MeshRenderer>() ==null  )
	    	{
	    		EditorUtility.DisplayDialog("No Mesh Renderer found!", "Please check your gameObjects, there is at least one that contains a MeshFilter but not MeshRenderer, called " +tt.gameObject.name+". You should only provide gameObjects that contain both MeshFilters and MeshRenderers. Thanks", "");
	    		return;
	    	}
	    		srcGO[i] = tt.gameObject;
	    		++i;
			
		}
		
		var topGO : List.<GameObject> = new List.<GameObject>();
		for ( tt in topSrc )
		{	
			topGO.Add( tt.gameObject );
	    }		
		//	
		var hasDynamicMeshes: boolean =false;
		var hasStaticMeshes: boolean =false;
		var BTStaticGOs : List.<GameObject> = new List.<GameObject>();
		var BTDynamicGOs : List.<GameObject> = new List.<GameObject>();
		var meshAll:MeshFilter[];
		composeStatic = false;
		composeDynamic = false;
		BTDs = new List.<BTDstruct>();
		i = 0;
		for (var item : GameObject in srcGO)
		{
			if  ( item.GetComponentsInChildren.<MeshFilter>().Length > 0 )
			{
				meshAll = new MeshFilter[ item.GetComponentsInChildren.<MeshFilter>().Length ];
				meshAll = item.GetComponentsInChildren.<MeshFilter>();
				var item_: MeshFilter;
				
				for ( item_ in meshAll )
				{
					if  ( item_.gameObject.GetComponent.<BTD>() != null )
					{
						if ( !BTDynamicGOs.Contains(item_.gameObject))
						{
								BTDynamicGOs.Add( item_.gameObject );
//								var BTDdata = new BTDstruct();
//								BTDdata.parent  = item_.gameObject.transform.parent;
//								BTDdata.position = item_.gameObject.transform.position;
//								BTDdata.rotation = item_.gameObject.transform.rotation;
//								BTDdata.cols = item_.sharedMesh.colors;
//								BTDs.Add(BTDdata);
//								++i;
						}		
					}
					else
					{
						if ( !BTStaticGOs.Contains(item_.gameObject))
						{
								BTStaticGOs.Add( item_.gameObject );
						}
					}
				}	
			}
		}
	
		if ( BTDynamicGOs.Count>1 )
		{
			hasDynamicMeshes = true;
			composeDynamic = true;
		}
	
		if ( BTStaticGOs.Count>1 )
		{
			hasStaticMeshes = true;
			composeStatic = true;		
		}
		
		if( hasDynamicMeshes && hasStaticMeshes )
		{
			EditorUtility.DisplayDialog("Cannot mix dynamic and static", "Please select only dynamic or static objects. You cannot mix them. Thans", "");
			return;
		}
		
		if( hasStaticMeshes&&Check( BTStaticGOs ) )
		{
			StoreOriginals( topGO );
			CombineMaterials( BTStaticGOs) ;
		}
			
		if( hasDynamicMeshes&&Check( BTDynamicGOs ) )
		{
			StoreOriginals( topGO );
			CombineMaterials( BTDynamicGOs );
		}
				
		if(!hasDynamicMeshes&&!hasStaticMeshes)
		{
			EditorUtility.DisplayDialog("Not enough ( more than one ) MeshFilter components found!", "Please select 2 or more gameObjects with MeshFilters to combine. Thanks", "");
			return;
		}
	}
	
	//Retrieve Mesh Filters, Mesh Renderers, IDs, check for multiple, unsuported shaders, >65k vertices
	function Check(srcGo:List.<GameObject>):boolean
	{
		//--Retrieve Mesh Filters
		var check : boolean = false;
		var mflength:int = srcGo.Count;
		gs_mf = new MeshFilter[mflength];
		gs_mr  = new MeshRenderer[mflength];
		IDs = new List.<int>();
		var i:int=0;
		var itemMF : GameObject;
		
		for ( itemMF in srcGo)
		{
			
			gs_mf[i] = itemMF.GetComponent.<MeshFilter>();
			gs_mr[i] = itemMF.GetComponent.<MeshRenderer>();
			
			if( gs_mf.Length != gs_mr.Length )
			{
				EditorUtility.DisplayDialog("Non - equal number of MeshFilters and MeshRenderers!", "Please check your gameObjects, there is at least one with a MeshFilter but without a MeshRenderer or the inverse. You should either eliminate the excessive or add the missing. Thanks", "");
	    		return check;
			}

			IDs.Add(itemMF.gameObject.GetInstanceID());
			++i;
		}
		//--Check if we retrieved one shader only
		var shaders : Shader[] = new Shader[mflength];
		var item1 : MeshRenderer;
		i=0;

		for (item1 in gs_mr)
		{

			shaders[i] = item1.sharedMaterial.shader;
			++i;
		}
		
		var distinct :int = Enumerable.Count(Enumerable.Distinct(shaders));
		
		if(distinct>1)
		{
			EditorUtility.DisplayDialog("More than one shaders found!", "Please only select gameObjects with the same shader. Thanks", "");
	    	return check;
		}
		
		shaderName = shaders[0].name;
		
		//--Check shader compatibility, retrieve material and set flags
		_BTShader = BTHelper.SelectShader(shaderName);
		combineBump = _BTShader.combineBump;
		bakeColorsToVertex = _BTShader.bakeColorsToVertex;
		
		//--Check for submeshes
		var meshFilterWithSubmesh  : MeshFilter;
		var hasSubmeshes : boolean = false;
		for	( meshFilterWithSubmesh in gs_mf)
		{
			if (meshFilterWithSubmesh.sharedMesh.subMeshCount > 1)
			{
				EditorUtility.DisplayDialog("Mesh with submeshes found !", "Please manually select gameObject named " +meshFilterWithSubmesh.gameObject.name + " and perform ExpandSubmeshes on it. After that, please remove its mesh renderer component. Thanks", "");
	    		return check;
			}
			
		}
	    //--65k limit workaround---------------------------
	    var vertsCount : int =0;// sum of vertices so far
	    
	    for( i = 0; i< mflength; ++i)
		{
			vertsCount = vertsCount + gs_mf[i].sharedMesh.vertices.Length;
		}
		//	65k limit applies always
		if( vertsCount>65000  )
		{
			EditorUtility.DisplayDialog("Too many vertices !", "Please select less gameObjects. Currently selected gameobject's total vertices are " + vertsCount.ToString()+ ". You should aim for a total number of vertices that is less than 65000.  Thanks", "");
	    	return check;
		}
		
		check = true;
		return check;
	}
		
	function StoreOriginals(srcGo:List.<GameObject>)
	{	
		//var ttopLevel : List.<Transform> = new List.<Transform>();
		
		for ( var go: GameObject in srcGo)
		{
			go.AddComponent.<TopLevel>();
			if(go.transform.parent!=go.transform)
			{
				go.GetComponent.<TopLevel>().parentS = go.transform.parent;
			}
			else
			{
				go.GetComponent.<TopLevel>().parentS = null;
			}
			
			//ttopLevel.Add(go.transform);
		}
		
		combinedObjects  = new GameObject("Original Objects_" + combinedObjectsName);
		var _transformGO : Transform = combinedObjects.transform;
		parents = new List.<Transform>();
		var item : GameObject;
		
		for ( item in srcGo)
		{
			parents.Add(item.transform.parent);
			item.transform.parent = _transformGO;
		}
		
		originalObjects = Instantiate( combinedObjects ) cast GameObject;
		originalObjects.name = "Original Objects_" + combinedObjectsName;
		originalObjects.AddComponent.<OriginalObjects>();
		originalObjects.GetComponent.<OriginalObjects>().parents = parents;
		originalObjectsID = originalObjects.GetInstanceID();
		if(composeDynamic)
		{
			originalObjects.GetComponent.<OriginalObjects>().isDynamic = true;
		}
	
	}
	
	function CombineMaterials( srcGo:List.<GameObject> )
	{
	    var mf:MeshFilter[] = gs_mf;
		var mflength:int=mf.length;
		var progress : float = 0.0;
	    var step : float = 1.0 / mflength ;
	    var mr:MeshRenderer[] = gs_mr;
	    var mrlength:int=mr.length;
   	 	var textures: Texture2D[] = new Texture2D[mrlength];
   	 	var texturesDictionary: Dictionary.< Texture2D, Texture2D > = new Dictionary.< Texture2D, Texture2D >(mrlength);    	 	
   	 	var texturesBump : Texture2D[] = new Texture2D[mrlength];
		var meshes : Mesh[] = new Mesh[mrlength];
		//--Create a new mesh for each GO, since we use the sharedMesh
		var newMeshes : Mesh[] = new Mesh[mrlength];
		//-------------------------------Combine Materials-------------------------------
	    //--Ensure textures are readable and without mip maps. If not, modifies them to be so.
	    //--Removed var declaration outside the for loop to increase speed   
	    var i:int=0;
	    var textureBump : Texture2D;
	    var texture : Texture2D;
	    var path : String ;
	    var textureImporter : TextureImporter;
	 	var textureWhite : Texture2D = BTHelper.MakeTile("opa", Color.white, false);
	    //Gather Textures
	    for (i=0; i< mflength; ++i)
	    {
			EditorUtility.DisplayProgressBar("Batching Tools", "Adding texture " +mr[i].sharedMaterial.name , progress + step);
			progress = progress + step;
			//--"as Texture2D;" Solves BCW0028: WARNING: Implicit downcast from 'UnityEngine.Texture' to 'UnityEngine.Texture2D'.
			texture = mr[i].sharedMaterial.mainTexture as Texture2D;
			path = AssetDatabase.GetAssetPath(texture); 
	        textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
	        
	        if(textureImporter == null)
	        {
        		textures[i] = textureWhite;
	        }
	        else
	        {
		        if( textureImporter.isReadable == false||textureImporter.textureFormat != TextureImporterFormat.ARGB32)//||textureImporter.mipmapEnabled ==true)
		        {
		        	EditorUtility.DisplayProgressBar("Batching Tools", "Batching Tools sets texture " + texture.name + " to be Readable. This takes a while", progress);
		        	textureImporter.isReadable = true;
		        	textureImporter.textureFormat = TextureImporterFormat.ARGB32;
		        	AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
		        }
				textures[i] = texture;
			}
			
			if (combineBump)
			{
				textureBump = mr[i].sharedMaterial.GetTexture("_BumpMap") as Texture2D;
				path = AssetDatabase.GetAssetPath(textureBump); 
				textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
				
				if(textureImporter == null)
				{
					textureBump = BTHelper.MakeTile("opa", Color(0.0,0.053,1.0), true);
					
					texturesBump[i] = textureBump;
				}
				else
				{
					if( textureImporter.isReadable == false)
					{
						//Debug.Log("Batching Tools sets texture " +textureBump.name+ " to be Readable. This takes a while");
						textureImporter.isReadable = true;
						textureImporter.textureFormat = TextureImporterFormat.ARGB32;
						AssetDatabase.ImportAsset(path);
					}
					texturesBump[i] = textureBump;
				}
			}
			
			if(bakeColorsToVertex)
			{
				if(mr[i].sharedMaterial.color!=null)
	        	{	
	    			var mfc : MeshFilter = mr[i].gameObject.GetComponent.<MeshFilter>();
					var colors : Color[] = new Color[mfc.sharedMesh.vertices.Length];
					
					for (var color :Color in colors) 
					{
						color = mr[i].sharedMaterial.color;
					}
					
					var mesh : Mesh = new Mesh();
					mesh.vertices = mfc.sharedMesh.vertices;
					mesh.normals = mfc.sharedMesh.normals;
					mesh.uv = mfc.sharedMesh.uv;
					mesh.uv2 = mfc.sharedMesh.uv2;
					mesh.tangents = mfc.sharedMesh.tangents;
					mesh.colors = colors;
					mesh.triangles = mfc.sharedMesh.triangles;
					mesh.RecalculateBounds();
					;
					mfc.sharedMesh = mesh;
	        	}
	        	else
	        	{
	        		EditorUtility.DisplayDialog("Unsupported Shader", "Please select only items with supported shaders", "");
	        	}
	        }	
		}
	    //--Create uv subspaces, texture atlas
		ModifyUV(textures, texturesBump, mflength, mf, step, meshes, newMeshes  );
		//--Create the project folders that will hold the assets
		//Root folder for all assets, Assets/BatchingToolsPrefab
		EditorUtility.DisplayProgressBar("Batching Tools", "Making Folders", 0.1);
		BTHelper.MakeFoldersIfNotYetThere("Assets", "BatchingToolsPrefabs");
		//Root folder for the currently combined objects
		var dir : String = BTHelper.MakeUniqueFolderInFolder("Assets/BatchingToolsPrefabs", combinedObjectsName);
		var partsDir : String = BTHelper.MakeUniqueFolderInFolder( dir, "Parts");
		var mat1 : Material = _BTShader.mat;
	    
	    if ( mat1.HasProperty( "_MainTex" ) )
	    {
	    	mat1.mainTexture = packedTexture;
	    	mat1.mainTexture.wrapMode = TextureWrapMode.Clamp;
	    	mat1.mainTexture.filterMode = FilterMode.Bilinear;
	    }
	    
	    if ( mat1.HasProperty( "_BumpMap" ) )
	    {
	    	mat1.SetTexture( "_BumpMap", packedTextureBump );
	    }

	    EditorUtility.DisplayProgressBar( "Batching Tools", "Saving Materials ( this take a while )", 0.8 );
	    mat1 = BTHelper.ExportMaterial( mat1, dir, combineBump, combinedObjectsName );
		mat = mat1;
		EditorUtility.DisplayProgressBar( "Batching Tools", "Making copy of original meshes", 0.5 );
		i = 0;
		progress = 0.0;
		step = 1.0 / gs_mf.Length ;
		var item : MeshFilter;
		
		for ( item in gs_mf )
		{
			EditorUtility.DisplayProgressBar("Batching Tools", "Saving new Mesh " +item.name, progress + step);
			progress = progress + step;
			item.sharedMesh = BTHelper.ExportMesh( newMeshes[i], item.name, partsDir );
			item.gameObject.GetComponent.<Renderer>().sharedMaterial = mat;
			//item.gameObject.transform.parent = parents[i];
			gs_mf[i] = item;
			++i;
		}

		EditorUtility.ClearProgressBar();
		if ( composeStatic)
		{
			composeStatic = false;
			ComposeStatic( dir );
		}

		if ( composeDynamic )
		{
			composeDynamic = false;
			ComposeDynamic( dir );
		}
	}
	
	function ComposeStatic( dir:String )
	{				
    	//Compose combined objects GO
		combinedObjects.name = combinedObjectsName + "_Static";
		combinedObjects.AddComponent.<CombinedObjects>();
		combinedObjects.GetComponent.<CombinedObjects>().originalsID = originalObjectsID;

		//combinedObjects.GetComponent.<CombinedObjects>().IDs = IDs;
		//Compose combined objects BTStatic
		combinedObjects.GetComponent.<CombinedObjects>().BT = BTStaticCombine(  dir );
		combinedObjects.GetComponent.<CombinedObjects>().BT.name = "BTStatic_" + combinedObjectsName;
		combinedObjects.GetComponent.<CombinedObjects>().BT.transform.parent = combinedObjects.transform;
		combinedObjects.GetComponent.<CombinedObjects>().BT.AddComponent.<Rigidbody>();
		combinedObjects.GetComponent.<CombinedObjects>().BT.GetComponent.<Rigidbody>().isKinematic = true;
		combinedObjects.GetComponent.<CombinedObjects>().BT.GetComponent.<Rigidbody>().useGravity = false;
		BTHelper.ExportGO( combinedObjects, dir );
		
		//originalObjects.GetComponent.<OriginalObjects>().PlaceBack();
	}
	
	function ComposeDynamic(  dir:String )
	{
		//Compose combined objects GO
		//originalObjects.GetComponent.<OriginalObjects>().BTDs = BTDs;
		originalObjects.GetComponent.<OriginalObjects>().isDynamic = true;
		combinedObjects.name = combinedObjectsName + "_Dynamic";
		combinedObjects.AddComponent.<CombinedObjects>();
		combinedObjects.GetComponent.<CombinedObjects>().originalsID = originalObjectsID;
		combinedObjects.GetComponent.<CombinedObjects>().isDynamic = true;
		
//		for (var mes: MeshFilter in combinedObjects.GetComponentsInChildren(MeshFilter) )
//		{
//			Debug.Log(mes.sharedMesh.name);
//		}
		
		//combinedObjects.GetComponent.<CombinedObjectsDynamic>().IDs = IDs;
		//Compose combined objects BTStatic
		combinedObjects.GetComponent.<CombinedObjects>().BT = BTDynamicCombine( dir );
		combinedObjects.GetComponent.<CombinedObjects>().BT.name = "BTDynamic_" + combinedObjectsName;
		combinedObjects.GetComponent.<CombinedObjects>().BT.transform.parent = combinedObjects.transform;
		combinedObjects.GetComponent.<CombinedObjects>().BT.AddComponent.<Rigidbody>();
		combinedObjects.GetComponent.<CombinedObjects>().BT.GetComponent.<Rigidbody>().isKinematic = true;
		combinedObjects.GetComponent.<CombinedObjects>().BT.GetComponent.<Rigidbody>().useGravity = false;
		BTHelper.ExportGO( combinedObjects, dir );
	}
	
	function ModifyUV(textures: Texture2D[], texturesBump : Texture2D[], mflength:int, mf:MeshFilter[],
	step : float, meshes : Mesh[], newMeshes : Mesh[])
	{
		//texture atlas
		packedTexture = new Texture2D(maximumAtlasSize, maximumAtlasSize, TextureFormat.ARGB32, false);
		var uvs: Rect[]  = packedTexture.PackTextures(textures,padding,maximumAtlasSize);
		packedTexture.Apply();
		
		if (combineBump)
		{
			packedTextureBump = new Texture2D(maximumAtlasSize, maximumAtlasSize, TextureFormat.ARGB32, false);
			var uvBump: Rect[]  = packedTextureBump.PackTextures(texturesBump,padding,maximumAtlasSize);
			packedTextureBump.Apply();
		}
		
		var b : int = 0;var b1 : int = 0;var b2 : int = 0;
		var k : int = 0;
		var normalsList : List.<Vector3> = new List.<Vector3>();
		uvList = new List.<Vector2>();
		//var uv1List : List.<Vector2> = new List.<Vector2>();
		uv2List= new List.<Vector2>();
		//Make uv subspace slightly smaller to avoid texture bleeding
		//Suggested value 0.002 ( trial and error )
		var i:int=0;

		for ( i =0; i < uvs.Length; ++i) 
		{		
			uvs[i].x += inset;
			uvs[i].y += inset;
			uvs[i].width -=inset;
			uvs[i].height -=inset;
		}
		
		var progress: float = 0.0;

		for ( i =0; i < mflength; ++i) 
		{
			EditorUtility.DisplayProgressBar("Batching Tools", "Processing mesh " +mf[i].sharedMesh.name , progress + step - 0.5);
			progress = progress + step;
			meshes[i] = mf[i].sharedMesh;
			b = meshes[i].uv.Length;
			b2  = meshes[i].uv2.Length;
			var uva : Vector2[]= new Vector2[b];
			var uva2: Vector2[] = new Vector2[b2];
			uva= meshes[i].uv;
			uva2 = meshes[i].uv2;
			var uvb : Vector2[] = new Vector2[b];
			var uvb2 : Vector2[] = new Vector2[b2];
					
			for ( k = 0; k < b; ++k)
			{
				uvb[k] = new Vector2 ( (uva[k].x*uvs[i].width) + uvs[i].x, (uva[k].y*uvs[i].height) + uvs[i].y);
			}

			if(b2>0)
			{
				for ( k = 0; k < b; ++k)
				{
					uvb2[k] = new Vector2 ( (uva2[k].x*uvs[i].width) + uvs[i].x, (uva2[k].y*uvs[i].height) + uvs[i].y);
				}
			}

			newMeshes[i] = new Mesh();
			newMeshes[i].vertices = meshes[i].vertices;
			newMeshes[i].triangles = meshes[i].triangles;
			newMeshes[i].uv = uvb;
			newMeshes[i].uv2 = uvb2;
			newMeshes[i].normals = meshes[i].normals;
			uvList.AddRange(uvb);
			uv2List.AddRange(uvb2);
			normalsList.AddRange(meshes[i].normals);
			newMeshes[i].tangents = meshes[i].tangents;
			newMeshes[i].colors = meshes[i].colors;
		}
	}

	function BTStaticCombine( dir:String ):GameObject
	{	
		var mflength : int = gs_mf.Length;
		var newMesh = new Mesh();
	    var i:int=0;
		//Prepare combine instance
		var combine : CombineInstance[] = new CombineInstance[mflength];
	
		for ( i = 0; i < mflength; ++i)
		{
			combine[i].mesh = gs_mf[i].sharedMesh;
			//combine[i].subMeshIndex = i;//gives artifacts
			combine[i].transform = gs_mf[i].transform.localToWorldMatrix;//Matrix to transform the mesh with before combining
			gs_mf[i].gameObject.SetActive(true);
			//deactivate original objects mesh renderers
			gs_mf[i].gameObject.GetComponent.<Renderer>().enabled = false;
		}
	
		EditorUtility.DisplayProgressBar("BatchingTools", "Combining meshes" ,0.8);
	    //--Assemble to a new gameObject------------------------------
		//----Mesh
		var GO = new GameObject( "BTStatic_" , MeshFilter, MeshRenderer );
		GO.GetComponent.<MeshFilter>().sharedMesh = new Mesh();
		GO.GetComponent.<MeshFilter>().sharedMesh.CombineMeshes(combine);
		newMesh.vertices = GO.GetComponent.<MeshFilter>().sharedMesh.vertices;
		newMesh.triangles = GO.GetComponent.<MeshFilter>().sharedMesh.triangles;
		newMesh.uv = uvList.ToArray();
		
		if(GO.GetComponent.<MeshFilter>().sharedMesh.normals.Length!=GO.GetComponent.<MeshFilter>().sharedMesh.vertices.Length)
		{
				newMesh.RecalculateNormals();
		}
		else
		{
			newMesh.normals = GO.GetComponent.<MeshFilter>().sharedMesh.normals;
		}		
		
		if(GO.GetComponent.<MeshFilter>().sharedMesh.tangents.Length==GO.GetComponent.<MeshFilter>().sharedMesh.vertices.Length)
		{
			newMesh.tangents = GO.GetComponent.<MeshFilter>().sharedMesh.tangents;
		}
		
		if(GO.GetComponent.<MeshFilter>().sharedMesh.colors.Length==GO.GetComponent.<MeshFilter>().sharedMesh.vertices.Length)
		{
			newMesh.colors = GO.GetComponent.<MeshFilter>().sharedMesh.colors;
		}
		
//		if(uv2List.Count!=GO.GetComponent.<MeshFilter>().sharedMesh.vertices.Length)
//		{
			Unwrapping.GenerateSecondaryUVSet (newMesh);
//		}
//		else
//		{
//			newMesh.uv2 = uv2List.ToArray();
//		}
		
		//------Assign and export Mesh
		GO.GetComponent.<MeshFilter>().sharedMesh = newMesh;
		BTHelper.ExportMesh( newMesh, "BTStaticMesh", dir );
		//---------------mats
		
		GO.GetComponent.<MeshRenderer>().sharedMaterial = mat;
		GO.gameObject.SetActive(true) ;
		EditorUtility.ClearProgressBar();
		//StaticReady = true;
		return GO;				
	}
	
	function BTDynamicCombine(dir : String ):GameObject
	{
		var meshes:List.<Mesh> = new List.<Mesh>();
		var gameObjects:List.<GameObject> = new List.<GameObject>();
		var transforms:List.<Transform> = new List.<Transform>();
		var transformsT:List.<Transform> = new List.<Transform>();
		var itm : MeshFilter;
		var pos : List.<Vector3> = new List.<Vector3>();
		var rot : List.<Quaternion> = new List.<Quaternion>();
		var locRot : List.<Quaternion> = new List.<Quaternion>();
		var locPos : List.<Vector3> = new List.<Vector3>();
		var colors : List.<Color> = new List.<Color>();
		
		for ( itm in gs_mf)
        {
            //Debug.Log(itm.gameObject.transform.position);
            itm = BTHelper.BakeSize(itm);
            meshes.Add(itm.sharedMesh);
            transforms.Add(itm.gameObject.transform);
            transformsT.Add(itm.gameObject.transform.parent);
            pos.Add(itm.gameObject.transform.position);
            locPos.Add(itm.gameObject.transform.localPosition);
            rot.Add(itm.gameObject.transform.rotation);
            locRot.Add(itm.gameObject.transform.localRotation);
            //Debug.Log(itm.gameObject.transform.localPosition);
			gameObjects.Add(itm.gameObject);
//			if(itm.gameObject.transform.parent !=null)
//			{
//				//Debug.Log(itm.gameObject.transform.parent.name);
//			}	
        }
         
		var triangles:List.<int> = new List.<int>();
        var vertices:List.<Vector3> = new List.<Vector3>();
        var normals:List.<Vector3> = new List.<Vector3>();
		var weights:List.<BoneWeight> = new List.<BoneWeight>();
        var bindposes : List.<Matrix4x4> = new List.<Matrix4x4>();
        var vertIdx:int = 0;
        var idx:int = 0;
        var __weights : BoneWeight[] = new BoneWeight[vertIdx];
        //Rebuild the combined mesh from the array of the meshes
        var newMesh : Mesh = new Mesh();
        var trianglesIdx:int = 0;
        var _bonesIdx:int = 0;
        var bonesA : Transform[] = new Transform[gameObjects.Count];
        var bindPosesA : Matrix4x4[] = new Matrix4x4[gameObjects.Count];
        var _meshes : Mesh[] = meshes.ToArray();
		var GO = new GameObject ( "BTDynamic" );
		              
        for (var item:Mesh in _meshes )
		{
			for(var _item: Vector3 in item.vertices)
			{    
			    vertices.Add(_item);
			}
			// BTDs[_bonesIdx].verts = item.vertices;
			
			for(var _item: int in item.triangles)
			{    
			    triangles.Add(_item + trianglesIdx);
			}
			//BTDs[_bonesIdx].tris = item.triangles;
			
			for(var _item: Color in item.colors)
			{    
			    colors.Add(_item);
			}
			
			for(var _item: Vector3 in item.normals)
			{    
			    normals.Add(_item);
			}
			
			vertIdx = item.vertices.Length;
			idx = 0;
			__weights  = new BoneWeight[vertIdx];
			
			for ( idx = 0; idx<vertIdx; ++idx )
			{    
			    __weights[idx].boneIndex0 = _bonesIdx;
			    __weights[idx].weight0 = 1.0;
			    weights.Add(__weights[idx]);
			}
			
			newMesh.vertices = vertices.ToArray();
			trianglesIdx = vertices.Count;
			gameObjects[_bonesIdx].transform.localPosition = transforms[_bonesIdx].localPosition;
			bonesA[_bonesIdx] = gameObjects[_bonesIdx].transform;
			bonesA[_bonesIdx].parent = GO.transform;
			bonesA[_bonesIdx].localRotation = Quaternion.identity;
			bonesA[_bonesIdx].localPosition = Vector3.zero;
			bindPosesA[_bonesIdx] = bonesA[_bonesIdx].worldToLocalMatrix * GO.transform.localToWorldMatrix;
			gameObjects[_bonesIdx].transform.localPosition = transforms[_bonesIdx].localPosition;
			gameObjects[_bonesIdx].transform.localRotation = transforms[_bonesIdx].localRotation;
			gameObjects[_bonesIdx].transform.localScale = transforms[_bonesIdx].localScale;
			++_bonesIdx;
		}
            
            newMesh.uv = uvList.ToArray();
            newMesh.uv2 = uv2List.ToArray();
            newMesh.triangles = triangles.ToArray();
            newMesh.colors = colors.ToArray();
            newMesh.normals = normals.ToArray();
            //Debug.Log(newMesh.normals[10]);
            //newMesh.RecalculateNormals();
            newMesh.boneWeights = weights.ToArray();
            newMesh.bindposes = bindPosesA;
            newMesh.RecalculateBounds();    
            ;
            //mesh = newMesh;
            GO.AddComponent.<SkinnedMeshRenderer>();
            var _renderer : SkinnedMeshRenderer = GO.GetComponent.<SkinnedMeshRenderer>();
            _renderer.bones = bonesA;
            _renderer.sharedMaterial  = mat;
            _renderer.sharedMesh = newMesh;
            _renderer.quality = SkinQuality.Bone1;
            BTHelper.ExportSkinnedMesh( GO, dir );
            var ttt : Transform;
            var i:int=0;
            for ( ttt in transformsT)
            {

            	gameObjects[i].transform.parent = ttt;
            	
            	if(ttt == gameObjects[i].transform)
            	{
            		gameObjects[i].transform.position = pos[i];
            		gameObjects[i].transform.rotation = rot[i];
            	}
            	else
            	{
            		gameObjects[i].transform.localPosition = locPos[i];
            		gameObjects[i].transform.localRotation = locRot[i];
            	}

            	++i;

            }
            
//            for ( var mes: Mesh in _meshes )
//           {
//           	Debug.Log(mes.name);
//           }
            return GO;
	}

	
	var inset : float = 0.0;
	var showExtraOptions : boolean;
	var combinedObjectsName :String = "level part 1";
	var maximumAtlasSize :int=1024;
	var padding:int=0;
	//popups
	var resOptions : String[] = ["4096", "2048", "1024", "512", "256", "128", "64", "32"];
	var resIndex : int = 2;//default 1024
	var comOptions : String[] = ["Combine materials"];
	var comTooltip : String[] = ["Combine materials is used to combine many materials from many gameObjects that share the same shader into one material\nUse it with unity built in batching"];
	var comIndex : int = 0; //default Combine materials
			
	function OnGUI() 
	{
		GUILayout.BeginHorizontal();
			GUILayout.Label ("",GUILayout.Width(2) );
			
			if(GUILayout.Button(GUIContent( comOptions[comIndex], comTooltip[comIndex]), GUILayout.Width(180), GUILayout.Height(27)))
			{
				Undo.RegisterSceneUndo("");
				var  selectionD : Transform[] = Selection.GetTransforms(	SelectionMode.Deep );
				var topLevelS : Transform[] = Selection.GetTransforms(	SelectionMode.TopLevel );
				
				if( combinedObjectsName  =="")
				{
					EditorUtility.DisplayDialog("No name found!", "Please fill a name in the Combined Objects name field. Thanks", "");
	    			return;
	    		}	
				
				SeperateStaticDynamic( selectionD, topLevelS );
			}

		GUILayout.EndHorizontal();
		GUILayout.Space (1);
		showExtraOptions  = EditorGUILayout.Foldout(showExtraOptions, GUIContent("Options","Extra options regarding maximum texture size, \n padding between the texture\n and naming of combined objects"));
		
		if(showExtraOptions)
		{
			this.position = Rect(this.position.x,this.position.y, 200,270);
			
			#if UNITY_3_5 
			GUILayout.Space (10);
			#endif
			#if UNITY_3_4||UNITY_3_3||UNITY_3_2||UNITY_3_1||UNITY_3_0_0
			GUILayout.Space (5);
			#endif
			
			GUILayout.Label(GUIContent("                  Skinned Mesh", "The name of the combined object's container"));
			GUILayout.BeginHorizontal();
			GUILayout.Label ("",GUILayout.Width(30) );
			if(GUILayout.Button(GUIContent( "+", comTooltip[comIndex]), GUILayout.Width(50), GUILayout.Height(15)))
			{
				Undo.RegisterSceneUndo("");
				BTHelper.AddBTD();
				//BTDynamicCombine();
			}
			GUILayout.Label ("",GUILayout.Width(3) );
			if(GUILayout.Button(GUIContent( "-", comTooltip[comIndex]), GUILayout.Width(50), GUILayout.Height(15)))
			{
				Undo.RegisterSceneUndo("");
				BTHelper.RemoveBTD();
				//BTDynamicCombine();
			}
			GUILayout.EndHorizontal();
			
			var rect: Rect = Rect( 8, 60, 193, 260 );
			Handles.color = Color(0.2,0.2,0.2,0.7);
			Handles.DrawLine(Vector3(rect.x, rect.y, 0), Vector3(rect.width, rect.y, 0));
			Handles.DrawLine(Vector3(rect.x, rect.height, 0), Vector3(rect.width, rect.height, 0));
			Handles.DrawLine(Vector3(rect.x, rect.y, 0), Vector3(rect.x, rect.height, 0));
			Handles.DrawLine(Vector3(rect.width, rect.y, 0), Vector3(rect.width, rect.height,0));
			Handles.color = Color(0.1,0.1,0.1,0.4);
			
			#if UNITY_3_5 
			Handles.DrawLine(Vector3(25,110,0), Vector3(175,110,0));
			Handles.DrawLine(Vector3(25,165,0), Vector3(175,165,0));
			Handles.DrawLine(Vector3(25,210,0), Vector3(175,210,0));
			#endif
			#if UNITY_3_4||UNITY_3_3||UNITY_3_2||UNITY_3_1||UNITY_3_0_0
			Handles.DrawLine(Vector3(25,110,0), Vector3(175,110,0));
			Handles.DrawLine(Vector3(25,165,0), Vector3(175,165,0));
			Handles.DrawLine(Vector3(25,210,0), Vector3(175,210,0));
			#endif
			
				#if UNITY_3_5 
			GUILayout.Space (18);
			#endif
			#if UNITY_3_4||UNITY_3_3||UNITY_3_2||UNITY_3_1||UNITY_3_0_0
			GUILayout.Space (9);
			#endif
			
				GUILayout.Label(GUIContent("          Combined Objects Name", "The name of the combined object's container"));
				GUILayout.Space (2);
				GUILayout.BeginHorizontal();
					GUILayout.Label ("",GUILayout.Width(18) );//positioning hack
					combinedObjectsName = EditorGUILayout.TextField (combinedObjectsName, GUILayout.Width(150));
				GUILayout.EndHorizontal();
			
			#if UNITY_3_5 
			GUILayout.Space (15);
			#endif
			#if UNITY_3_4||UNITY_3_3||UNITY_3_2||UNITY_3_1||UNITY_3_0_0
			GUILayout.Space (7);
			#endif
						
				GUILayout.BeginVertical();
					GUILayout.Label(GUIContent("             Maximum Atlas Size", "The maximum size of the produced atlas texture\nUse 1024 or smaller for best perfomance"), GUILayout.Width(168));
					GUILayout.BeginHorizontal();
						GUILayout.Label ("",GUILayout.Width(108) );//positioning hack
						resIndex = EditorGUILayout.Popup( resIndex, resOptions, GUILayout.Width(60) );
						maximumAtlasSize = int.Parse( resOptions[ resIndex ] );//EditorGUILayout.IntField( maximumAtlasSize, GUILayout.Width(50));
					GUILayout.EndHorizontal();
				GUILayout.EndVertical();

				
			
			#if UNITY_3_5 
			GUILayout.Space (18);
			#endif
			#if UNITY_3_4||UNITY_3_3||UNITY_3_2||UNITY_3_1||UNITY_3_0_0
			GUILayout.Space (9);
			#endif
			
				GUILayout.BeginHorizontal();
					GUILayout.Label ("",GUILayout.Width(27) );
				
					if( GUILayout.Button( GUIContent( "Expand submeshes", "Expands each submesh from a mesh\ninto one seperate mesh"), GUILayout.Width(128), GUILayout.Height(23)))
					{
						Undo.RegisterSceneUndo("");
						BTHelper.ExpandSubmeshes();
					}
					EditorUtility.ClearProgressBar();
				GUILayout.EndHorizontal();
		}
		else
		{
			this.position = Rect(this.position.x,this.position.y, 200,60);
		}
	}
	
	function OnInspectorUpdate() 
	{
		Repaint();
	}
	
	@MenuItem("Window/Batching Tools")
	static function Execute() 
	{
		//as BatchingTools; fixes
		//BCW0028: WARNING: Implicit downcast from 'UnityEngine.ScriptableObject' to 'BatchingTools'.
		var window : BatchingTools= ScriptableObject.CreateInstance(BatchingTools) as BatchingTools;
		window.ShowUtility();//This lets it sit on top
		window.position = Rect(600,200, 210,200);//Defines the position and the size of the window 
		// 50 +40
	}
}
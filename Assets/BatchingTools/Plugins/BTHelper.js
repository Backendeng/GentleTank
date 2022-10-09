// prod : Batching Tools v 1.4
// name : BTHelper.js
// auth : Ippokratis Bournells
// 
// 
//
#pragma strict
#pragma downcast
#if UNITY_EDITOR
import System.IO;
import System.Collections.Generic;
import System.Linq;

static function BTWriteAllBytes( path:String,  bytes : System.Byte[] ):void
{
	var fs : FileStream = FileStream(path , FileMode.CreateNew);
	var w : BinaryWriter = BinaryWriter(fs);
	w.Write(bytes);
	w.Close();
	fs.Close();
}
 
static function ExportMaterial(matIn:Material, froot:String, _combineBump : boolean, _combinedObjectsName:String):Material
	{
		var dir : String = BTHelper.MakeUniqueFolderInFolder(froot, _combinedObjectsName);
		var path : String = dir + "/" + _combinedObjectsName;
		//main texture
		EditorUtility.DisplayProgressBar("BatchingTools Tools", "Saving main texture", 0.7);
		var bytes : System.Byte[];
		var packedTexture : Texture2D = new Texture2D(matIn.mainTexture.width, matIn.mainTexture.height, TextureFormat.ARGB32, false);
		packedTexture = matIn.mainTexture as Texture2D;
		bytes = packedTexture.EncodeToPNG();
		
	   	
	   	BTWriteAllBytes( path + ".png", bytes);
	   	
	   	EditorUtility.DisplayProgressBar("Batching Tools", "Saving main texture ( takes a while )", 0.75);
	   	AssetDatabase.Refresh();
		var textureImporter : TextureImporter ;
		EditorUtility.DisplayProgressBar("Batching Tools", "Saving main texture ( modifying the asset )", 0.80);
		textureImporter = AssetImporter.GetAtPath( path + ".png") as TextureImporter;
		textureImporter.isReadable = false;
	    textureImporter.mipmapEnabled = true;
	    textureImporter.wrapMode = TextureWrapMode.Clamp;
	    textureImporter.filterMode = FilterMode.Bilinear;
	    textureImporter.textureFormat = TextureImporterFormat.ARGB32;
	    EditorUtility.DisplayProgressBar("Batching Tools", "Saving main texture ( second reimporting )", 0.85);
		AssetDatabase.ImportAsset(path + ".png", ImportAssetOptions.ForceUpdate);

		//bump map
		if(_combineBump)
		{
			EditorUtility.DisplayProgressBar("Batching Tools", "Saving bump texture", 0.8);
			var bytesBump : System.Byte[];
			var packedTextureBump : Texture2D = new Texture2D(matIn.GetTexture("_BumpMap").width, matIn.GetTexture("_BumpMap").height, TextureFormat.ARGB32, false);
			packedTextureBump = matIn.GetTexture("_BumpMap") as Texture2D;
			bytesBump = packedTextureBump.EncodeToPNG();
		   	BTWriteAllBytes( path + "_Bump.png", bytesBump );
		   	EditorUtility.DisplayProgressBar("Batching Tools", "Saving bump texture", 0.85);
		   	AssetDatabase.Refresh();
			var textureImporterBump  : TextureImporter ;
			textureImporterBump  = AssetImporter.GetAtPath( path + "_Bump.png") as TextureImporter;
			textureImporterBump.isReadable = false;
		    textureImporterBump.mipmapEnabled = true;
		    textureImporterBump.wrapMode = TextureWrapMode.Clamp;
		    textureImporterBump.filterMode = FilterMode.Bilinear;
		    textureImporterBump.textureFormat = TextureImporterFormat.ARGB32;
		    textureImporterBump.normalmap = false;
		    EditorUtility.DisplayProgressBar("Batching Tools", "Saving bump texture", 0.90);
		   	AssetDatabase.ImportAsset(path + "_Bump.png", ImportAssetOptions.ForceUpdate);
	    }
	    //material
	    EditorUtility.DisplayProgressBar("Batching Tools", "Saving material", 0.9);
	    var material = new Material(matIn);
		AssetDatabase.CreateAsset(material, path + ".mat");
		AssetDatabase.Refresh();
		var matOut : Material = AssetDatabase.LoadAssetAtPath(path + ".mat", Material);
		var tex : Texture2D = AssetDatabase.LoadAssetAtPath(path + ".png", Texture2D);
		matOut.SetTexture("_MainTex", tex);
		
		if(_combineBump)
		{
			var texBump : Texture2D = AssetDatabase.LoadAssetAtPath(path + "_Bump.png", Texture2D);
			matOut.SetTexture("_BumpMap", texBump);
		}
		
		EditorUtility.DisplayProgressBar("Batching Tools", "Refreshing the project", 0.95);
	   	AssetDatabase.Refresh();
	   	EditorUtility.DisplayProgressBar("Batching Tools", "Saving all assets", 0.7);
	    AssetDatabase.SaveAssets();
	   	return matOut;
	}
	
static function ExpandSubmeshes()
	{
		if(Selection.gameObjects.Length!=1)
		{
			EditorUtility.DisplayDialog("Please select one gameObject", "You should select one gameObject that contains a mesh with submeshes. Please try again", "");
			return;
		}
		
		var _mf:Mesh = new Mesh();
		_mf= Selection.gameObjects[0].GetComponent.<MeshFilter>().sharedMesh;
		
		if(_mf.subMeshCount<2)
		{
			EditorUtility.DisplayDialog("Please select one gameObject with submeshes", "You should select one gameObject that contains a mesh with submeshes. Those are represented in Unity as a gameObject with one mesh and many materials. Please try again", "");
			return;
		}
		
		
		var Orig:GameObject = Instantiate( Selection.gameObjects[0]);
		Orig.name = "Original_" + Selection.gameObjects[0].name;
		Orig.transform.parent = Orig.transform;
		Orig.SetActive(false);
		var _newMesh : Mesh = BTHelper.CopyMesh(_mf);
	    var mfSubMeshCount : int = _newMesh.subMeshCount;
		var offset_Newmesh : int = 0;
		var offset  : int = 0;
	    
	    for (var i:int = 0; i<mfSubMeshCount; ++i)
		{

			var newMesh : Mesh = new Mesh();
			newMesh.name = _newMesh.name;
    		var triangles : int[] = _newMesh.GetTriangles(i);
    		var trianglesLength : int = triangles.Length;
			
    		var newVertices : Vector3[] = new Vector3[trianglesLength];
    		var newUvs :  Vector2[] = new Vector2[trianglesLength];
//->
    		var newNormals : Vector3[] = new Vector3[trianglesLength];
    		var newTangents : Vector4[] = new Vector4[trianglesLength];
    		
    		var oldToNewIndices : int[] = new int[_newMesh.vertices.Length];
    		var newIndex : int = 0;
    		var v :Vector3[]= _newMesh.vertices;
    		var uv :Vector2[]= _newMesh.uv;
//->
    		var normals : Vector3[] = _newMesh.normals;
    		var tangents: Vector4[] = _newMesh.tangents;
			
			// Collect vertices, uvs. triangles
			var vertexIndex : int=0;
     		var k : int= 0;
     		var verticesDictionary : Dictionary.<int, Vector3> = new Dictionary.<int, Vector3>(trianglesLength);
     		var dummy : Vector3 = Vector3(0.0,0.0,0.0);
    		for ( k = 0; k < trianglesLength; ++k)//do it for every triangle indice
    		{
        		vertexIndex = triangles[k];//vertex Index = triangle index
        		if( oldToNewIndices[vertexIndex] == 0 )//for every triangle indice 
        		{
		            newVertices[newIndex] = v[vertexIndex];// new index 0 ->  newVertices[0] = oldMesh.vertices[0]
		            newUvs[newIndex] = uv[vertexIndex];
//->
					newNormals[newIndex] = normals[vertexIndex];
					newTangents[newIndex] = tangents[vertexIndex];		  
					          
		            oldToNewIndices[vertexIndex] = newIndex + 1;//oldToNewIndices[0] = 
		            ++newIndex;
        		}
        		if( !verticesDictionary.ContainsKey( triangles[k]) )
				{
					verticesDictionary.Add(triangles[k], dummy);
				}	
    		}

 		
			var newTriangles : int[] = new int[trianglesLength];
    		var newTrianglesLength = newTriangles.Length;
 
		    // Collect the new triangles indices
		    for ( k = 0; k < newTrianglesLength; ++k)
		    {
		        newTriangles[k] = oldToNewIndices[triangles[k]] - 1;
		    }
		    var newVerticesTrimmed : Vector3[] = new Vector3[ verticesDictionary.Count];
		    var newUvsTrimmed : Vector2[] = new Vector2[ verticesDictionary.Count];
//->		    
		    var newNormalsTrimmed : Vector3[] = new Vector3[ verticesDictionary.Count];
		    var newTangentsTrimmed : Vector4[] = new Vector4[ verticesDictionary.Count];
		    
		    System.Array.Copy( newVertices, newVerticesTrimmed,verticesDictionary.Count);
		    System.Array.Copy( newUvs, newUvsTrimmed, verticesDictionary.Count);
//->
		    System.Array.Copy( newNormals, newNormalsTrimmed, verticesDictionary.Count); 
		    System.Array.Copy( newTangents, newTangentsTrimmed, verticesDictionary.Count);
		    
		    for ( var item : Vector3 in newVerticesTrimmed)
		    {
		    	item = Selection.gameObjects[0].transform.TransformPoint(item);
		    }
		    // Assemble the new mesh with the new vertices/uv/triangles.
		    newMesh.vertices = newVerticesTrimmed;
		    newMesh.uv = newUvsTrimmed;
		    newMesh.triangles = newTriangles;
			newMesh.normals = newNormalsTrimmed;
		    newMesh.tangents = newTangentsTrimmed;
		    newMesh.RecalculateNormals();
			newMesh.RecalculateBounds();
			BTHelper.MakeFoldersIfNotYetThere("Assets", "BatchingToolsPrefabs");
			newMesh = BTHelper.ExportMesh(newMesh, newMesh.name, "Assets/BatchingToolsPrefabs");
			var mat1 : Material = new Material(Selection.gameObjects[0].GetComponent.<MeshRenderer>().sharedMaterials[i]);
			var CM : GameObject = new GameObject("submesh " + i.ToString() + " of " + Selection.gameObjects[0].name, MeshRenderer, MeshFilter);
			CM.transform.parent = Selection.gameObjects[0].transform;
			CM.GetComponent.<MeshRenderer>().sharedMaterial =  mat1;
			CM.GetComponent.<MeshFilter>().sharedMesh = newMesh;
		}
		
		DestroyImmediate(Selection.gameObjects[0].GetComponent.<MeshFilter>());
		DestroyImmediate(Selection.gameObjects[0].GetComponent.<MeshRenderer>());
}	

static function MakeFoldersIfNotYetThere( froot:String,fname:String) :String
{
	var p : String = froot + "/" + fname;
	var newFolderPath : String ;
	var myGuid : String ;
	if ( !System.IO.Directory.Exists(p) )
	{
		myGuid  = AssetDatabase.CreateFolder( froot, fname );
		newFolderPath= AssetDatabase.GUIDToAssetPath( myGuid );
		AssetDatabase.Refresh();
	}
	return;
}

static function MakeUniqueFolderInFolder(froot:String, fName:String):String
{
	var folderName : String = fName;
	var guid : String = AssetDatabase.CreateFolder(froot, folderName);
	var dir : String = AssetDatabase.GUIDToAssetPath(guid);
	AssetDatabase.Refresh();
	return dir;
}

static function ExportMesh(go:GameObject, froot:String)
{
	var dir : String = MakeUniqueFolderInFolder(froot, go.name);
	var mf :MeshFilter = go.GetComponent.<MeshFilter>();
	
	var path : String = dir + "/" + go.name ;//+ i.ToString();
	var mesh1 : Mesh = mf.sharedMesh;
	AssetDatabase.CreateAsset(mesh1, path + "_mesh.asset");
	AssetDatabase.Refresh();
	AssetDatabase.ImportAsset(path + "_mesh.asset", ImportAssetOptions.ForceUpdate);

	var mesh : Mesh = new Mesh();
	mesh = AssetDatabase.LoadAssetAtPath(path + "_mesh.asset", Mesh);
	go.GetComponent.<MeshFilter>().sharedMesh = mesh;
	
   	AssetDatabase.Refresh();
    AssetDatabase.SaveAssets();
   	return;
}

static function ExportSkinnedMesh(go:GameObject, froot:String)
{
	var dir : String = MakeUniqueFolderInFolder(froot, go.name);
	var mf : SkinnedMeshRenderer = go.GetComponent.<SkinnedMeshRenderer>();
	
	var path : String = dir + "/" + go.name ;//+ i.ToString();
	var mesh1 : Mesh = mf.sharedMesh;
	AssetDatabase.CreateAsset(mesh1, path + "_SKmesh.asset");
	AssetDatabase.Refresh();
	AssetDatabase.ImportAsset(path + "_SKmesh.asset", ImportAssetOptions.ForceUpdate);

	var mesh : Mesh = new Mesh();
	mesh = AssetDatabase.LoadAssetAtPath(path + "_SKmesh.asset", Mesh);
	go.GetComponent.<SkinnedMeshRenderer>().sharedMesh = mesh;
	
   	AssetDatabase.Refresh();
    AssetDatabase.SaveAssets();
   	return;
}

static function ExportMesh(goMesh:Mesh, fname :String, froot:String):Mesh
{
	var dir : String = MakeUniqueFolderInFolder(froot, fname);
	var path : String = dir + "/" + fname ;
	AssetDatabase.CreateAsset(goMesh, path + "_mesh.asset");
	AssetDatabase.Refresh();
	AssetDatabase.ImportAsset(path + "_mesh.asset", ImportAssetOptions.ForceUpdate);

	var mesh : Mesh = new Mesh();
	mesh = AssetDatabase.LoadAssetAtPath(path + "_mesh.asset", Mesh);
   	AssetDatabase.Refresh();
    AssetDatabase.SaveAssets();
   	return mesh;
}

static function ExportGO(go:GameObject, froot:String)
{
	var path : String = froot + "/" + go.name + ".prefab";
//	#if UNITY_3_4||UNITY_3_3||UNITY_3_2||UNITY_3_1||UNITY_3_0_0
//	var prefab : UnityEngine.Object = EditorUtility.CreateEmptyPrefab(path);
//	EditorUtility.ReplacePrefab(go, prefab);
//	#endif
//	#if UNITY_3_5 
	var prefab : UnityEngine.Object = PrefabUtility.CreateEmptyPrefab(path);
	PrefabUtility.ReplacePrefab(go, prefab);
//	#endif
	
	AssetDatabase.Refresh();
	DestroyImmediate(go);
	
//	#if UNITY_3_4||UNITY_3_3||UNITY_3_2||UNITY_3_1||UNITY_3_0_0
//	var killID: int = EditorUtility.InstantiatePrefab(prefab).GetInstanceID();
//	#endif
//	#if UNITY_3_5 
	var killID: int = PrefabUtility.InstantiatePrefab(prefab).GetInstanceID();
//	#endif
	//Instantiate (prefab, Vector3(0.0, 0.0, 0.0), Quaternion.identity);
	
	//Remove the tiles
	FileUtil.DeleteFileOrDirectory("Assets/BatchingToolsTemp"); 
	AssetDatabase.Refresh();
	AssetDatabase.SaveAssets();
}

static function CopyMesh(inputMesh : Mesh):Mesh
{
	var newMesh = new Mesh();
	var i:int = 0;
	newMesh.name = inputMesh.name;
	newMesh.vertices = inputMesh.vertices;
	newMesh.triangles = inputMesh.triangles;
	newMesh.subMeshCount = inputMesh.subMeshCount;
	for (i=0;i<inputMesh.subMeshCount; ++i)
	{
		newMesh.SetTriangles( inputMesh.GetTriangles(i), i);
	}
	
	newMesh.uv = inputMesh.uv;
		
	if ( inputMesh.normals.Length == inputMesh.vertices.Length )
	{
		newMesh.normals = inputMesh.normals;
	}
	else
	{
		newMesh.normals = new Vector3[inputMesh.vertices.Length];
		newMesh.RecalculateNormals();
	}
	
	if ( inputMesh.uv2.Length == inputMesh.vertices.Length )
	{
		newMesh.uv2 = inputMesh.uv2;
	}
	else
	{
		Unwrapping.GenerateSecondaryUVSet (newMesh);
	}
	
	if  ( inputMesh.tangents.Length == inputMesh.vertices.Length )
	{
		newMesh.tangents = inputMesh.tangents;
	}
	if  ( inputMesh.colors.Length == inputMesh.vertices.Length )
	{
		newMesh.colors = inputMesh.colors;
	}
	newMesh.RecalculateBounds();
	;
	return newMesh;
}


static function CopyMeshParts(fromMesh : Mesh, toMesh: Mesh, offset :int):Mesh
{
	
	toMesh.name = fromMesh.name;
	var i : int = 0;
	var k : int = 0;
		
	if ( fromMesh.normals.Length == fromMesh.vertices.Length )
	{
		var normals :Vector3[] = new Vector3[toMesh.vertices.Length];
		k = offset;

		for( i = 0; i< toMesh.vertices.Length; ++i )
		{
			normals[i] = fromMesh.normals[k];
			++k;
		}
		
		toMesh.normals = normals;
	}
	else
	{
		toMesh.RecalculateNormals();
	}
	
	
	if ( fromMesh.uv2.Length == fromMesh.vertices.Length )
	{

		var uv2 :Vector2[] = new Vector2[toMesh.vertices.Length];
		k = offset;

		for( i = 0; i< toMesh.vertices.Length; ++i )
		{
			uv2[i] = fromMesh.uv2[k];
			++k;
		}
		
		toMesh.uv2 = uv2;
	}
	else
	{
		Unwrapping.GenerateSecondaryUVSet (toMesh);
	}
	
	if  ( fromMesh.tangents.Length == fromMesh.vertices.Length )
	{
		var tangents : Vector4[] = new Vector4[toMesh.vertices.Length];
		k = offset;

		for( i = 0; i< toMesh.vertices.Length; ++i )
		{
			
			tangents[i] = fromMesh.tangents[k];
			++k;
		}
		
		toMesh.tangents = tangents;
	}

	
	
	if  ( fromMesh.colors.Length == fromMesh.vertices.Length )
	{
		toMesh.colors = new Color[toMesh.vertices.Length];
		k = offset;

		for( i = 0; i< toMesh.vertices.Length; ++i )
		{
			toMesh.colors[i] = fromMesh.colors[k];
			++k;
		}
	}
	
	toMesh.RecalculateBounds();
	;
	return toMesh;
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

static function AddBTD()
{
	var selection : Transform[]  = Selection.GetTransforms( SelectionMode.Deep);
	for (var item : Transform in selection)
	{
		if  ( item.gameObject.GetComponent.<MeshFilter>()!=null )
		{
			if  ( item.gameObject.GetComponent.<BTD>()==null)
			{
				item.gameObject.AddComponent.<BTD>();
			}
		}
	} 
}

static function RemoveBTD()
{
	var selection : Transform[]  = Selection.GetTransforms( SelectionMode.Deep);
	for (var item : Transform in selection)
	{
		if  (item.gameObject.GetComponent.<MeshFilter>()!=null)
		{
			if  (item.gameObject.GetComponent.<BTD>()!=null)
			{
				DestroyImmediate( item.gameObject.GetComponent.<BTD>());
			}
		}
	} 
}
	
static function SelectShader(shaderName:String):BTShader
{

	var mat1 : Material;
	var combineBump : boolean = false;
	var bakeColorsToVertex : boolean = false;
	
	switch(shaderName)
	{
		case "BT/SelfIllum":
			mat1 = new Material(Shader.Find("BT/BT SelfIllum"));
			bakeColorsToVertex = true;
			break;
			
		case "BT/Transparent SelfIllum":
			mat1 = new Material(Shader.Find("BT/BT Transparent SelfIllum"));
			bakeColorsToVertex = true;
			break;	
							
		case "My Shaders/Self Illumination":
			mat1 = new Material(Shader.Find(shaderName));
			break;	
				
		case "Bumped Diffuse":
			mat1 = new Material(Shader.Find("BatchingTools/Bumped Diffuse"));
			combineBump = true;
			bakeColorsToVertex = true;
			break;
			
		case "Bumped Specular":
			mat1 = new Material(Shader.Find("BatchingTools/Bumped Specular"));
			combineBump = true;
			bakeColorsToVertex = true;
			break;	
		
		case "Diffuse":
			mat1 = new Material(Shader.Find("BatchingTools/Diffuse"));
			bakeColorsToVertex = true;
			break;
			
		case "Transparent/Diffuse":
			mat1 = new Material(Shader.Find("BatchingTools/Transparent Diffuse"));
			bakeColorsToVertex = true;
			break;
			
		case "Unlit/Texture":
			mat1 = new Material(Shader.Find(shaderName));
			break;
			
		case "Unlit/Transparent":
			mat1 = new Material(Shader.Find(shaderName));
			break;
			
		case "Mobile/Diffuse":
			mat1 = new Material(Shader.Find(shaderName));
			break;
			
		case "Mobile/Unlit (Supports Lightmap)":
			mat1 = new Material(Shader.Find(shaderName));
			break;
					
		case "Mobile/Bumped Diffuse":
			mat1 = new Material(Shader.Find(shaderName));
			break;
			
		case "Mobile/Bumped Specular":
			mat1 = new Material(Shader.Find(shaderName));
			break;
			
		case "Mobile/Bumped Specular (1 Directional Light)":
			mat1 = new Material(Shader.Find(shaderName));
			break;

		case "Mobile/Particles/Additive":
			mat1 = new Material(Shader.Find(shaderName));
			break;
			
		case "Mobile/Particles/Alpha Blended":
			mat1 = new Material(Shader.Find(shaderName));
			break;
			
		case "Mobile/Particles/Multiply":
			mat1 = new Material(Shader.Find(shaderName));
			break;
			
		case "Mobile/Particles/VertexLit":
			mat1 = new Material(Shader.Find(shaderName));
			break;
			
		case "Mobile/Particles/VertexLit (Only Directional Lights)":
			mat1 = new Material(Shader.Find(shaderName));
			break;		
						
		default:
			EditorUtility.DisplayDialog("Unsupported shader found !", "Please only select gameObjects that have supported shaders. Shader " + shaderName + " is not currently supported. Please try again using one of the supported shaders. Thanks", "");
    		return;

    }
    
    var _BTShader : BTShader = ScriptableObject.CreateInstance.<BTShader>();
    _BTShader.mat = mat1;
    _BTShader.combineBump = combineBump;
    _BTShader.bakeColorsToVertex = bakeColorsToVertex;
    return _BTShader;
}

static function MakeTile(textureName : String, matColor : Color, combineBump : boolean):Texture2D
{
	var textureWidth : int = 4;
	var textureHeight : int = 4;
	var p : String = "Assets/BatchingToolsTemp";
	var newFolderPath : String ;
	var myGuid : String;
	var dir : String;
	
	if (!System.IO.Directory.Exists(p))
	{
		myGuid = AssetDatabase.CreateFolder("Assets", "BatchingToolsTemp");
		AssetDatabase.Refresh();
		dir  = "Assets/BatchingToolsTemp";			
	}
	else
	{
		dir  = "Assets/BatchingToolsTemp";
	}	
	
	var i :int = 0;
	var backspace : boolean;
	var path : String = dir + "/" + textureName ;
	var filenamePath :String = path + ".png";
	
	while(System.IO.File.Exists(filenamePath))
	{
		++i;
		if (backspace)
		{
			if(i<10)
			{
				path = path.Remove(path.Length - 1);
			}	
			else
			{
				if(i<100)
				{
					path = path.Remove(path.Length - 2);
				}	
				else
				{
					if(i<1000)
					{
						path = path.Remove(path.Length - 3);
					}
					else
					if(i<10000)	
					{
						path = path.Remove(path.Length - 4);
					}
					else
					{
						if( i < 100000)
						{
							path = path.Remove(path.Length - 5);
						}
					}
				}	
			}
		}
		
		path = path + i.ToString();
		filenamePath = path + ".png";
		backspace = true;
	}
	
	var color : Color32 = matColor;
	var texture : Texture2D = new Texture2D ( textureWidth, textureHeight, TextureFormat.ARGB32, false);
	var colors : Color32[] = texture.GetPixels32(0);
	
	for ( var item :Color32 in colors)
	{
		item = color;
	}
	
	texture.SetPixels32( colors, 0 );
	texture.Apply( false );
	
	var bytes : System.Byte[];
	bytes = texture.EncodeToPNG();
	
   	BTWriteAllBytes( path + ".png", bytes);
   	AssetDatabase.Refresh();
	var textureImporter : TextureImporter ;
	textureImporter = AssetImporter.GetAtPath( path + ".png") as TextureImporter;
	textureImporter.isReadable = true;
    textureImporter.mipmapEnabled = false;
    textureImporter.textureFormat = TextureImporterFormat.ARGB32;
    
    if(combineBump)
    {
    	textureImporter.normalmap = true;
    }
    
    AssetDatabase.ImportAsset(path + ".png", ImportAssetOptions.ForceUpdate );
    AssetDatabase.SaveAssets();
    texture =AssetDatabase.LoadAssetAtPath(path + ".png", Texture2D);
    return texture;
}

static function BakeSize (mf : MeshFilter): MeshFilter
{
    var vertices : Vector3[];
    var triangles : int[];
    var tangents : Vector4[];
    var colors  : Color[];
    var normals : Vector3[];
    var uv : Vector2[];
    var uv2 : Vector2[];
    var pos : Vector3;
    var rot : Quaternion;
	var GO : GameObject = mf.gameObject;
	var mfName : String;

    vertices = new Vector3[mf.sharedMesh.vertices.length];
    vertices = mf.sharedMesh.vertices;
    triangles = new int[mf.sharedMesh.triangles.length];
    triangles = mf.sharedMesh.triangles;
    tangents = new Vector4[mf.sharedMesh.tangents.length];
    tangents = mf.sharedMesh.tangents;
    uv = new Vector2[mf.sharedMesh.uv.length];
    uv = mf.sharedMesh.uv;
    uv2 = new Vector2[mf.sharedMesh.uv2.length];
    uv2 = mf.sharedMesh.uv2;
    colors = new Color[mf.sharedMesh.colors.length];
    colors = mf.sharedMesh.colors;
    normals = new Vector3[mf.sharedMesh.normals.length];
    normals = mf.sharedMesh.normals;
    mfName = mf.sharedMesh.name;
   //Process data
    pos = mf.gameObject.transform.position;
    rot = mf.gameObject.transform.rotation;
    mf.gameObject.transform.rotation = Quaternion.identity;
    mf.gameObject.transform.position = Vector3(0.0,0.0,0.0);
    var thisMatrix : Matrix4x4 = mf.gameObject.transform.localToWorldMatrix;

    for (var i : int = 0; i < vertices.Length; i++)
    {
       vertices[i] = thisMatrix.MultiplyPoint3x4(mf.sharedMesh.vertices[i]);
    }
    mf.gameObject.transform.position = pos;
    mf.gameObject.transform.rotation = rot;
    //Bake data
    //mf = null;
    DestroyImmediate (GO.GetComponent.<MeshFilter>());
    GO.AddComponent.<MeshFilter>();
    var _mf : MeshFilter = GO.GetComponent.<MeshFilter>();
    //_mf = GetComponent.<MeshFilter>();
    _mf.sharedMesh = new Mesh();
    _mf.sharedMesh.vertices = vertices;
    _mf.sharedMesh.triangles = triangles;
    _mf.sharedMesh.uv = uv;
    _mf.sharedMesh.uv2 = uv2;
    _mf.sharedMesh.colors = colors;
    _mf.sharedMesh.tangents = tangents;
    _mf.sharedMesh.normals = normals;
    _mf.sharedMesh.name = mfName;
    GO.transform.localScale = Vector3(1.0,1.0,1.0);
    return _mf;
}		
#endif
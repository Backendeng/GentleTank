// prod : Batching Tools v 1.4
// name : CombinedObjectsEditor.js
// auth : Ippokratis Bournells
// 
// 
//

@CustomEditor(CombinedObjects)
class CombinedObjectsEditor extends Editor 
{
	var batchOptionsStatic : String[] = ["Unity Batching", "BTStatic"];
	var batchOptionsDynamic : String[] = ["Unity Batching", "BTDynamic"];
	var batchTooltipStatic : String[] = ["Use unity built-in batching",
	  							"BTstatic batching is a custom batching method that \"stiches\" many meshes with many materials together into one mesh with one material\nby applying CombineChildren script on them"];
	  							
	var batchIndex : int; 
	
	function OnInspectorGUI() 
	{
		batchIndex = target.batchIndex;
		GUILayout.BeginVertical();
		GUILayout.Label(GUIContent("    Batching method", "Select Combine materials for using Unity built - in batching\nOr use BTstatic or BTdynamic custom batching solutions instead"));
		GUILayout.BeginHorizontal();
		GUILayout.Label ("",GUILayout.Width(7) );//positioning hack
		if(!target.isDynamic)
		{
			batchIndex = EditorGUILayout.Popup( batchIndex, batchOptionsStatic, GUILayout.Width(180) );
		}
		else
		{
			batchIndex = EditorGUILayout.Popup( batchIndex, batchOptionsDynamic, GUILayout.Width(180) );
		}	
		//One click hack
		if(target.batchIndex != batchIndex)
		{
			//batchIndexTemp = batchIndex;
			target.batchIndex = batchIndex;
			target.Batch();
		}
		
		if(GUILayout.Button ("Bake", GUILayout.Width( 80 ) ) )
		{
			target.batchIndex = batchIndex;
			target.Bake();
		}	
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
			
		//DrawDefaultInspector ();
		
	}
}
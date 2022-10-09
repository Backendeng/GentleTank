// prod : Batching Tools v 1.4
// name : OriginalObjectsEditor.js
// auth : Ippokratis Bournells
// 
// 
//

@CustomEditor(OriginalObjectsDynamic)
class OriginalObjectsDynamicEditor extends Editor 
{
	var fold : boolean;
	function OnInspectorGUI() 
	{
		GUILayout.BeginVertical(); 
			GUILayout.Space (10);
			GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if(GUILayout.Button(GUIContent("Reset", "Reverts to previous state "),
												GUILayout.Width(200), GUILayout.Height(25)))
				{
					Undo.RegisterSceneUndo("");
					target.DoReset();
				}
				GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		
			GUILayout.Space (5);
		GUILayout.EndVertical();
	//The following line causes Unity Editor to crash	
	//DrawDefaultInspector ();
		
	}
}
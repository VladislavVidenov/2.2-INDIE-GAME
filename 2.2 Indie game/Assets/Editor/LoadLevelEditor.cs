using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LoadLevelGame))]
public class LoadLevelEditor : Editor {

	public override void OnInspectorGUI () {
		DrawDefaultInspector ();

		LoadLevelGame loadlevel = (LoadLevelGame)target;

		if (GUILayout.Button("Build")) {
			loadlevel.LoadLevel();
		}
		if (GUILayout.Button("Delete")) {
			loadlevel.DeleteScene();
		}
	}
}

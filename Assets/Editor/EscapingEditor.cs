using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Escaping))]
public class EscapingEditor : Editor {

	void OnSceneGUI() {
		Escaping escape = (Escaping) target;
		
		if(escape.isEscaping){
			Handles.color = Color.white;
			Handles.DrawWireDisc(escape.transform.position , escape.transform.forward, escape.escapeDistance);
		}
	}
}

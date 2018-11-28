using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor (typeof (FieldOfView))]
public class FieldOfViewEditor : Editor {
	void OnSceneGUI() {
		FieldOfView fow = (FieldOfView)target;
		if(fow.GetComponent<Escaping>().isEscaping){
			Handles.color = Color.white;
			Handles.DrawWireDisc(fow.transform.position, fow.transform.forward, fow.viewRadius);

			Handles.color = Color.red;
			if(fow.visibleTarget!=null)
				Handles.DrawLine(fow.transform.position, fow.visibleTarget.position);
		}
		
	}
}

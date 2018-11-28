using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Following))]
public class FollowingEditor : Editor {
	
	void OnSceneGUI() {
		Following follow = (Following) target;
		if(follow.hasRadius){
			Handles.color = Color.green;
			Handles.DrawWireDisc(follow.transform.position , follow.transform.forward, follow.rangeRadius);
		}
	}
} 

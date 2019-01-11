using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour {
public GameObject SpawningCharacter;


	// Use this for initialization
	void Start () {
		SpawningCharacter.transform.position = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

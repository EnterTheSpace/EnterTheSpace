using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class nextLvl : MonoBehaviour {

public int next;
	// Use this for initialization
	void Start () {

	}
	
	private void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Player")
        {
            SceneManager.LoadScene("level_"+next);
        }
	}
	// Update is called once per frame
	void Update () {
		
	}
} 

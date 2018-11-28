using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNav;

public class Escaping : MonoBehaviour {

	[Header("Escape"), LabelOverride("Can escape ?")]
	[Tooltip("If ennemy can escape from object.")]
	public bool isEscaping = false;
	[SerializeField, LabelOverride("Escaping radius")]
	[Tooltip("Distance at which ennemy stops from object.")]
	public float escapeDistance = .2f;
	[SerializeField]
	private LayerMask obstacleMask; 

	public float stopDistance=.1f;

	public void Escape(GameObject player, PolyNavAgent nav){
		if(isEscaping){
			if(Vector2.Distance(transform.position,player.transform.position)<escapeDistance-stopDistance){
				Vector2 opposite = (transform.position-player.transform.position).normalized;

				RaycastHit2D ray = Physics2D.Raycast(transform.position, opposite + player.GetComponent<Rigidbody2D>().velocity,Mathf.Infinity ,obstacleMask);
				
				if(ray){
					Debug.DrawLine(transform.position, ray.point, Color.green);
					nav.SetDestination(ray.point);
				}
				print("Escaping");
			}
		}
	}
}

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

	/// <summary>
    /// NavAgent escape from Player.
    /// </summary>
    /// <param name="player">The player you want to escape from.</param>
    /// <param name="nav">The ennemy who escape from player.</param>
	public void Escape(GameObject player, PolyNavAgent nav){
		if(isEscaping){
			if(Vector2.Distance(transform.position,player.transform.position)<escapeDistance){
				Vector2 opposite = (transform.position-player.transform.position).normalized;

				RaycastHit2D ray = Physics2D.Raycast(transform.position, opposite + player.GetComponent<Rigidbody2D>().velocity,Mathf.Infinity ,obstacleMask);
				
				if(ray){
                    #if UNITY_EDITOR
                        Debug.DrawLine(transform.position, ray.point, Color.green);
                    #endif
                    nav.SetDestination(ray.point);
				}
                #if UNITY_EDITOR
                    print("Escaping");
                #endif
            }
        }
	}
}

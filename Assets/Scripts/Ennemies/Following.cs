using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolyNav;

public class Following : MonoBehaviour {

	[Header("Radius"),LabelOverride("Has radius ?")]
	public bool hasRadius = true;
	[Hide("hasRadius",true)]
	public float rangeRadius = 5f;
	[SerializeField, LabelOverride("Stopping distance"),Hide("hasRadius")]
	[Tooltip("Distance at which ennemy stops from object.")]
	public float minDistance = .2f;
	[SerializeField]
	private LayerMask targetMask;
	[SerializeField]
	private LayerMask obstacleMask;

	[HideInInspector]
	public bool isOverlapping;
	[HideInInspector]
	public bool isFollowingPlayer;
	[HideInInspector]
	public List<Transform> visibleTargets = new List<Transform>();

	public float Follow(GameObject player, PolyNavAgent nav, float currentTime) {
		float tmpCurrentTime = currentTime;
		visibleTargets.Clear ();
		Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, rangeRadius, targetMask);
		
		for (int i = 0; i < targetsInViewRadius.Length; i++) {
			if(hasRadius){
				print("Following");
				nav.SetDestination(player.transform.position);
				tmpCurrentTime = 0;
				isFollowingPlayer = true;
			}	
		}

		if(targetsInViewRadius.Length==0)
			isOverlapping=false;
		else
			isOverlapping=true;
		
		if(!hasRadius){	//Si le radius n'est pas activé
			nav.SetDestination(player.transform.position);
			tmpCurrentTime = 0;
			isFollowingPlayer = true;
		}

		return tmpCurrentTime;
	}

	
}

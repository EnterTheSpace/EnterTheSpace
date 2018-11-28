using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour {

	public float viewRadius;
	[SerializeField]
	private LayerMask targetMask;
	[SerializeField]
	private LayerMask obstacleMask;

	[HideInInspector]
	public Transform visibleTarget;

	public Transform FindVisibleTargets() {
		visibleTarget = null;
		Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);

		for (int i = 0; i < targetsInViewRadius.Length; i++) {
			Transform target = targetsInViewRadius[i].transform;
			Vector2 dirToTarget = (target.position - transform.position).normalized;
			float dstToTarget = Vector2.Distance (transform.position, target.position);

			if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask)) {
				visibleTarget = target;
			}
		}

		return visibleTarget;
	}
}

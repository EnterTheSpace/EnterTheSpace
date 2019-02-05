using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Overlapper : MonoBehaviour {

	[HideInInspector] public List<GameObject> overlapped;

	public GameObject ClosestObject()
	{
		GameObject bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach(GameObject potentialTarget in overlapped)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            
			if(dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }
		return bestTarget;
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if(!overlapped.Contains(other.gameObject))
			overlapped.Add(other.gameObject);
	}

	private void OnTriggerExit2D(Collider2D other) {
		if(overlapped.Contains(other.gameObject))
			overlapped.Remove(other.gameObject);
	}
}

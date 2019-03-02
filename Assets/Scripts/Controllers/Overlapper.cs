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

    public T GetFirstObject<T>() {
        for (int i = 0; i < overlapped.Count; i++) {
            if(overlapped[i]!=null)
                if (overlapped[i].GetComponent<T>() != null)
                    return overlapped[i].GetComponent<T>();
        }
        return default(T);
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

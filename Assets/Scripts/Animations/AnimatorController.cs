using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
	Up, Right, Down, Left
}

public class AnimatorController : MonoBehaviour
{
	public bool canShoot;
	
	public float movementSpeed {get; set;}
	public Direction facingDirection {get; set;}

	private void Update()
	{
		
	}
}

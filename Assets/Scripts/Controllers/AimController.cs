using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENUM_Input
{
	None, Mouse, Joystick, NB_ENTRIES
}

public class AimController : MonoBehaviour 
{
		//PROPERTIES
	private ENUM_Input aimInfluence;//If different of None, targetTrans is useless.
	[SerializeField] public Transform aimingTrans;
	[SerializeField] private Transform targetTrans;
	private Vector3 targetDirection;
	
	// Update is called once per frame
	void Update ()
	{
		Aiming();
	}

	private void Aiming()
	{
		float rotForce = 0f;

		switch(aimInfluence)
		{
			case ENUM_Input.Mouse:
			targetDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - aimingTrans.position).normalized;
			break;
			case ENUM_Input.Joystick:
			targetDirection = new Vector3(	Input.GetAxisRaw("Look_X"), Input.GetAxisRaw("Look_Y"), 0f);
			break;
			default:
			targetDirection = (targetTrans.position - aimingTrans.position).normalized;
			break;
		}

		rotForce = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
		aimingTrans.rotation = Quaternion.Euler(0f, 0f, rotForce);
	}

		//MUTATORS

	public void SetTarget(Transform target)
	{
		targetTrans = target;
	}

		//ACCESSORS
	public Vector3 GetTargetDirection()
	{
		return targetDirection;
	}

		//INITIALIZATIONS

	public void Init(Transform aimingTrans, ENUM_Input aimingInfluence)
	{
		this.aimingTrans = aimingTrans;
		aimInfluence = aimingInfluence;
	}

	public void Init(Transform aimingTrans, Transform target)
	{
		this.aimingTrans = aimingTrans;
		aimInfluence = ENUM_Input.None;
		targetTrans = target;
	}
}

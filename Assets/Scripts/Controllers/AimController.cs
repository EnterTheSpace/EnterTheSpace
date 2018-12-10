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
	private ENUM_Input m_aimInfluence;//If different of None, mRef_targetTrans is useless.
	[SerializeField] public Transform mRef_aimingTrans;
	[SerializeField] private Transform mRef_targetTrans;
	private Vector3 mV_targetDirection;
	
	// Update is called once per frame
	void Update ()
	{
		Aiming();
	}

	private void Aiming()
	{
		float rotForce = 0f;
		

		switch(m_aimInfluence)
		{
			case ENUM_Input.Mouse:
			mV_targetDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - mRef_aimingTrans.position).normalized;
			break;
			case ENUM_Input.Joystick:
			mV_targetDirection = new Vector3(	Input.GetAxisRaw("Look_X"),
												Input.GetAxisRaw("Look_Y"),
												0f);
			break;
			default:
			mV_targetDirection = (mRef_targetTrans.position - mRef_aimingTrans.position).normalized;
			break;
		}

		rotForce = Mathf.Atan2(mV_targetDirection.y, mV_targetDirection.x) * Mathf.Rad2Deg;
		mRef_aimingTrans.rotation = Quaternion.Euler(0f, 0f, rotForce);
	}

		//MUTATORS

	public void SetTarget(Transform target)
	{
		mRef_targetTrans = target;
	}

		//ACCESSORS
	public Vector3 GetTargetDirection()
	{
		return mV_targetDirection;
	}

		//INITIALIZATIONS

	public void Init(Transform aimingTrans, ENUM_Input aimingInfluence)
	{
		mRef_aimingTrans = aimingTrans;
		m_aimInfluence = aimingInfluence;
	}

	public void Init(Transform aimingTrans, Transform target)
	{
		mRef_aimingTrans = aimingTrans;
		m_aimInfluence = ENUM_Input.None;
		mRef_targetTrans = target;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DashController : MonoBehaviour
{
		//PROPERTIES
	[SerializeField] private float mFlt_speed;
	[SerializeField] private float mFlt_range;
	[SerializeField] private float mFlt_coolDown;
	private float mFlt_dashTime;
	private float mFlt_cdTime;
	private Vector3 mV_direction;
		//REFERENCES
	private Rigidbody2D mRef_rigidBody;

	private void Start()
	{
		Init();
	}

	private void FixedUpdate()
	{
		DashManager();
	}

	private void DashManager()
	{
		if(mFlt_dashTime > 0f)
		{
			mFlt_dashTime -= Time.deltaTime;
			mRef_rigidBody.MovePosition(mRef_rigidBody.position + (Vector2)mV_direction * mFlt_speed * Time.deltaTime);
		}
		if(mFlt_cdTime > 0f)
		{
			mFlt_cdTime -= Time.deltaTime;
		}
	}

	public bool TryDash(Vector3 direction)
	{
		if(CanDash())
		{
			mV_direction = direction.normalized;
			mFlt_cdTime = mFlt_coolDown;
			mFlt_dashTime = mFlt_range/mFlt_speed;
			return true;
		}else
		{
			Debug.Log("[Info] Cannot dash yet.");
			return false;
		}
	}
		//TESTS
	public bool CanDash()
	{
		return(mFlt_cdTime <= 0f);
	}

		//INITIALIZATIONS

	public void Init()
	{
		mFlt_cdTime = 0.0f;
		mRef_rigidBody = this.GetComponent<Rigidbody2D>();
	}

		//ACCESSORS
	public float GetDashSpeed()
	{
		return mFlt_speed;
	}

	public float GetDashRange()
	{
		return mFlt_range;
	}
}

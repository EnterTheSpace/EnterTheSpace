using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pawn : MonoBehaviour 
{
	public float mFlt_health {get; set;}
	[SerializeField] protected HealthBar mRef_healthBar;

	protected virtual void Initialization()
	{
		mFlt_health = 1f;
	}

	public virtual void ApplyDamages(float damages)
	{
		mFlt_health -= damages;
		if(mFlt_health>0)
			mFlt_health = Mathf.Clamp(mFlt_health, 0f, 1f);
		else
			Destroy(this.gameObject);
		mRef_healthBar.mFlt_health = mFlt_health;
	}
}

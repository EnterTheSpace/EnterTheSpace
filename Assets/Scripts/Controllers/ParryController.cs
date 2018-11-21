using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryController : MonoBehaviour 
{
	[SerializeField] private float mFlt_duration;
	[SerializeField] private float mFlt_cooldDown;
	[SerializeField] private GameObject mRef_parry;
	private float mFlt_cdTime;
	private float mFlt_playbackTime;

	// Update is called once per frame
	void Update ()
	{
		if(mFlt_cdTime > 0f)
		{
			mFlt_cdTime -= Time.deltaTime;
		}
		if(mFlt_playbackTime > 0f)
		{
			mFlt_playbackTime -= Time.deltaTime;
		}else
		{
			if(mRef_parry.activeInHierarchy)
				mRef_parry.SetActive(false);
		}
	}

	public bool TryParry()
	{
		if(mFlt_cdTime <= 0f)
		{
			mFlt_cdTime = mFlt_cooldDown;
			mRef_parry.SetActive(true);
			mRef_parry.GetComponent<Animator>().speed = 1/mFlt_duration;
			mFlt_playbackTime = mFlt_duration;
			return true;
		}else
		{
			Debug.Log("[Info] Cannot parry yet.");
			return false;
		}
	}
}

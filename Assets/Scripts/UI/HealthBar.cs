using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

	public float mFlt_health {set; get;}
	[SerializeField] public float mFlt_lerpSpeed;//Percent per second

	private bool mB_lerping;
	private Slider mRef_healthBar;

	private void Start()
	{
		mRef_healthBar = this.GetComponent<Slider>();
		mFlt_health = 1f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		mFlt_health = Mathf.Clamp(mFlt_health, 0f, 1f);
		
		if(mB_lerping)
		{
			if(mRef_healthBar.value > mFlt_health)
			{
				mRef_healthBar.value -= mFlt_lerpSpeed * Time.deltaTime;
			}else if(mRef_healthBar.value < mFlt_health)
			{
				mRef_healthBar.value += mFlt_lerpSpeed * Time.deltaTime;
			}else
				mB_lerping = false;
			mRef_healthBar.value = mRef_healthBar.value;
		}else
		{
			if(mFlt_health != mRef_healthBar.value)
				mB_lerping = true;
		}
	}
}

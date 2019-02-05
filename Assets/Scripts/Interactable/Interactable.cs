using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class Interactable : MonoBehaviour
{
	//Custom
	[SerializeField] protected bool manualUse;
	public bool Manual{get{return manualUse;} set{manualUse = value;} }
	[SerializeField, Hide("manualUse", true)] protected bool requireHold;
	public bool RequireHold{get{return requireHold;} set{requireHold = value;}}
	[SerializeField, Hide("manualUse", true)] private Transform hintRef;
	public Transform HintRef{ get{return hintRef;} set{hintRef = HintRef;} }
	[SerializeField] protected bool damageSensitive;
	public bool DamageSensitive{get{return damageSensitive;} set{damageSensitive = value;} }
	[SerializeField] protected bool switchable;
	public bool Switchable{get{return switchable;} set{switchable = value;}}
	[SerializeField] protected bool singleUse;
	public bool SingleUse{get{return singleUse;} set{singleUse = value;}}

	//Properties
	//State
	public bool beingUsed;
	public bool beingHighlighted;
	
	//References
	private Collider2D colliderRef;

	private void Start()
	{
		Initialization();
	}

	protected virtual void Initialization()
	{
		colliderRef = this.GetComponent<Collider2D>();
		if(manualUse && hintRef != null)
			Unhighlight();
	}

	public virtual void Interact()
	{
		if(!beingUsed)
		{
			beingUsed = true;

			/*
			if(!switchable)
				Release();
			*/
		}
		//TO DO : Add the hold functionality
	}

	public virtual void Release()
	{
		if(beingUsed)
		{
			beingUsed = false;
			if(singleUse)
			{
				Destroy(this);
			}
		}
	}

	public virtual void Highlight(bool hint, bool tObject)
	{
		if(hint)
		{
			if(manualUse && hintRef != null)
			{
				if(!beingHighlighted)
				{
					beingHighlighted = true;
					hintRef.gameObject.SetActive(true);
				}
			}
			else
				Debug.Log("[Interact : Highlight] Error, the interactable object has no hintRef or is not declared as manualUse");
		}
		if(tObject){
			//TO DO : highlight the object itself
		}
	}

	public virtual void Unhighlight()
	{
		if(beingHighlighted)
		{
			beingHighlighted = false;
			hintRef.gameObject.SetActive(false);
			//TO DO : Unhighlight the object itself
		}
	}
}

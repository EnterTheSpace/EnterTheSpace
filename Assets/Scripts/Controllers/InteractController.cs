using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Overlapper))]
public class InteractController : MonoBehaviour
{
	//Custom
	[SerializeField] private Cooldown cooldown;

	//References
	private Interactable inRangeObject;
	private Overlapper overlapper;

	private void Start() {
		Initialization();
	}

	private void Update() {
		Check();
	}

	public Interactable TryInteract() {
		print("Interact");
		if(inRangeObject != null){
			inRangeObject.Interact();

			return inRangeObject;
		}else{
			return null;
		}
	}

	private void Check(){
		Interactable temp = null;

		if(cooldown.Ready()){
			if((temp = overlapper.ClosestObject().GetComponent<Interactable>()) != null){
				inRangeObject = temp;
				inRangeObject.Highlight(true, true);
			}else{
				print("No interactable found");
				if(inRangeObject != null){
					print("Unhighlighting object");
					inRangeObject.Unhighlight();
					inRangeObject = null;
				}
			}
		}else{
			cooldown.Reset();
		}
	}

	private void Initialization(){
		overlapper = this.GetComponent<Overlapper>();
	}
}

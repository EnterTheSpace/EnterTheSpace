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
    public Interactable InRangeObject { get { return inRangeObject; } protected set{inRangeObject = value; } }
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

    public Interactable TryRelease() {
        print("Release");
        if (inRangeObject != null) {
            inRangeObject.Release();

            return inRangeObject;
        } else {
            return null;
        }
    }

    public bool TryNavigate(Vector2 joystick) {
        if(inRangeObject.GetComponent<Shop>() != null) {
            inRangeObject.GetComponent<Shop>().Navigate(joystick.y);

            return true;
        }
        return false;
    }

	private void Check(){
		Interactable temp = null;

		if(cooldown.Ready()){
			if((temp = overlapper.ClosestObject().GetComponent<Interactable>()) != null){
				inRangeObject = temp;
				inRangeObject.Highlight(true, true);
			}else{
				print("No interactable found");
				if(inRangeObject != null) {
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

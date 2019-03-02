using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

 #if UNITY_EDITOR
 using UnityEditor;
 #endif

[System.Serializable]
 public struct Permission
 {
	 public bool canMove;
	 public bool canDash;
	 public bool canInteract;
 }

[System.Serializable]
 public enum State
 {
	 None, Idle, Move, Dash, Parry, Interact, Dialogue, Die
 }

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(AimController))]
[RequireComponent(typeof(DashController))]
[RequireComponent(typeof(ParryController))]
[RequireComponent(typeof(InteractController))]
public class Player : Pawn
{
	//Properties
	[Header("Properties")]
	[Tooltip("Player speed"), SerializeField] private float speed;
	[SerializeField] private ENUM_Input input;
	[SerializeField] Dictionary<State, Permission> Abilities;

	private bool shootReset;

	//References
	[Header("References")]
	[Tooltip("Aiming transform reference"), SerializeField] private Transform arm;
	
	private WeaponController weaponRef;
	private MovementController mvmtController;
	private AimController aimController;
	private DashController dashController;
	private ParryController parryController;
	private InteractController interactController;

    public bool isInShop;

	void Start ()
	{
		Initialization();
	}

	protected override void Initialization()
	{
		base.Initialization();

		if(Input.GetJoystickNames().Length > 0)
			input = ENUM_Input.Joystick;
		else
			input = ENUM_Input.Mouse;

		shootReset = true;
        isInShop = false;

		weaponRef = this.GetComponent<WeaponController>();

		mvmtController = this.GetComponent<MovementController>();
		mvmtController.Init(Vector3.zero, speed, true);

		aimController = this.GetComponent<AimController>();
		aimController.Init(arm, input);

		dashController = this.GetComponent<DashController>();
		parryController = this.GetComponent<ParryController>();
		interactController = this.GetComponent<InteractController>();
	}

	public override void ApplyDamages(float damages)
	{
		base.ApplyDamages(damages);
		if(health == 0f)
		{
			//Destroy(this.gameObject);//Death here

			Scene currentScene = SceneManager.GetActiveScene();

			SceneManager.LoadScene(currentScene.name);
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
        Overlap();
		Inputs();//Handles player inputs
	}

    private void Overlap() {
        if(this.GetComponent<Overlapper>().GetFirstObject<Shop>() != null && this.GetComponent<Overlapper>().GetFirstObject<Shop>().BeingUsed) {
            isInShop = true;
            print("SHOP");
        } else {
            isInShop = false;
        }
    }

	private void Inputs()
	{
		Vector3 diff = aimController.GetTargetDirection();

        if (!isInShop) {
            //Sets the player body animator parameter to the greatest moving axis input (Mathf.Abs converts to the absolute value).
            this.GetComponent<Animator>().SetFloat("Speed", (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > Mathf.Abs(Input.GetAxisRaw("Vertical")) ? Mathf.Abs(Input.GetAxisRaw("Horizontal")) : Mathf.Abs(Input.GetAxisRaw("Vertical"))));

            switch (input) {
                case ENUM_Input.Joystick:
                    if (Input.GetAxis("Trigger") < -0.02f) {
                        weaponRef.TryShot(shootReset);
                        shootReset = false;
                    } else
                        shootReset = true;

                    if (Input.GetAxis("Trigger") > 0.02f)
                        parryController.TryParry();
                    break;
                case ENUM_Input.Mouse:
                    if (Input.GetButton("Fire1")) {
                        weaponRef.TryShot(shootReset);
                        shootReset = false;
                    } else {
                        shootReset = true;
                    }

                    if (Input.GetButtonDown("Parry"))
                        parryController.TryParry();
                    break;
                default:
                    break;
            }

            if (Input.GetButtonDown("Dash")) {
                if (dashController.TryDash(mvmtController.GetDirection())) {
                    this.GetComponent<Animator>().SetTrigger("Dash");
                    //this.GetComponent<Animator>().speed = 1/(dashController.GetDashRange()/dashController.GetDashSpeed());
                }
            }

            if (diff.x < 0f)//Player aiming left
            {
                if (!weaponRef.m_weaponSprite.GetComponent<SpriteRenderer>().flipY) {
                    weaponRef.m_weaponSprite.GetComponent<SpriteRenderer>().flipY = true;
                    weaponRef.BarrelRef().localPosition -= new Vector3(0f, 0.06f, 0f);
                }
                this.GetComponent<SpriteRenderer>().flipX = true;//Flip the character body sprite
                if (Input.GetAxis("Horizontal") > 0.0f)//If the player is moving in the opposite direction (to the right), the correct running animation is played.
                {
                    this.GetComponent<Animator>().SetBool("MoveForward", false);
                } else {
                    this.GetComponent<Animator>().SetBool("MoveForward", true);
                }
            } else if (diff.x > 0f)//Player aiming right
             {
                if (weaponRef.m_weaponSprite.GetComponent<SpriteRenderer>().flipY) {
                    weaponRef.m_weaponSprite.GetComponent<SpriteRenderer>().flipY = false;
                    weaponRef.BarrelRef().localPosition += new Vector3(0f, 0.06f, 0f);

                }
                this.GetComponent<SpriteRenderer>().flipX = false;//Restore the character body and the gun sprites orientation
                if (Input.GetAxis("Horizontal") < 0.0f)//If the player is moving in the opposite direction (to the left), the correct running animation is played.
                {
                    this.GetComponent<Animator>().SetBool("MoveForward", false);
                } else {
                    this.GetComponent<Animator>().SetBool("MoveForward", true);
                }
            }
        } else {
            interactController.TryNavigate(new Vector2(0f, Input.GetKeyDown(KeyCode.S) ? 1f : ((Input.GetKeyDown(KeyCode.Z) ? -1f : 0f))));

            if (Input.GetKeyDown(KeyCode.Escape))
                interactController.TryRelease();
        }

		if(Input.GetButtonDown("Interact"))
		{
			interactController.TryInteract();
		}
	}

	private void ChangeWeapon(WeaponController newWeapon)
	{
		weaponRef = newWeapon;
	}
}

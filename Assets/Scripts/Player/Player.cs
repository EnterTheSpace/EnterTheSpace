using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

 #if UNITY_EDITOR
 using UnityEditor;
 #endif

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(AimController))]
[RequireComponent(typeof(DashController))]
[RequireComponent(typeof(ParryController))]
public class Player : Pawn
{
	//Properties
	[Header("Properties")]
	[Tooltip("Player speed")][SerializeField] private float mFlt_speed;
	[SerializeField]private ENUM_Input m_controller;
	//References
	[Header("References")]
	[Tooltip("Aiming transform reference"), SerializeField] private Transform mRef_aiming;
	[Tooltip("Gun transform reference"), SerializeField] private Transform m_gunRef;
	[Tooltip("Weapon script reference"), SerializeField] private Weapon m_weaponRef;

	private MovementController mRef_mvmtController;
	private AimController mRef_aimController;
	private DashController mRef_dashController;
	private ParryController mRef_parryController;


	// Use this for initialization
	void Start ()
	{
		//Cursor.lockState = CursorLockMode.Confined;
		Initialization();
	}

	protected override void Initialization()
	{
		base.Initialization();
		if(m_controller != ENUM_Input.None)
		{
			if(Input.GetJoystickNames().Length > 0)
				m_controller = ENUM_Input.Joystick;
			else
				m_controller = ENUM_Input.Mouse;
		}

		mRef_mvmtController = this.GetComponent<MovementController>();
		mRef_mvmtController.Init(Vector3.zero, mFlt_speed, true);

		mRef_aimController = this.GetComponent<AimController>();
		mRef_aimController.Init(mRef_aiming, m_controller);

		mRef_dashController = this.GetComponent<DashController>();
		mRef_parryController = this.GetComponent<ParryController>();
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
		Inputs();//Handles player inputs
	}

	private void Inputs()
	{
		Vector3 diff = mRef_aimController.GetTargetDirection().normalized;

		//Sets the player body animator parameter to the greatest moving axis input (Mathf.Abs converts to the absolute value).
		this.GetComponent<Animator>().SetFloat("Speed", (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > Mathf.Abs(Input.GetAxisRaw("Vertical")) ? Mathf.Abs(Input.GetAxisRaw("Horizontal")) : Mathf.Abs(Input.GetAxisRaw("Vertical"))));

		switch(m_controller)
		{
			case ENUM_Input.Joystick:
				if(Input.GetAxis("Trigger") < -0.02f)
					m_weaponRef.TryShot(diff);
				if(Input.GetAxis("Trigger") > 0.02f)
					mRef_parryController.TryParry();
			break;
			case ENUM_Input.Mouse:
				if(Input.GetButtonDown("Fire1"))
					m_weaponRef.TryShot(diff);
				if(Input.GetButtonDown("Parry"))
					mRef_parryController.TryParry();
			break;
			default:
			break;
		}

		if(Input.GetButtonDown("Dash"))
		{
			if(mRef_dashController.TryDash(mRef_mvmtController.GetDirection()))
			{
				this.GetComponent<Animator>().SetTrigger("Dash");
				//this.GetComponent<Animator>().speed = 1/(mRef_dashController.GetDashRange()/mRef_dashController.GetDashSpeed());
			}
		}

		if(diff.x < 0f)//Player aiming left
		{
			if(!m_gunRef.GetChild(0).GetComponent<SpriteRenderer>().flipY)
			{
				m_gunRef.GetChild(0).GetComponent<SpriteRenderer>().flipY = true;
				m_gunRef.GetChild(0).GetComponent<Weapon>().BarrelRef().localPosition -= new Vector3(0f, 0.06f, 0f);
			}
			this.GetComponent<SpriteRenderer>().flipX = true;//Flip the character body sprite
			if(Input.GetAxis("Horizontal") > 0.0f)//If the player is moving in the opposite direction (to the right), the correct running animation is played.
			{
				this.GetComponent<Animator>().SetBool("MoveForward", false);
			}else
			{
				this.GetComponent<Animator>().SetBool("MoveForward", true);
			}
		}else if(diff.x > 0f)//Player aiming right
		{
			if(m_gunRef.GetChild(0).GetComponent<SpriteRenderer>().flipY)
			{
				m_gunRef.GetChild(0).GetComponent<SpriteRenderer>().flipY = false;
				m_gunRef.GetChild(0).GetComponent<Weapon>().BarrelRef().localPosition += new Vector3(0f, 0.06f, 0f);
			
			}
			this.GetComponent<SpriteRenderer>().flipX = false;//Restore the character body and the gun sprites orientation
			if(Input.GetAxis("Horizontal") < 0.0f)//If the player is moving in the opposite direction (to the left), the correct running animation is played.
			{
				this.GetComponent<Animator>().SetBool("MoveForward", false);
			}else
			{
				this.GetComponent<Animator>().SetBool("MoveForward", true);
			}			
		}
	}

	private void ChangeWeapon(Weapon newWeapon)
	{
		m_weaponRef = newWeapon;
	}
}

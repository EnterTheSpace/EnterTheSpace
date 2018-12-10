using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : MonoBehaviour
{
	//Properties
	[Header("Properties")]
	
	[Tooltip("Player speed")][SerializeField] private float mFlt_speed;
	//References
	[Header("References")]
	[Tooltip("Gun transform reference")][SerializeField] private Transform m_gunRef;
	private WeaponController m_weaponRef;
	private Transform m_bodyRef;
	
	private Vector3 m_mousePos;

	// Use this for initialization
	void Start ()
	{
		//Cursor.lockState = CursorLockMode.Confined;
		Initialization();
	}
	
	// Update is called once per frame
	void Update ()
	{
		Inputs();//Handles player inputs
	}

	void FixedUpdate(){
		//Moves the player considering his inputs and speed.
		this.transform.GetComponent<Rigidbody2D>().velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))* mFlt_speed * Time.deltaTime;
	}

	private void Inputs()
	{
		Vector3 diff = Vector3.zero;
		float rot_z = 0f;

		//Sets the player body animator parameter to the greatest moving axis input (Mathf.Abs converts to the absolute value).
		m_bodyRef.GetComponent<Animator>().SetFloat("Speed", (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > Mathf.Abs(Input.GetAxisRaw("Vertical")) ? Mathf.Abs(Input.GetAxisRaw("Horizontal")) : Mathf.Abs(Input.GetAxisRaw("Vertical"))));
		
		//Defines the mouse cursor position considering the gun's position.
		m_mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - m_gunRef.position;
		diff = m_mousePos.normalized;

		//Defines the rotation to operate between the mouse cursor position and the gun's position.
		rot_z = Mathf.Atan2(m_mousePos.y, m_mousePos.x) * Mathf.Rad2Deg;
		m_gunRef.rotation = Quaternion.Euler(0f, 0f, rot_z);

		/*
		Debug.DrawLine(Camera.main.ScreenToWorldPoint(Input.mousePosition), this.transform.position, Color.red);
		Debug.DrawLine(this.transform.position, new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, this.transform.position.y, this.transform.position.z), Color.blue);
		Debug.DrawLine(new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, this.transform.position.y, this.transform.position.z), Camera.main.ScreenToWorldPoint(Input.mousePosition), Color.green);
		*/

		if(diff.x < 0f)//Player aiming left
		{
			//m_bodyRef.localScale = new Vector3(-1.0f, 1.0f, 1.0f);//Flip the character body and the gun sprites
			m_bodyRef.GetComponent<SpriteRenderer>().flipX = true;

			m_gunRef.GetChild(0).GetComponent<SpriteRenderer>().flipY = true;

			if(Input.GetAxis("Horizontal") > 0.0f)//If the player is moving in the opposite direction (to the right), the correct running animation is played.
			{
				m_bodyRef.GetComponent<Animator>().SetBool("MoveForward", false);
			}else
			{
				m_bodyRef.GetComponent<Animator>().SetBool("MoveForward", true);
			}
		}else if(diff.x > 0f)//Player aiming right
		{
			//m_bodyRef.localScale = new Vector3(1.0f, 1.0f, 1.0f);//Restore the character body and the gun sprites orientation
			m_bodyRef.GetComponent<SpriteRenderer>().flipX = false;
			m_gunRef.GetChild(0).GetComponent<SpriteRenderer>().flipY = false;
			if(Input.GetAxis("Horizontal") < 0.0f)//If the player is moving in the opposite direction (to the left), the correct running animation is played.
			{
				m_bodyRef.GetComponent<Animator>().SetBool("MoveForward", false);
			}else
			{
				m_bodyRef.GetComponent<Animator>().SetBool("MoveForward", true);
			}			
		}

		if(Input.GetButtonDown("Fire1"))
		{
			m_weaponRef.TryShot(false);
			//Debug.Log(m_gunRef.rotation.eulerAngles);
		}
	}

	private void ChangeWeapon(WeaponController newWeapon)
	{
		m_weaponRef = newWeapon;
	}

	private void Initialization()
	{
		m_bodyRef = this.transform.GetChild(0);
		m_mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - m_gunRef.position;
		m_weaponRef = m_gunRef.GetChild(0).GetComponent<WeaponController>();
	}

}

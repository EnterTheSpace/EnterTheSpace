using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovementController : MonoBehaviour
{
		//CONSTS
	public const float SPEED_MULTIPLIER = 100;
		//PROPERTIES
	private Vector2 mV_direction;
	private float mFlt_MovementSpeed;
	private bool mB_rgdBodyBased;
	private bool mB_inputBased;
		//REFERENCES
	private Rigidbody2D mRef_rigidBody;
    public AudioClip[] sounds;

	// Update is called once per frame
	void FixedUpdate ()
	{
		MovementForces();
		Movement();
	}

	private void MovementForces()
	{
		if(mB_inputBased)
		{
			//Moves the object considering inputs and its speed.
            if(this.GetComponent<Player>() != null) {
                if(!this.GetComponent<Player>().isInShop)
                    mV_direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            } else
                mV_direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

		}
	}

	private void Movement()
	{
		mRef_rigidBody.velocity = mV_direction * mFlt_MovementSpeed * SPEED_MULTIPLIER * Time.deltaTime;
	}

    public void WalkStep() {
        this.GetComponent<AudioSource>().PlayOneShot(sounds[Random.Range(0, sounds.Length - 1)]);
    }

	public void SetDirection(Vector3 direction)
	{
		mV_direction = direction.normalized;
	}

	public void Stop()
	{
		mV_direction = new Vector3(0f, 0f, 0f);
	}

	public Vector3 GetDirection()
	{
		return mV_direction;
	}

		//INITIALIZATION

	public void Init(Vector3 direction, float speed, bool inputBased)
	{
		//Properties
		mV_direction = direction;
		mFlt_MovementSpeed = speed;
		mB_inputBased = inputBased;
		//References
		mRef_rigidBody = this.GetComponent<Rigidbody2D>();
	}
}

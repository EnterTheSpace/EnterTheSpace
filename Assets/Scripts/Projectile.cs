using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct sProjectileInfos
{
	public float speed;
	public float damages;
	public Vector3 direction;
	public float destroyer;
	public bool bounce;
}

public class Projectile : MonoBehaviour
{
	public sProjectileInfos m_infos;
	
	//TO DO : Implement this function when a projectile is instantiated, the fired projectiles of the Weapon class should be of type Projectile
	public Projectile Constructor(Vector2 direction, float speed, float damages)
	{
		m_infos.direction = direction;
		m_infos.speed = speed;
		m_infos.damages = damages;

		return this;
	}

	public void Destructor()
	{
		Destroy(this.gameObject);
	}

	//TO DO : Replace this function by the Constructor function
	public void Initialize(Vector2 direction, float speed, float damages)
	{
		m_infos.direction = direction;
		m_infos.speed = speed;
		m_infos.damages = damages;
		if(m_infos.bounce)
			this.GetComponent<Collider2D>().isTrigger = false;
		else
			this.GetComponent<Collider2D>().isTrigger = true;
	}

	public void Initialize(Vector3 direction)
	{
		m_infos.direction = direction;
		this.GetComponent<Rigidbody2D>().velocity = direction.normalized * m_infos.speed;
		Invoke("Destructor", m_infos.destroyer);
		if(m_infos.bounce)
			this.GetComponent<Collider2D>().isTrigger = false;
		else
			this.GetComponent<Collider2D>().isTrigger = true;
	}


	
	private void OnTriggerEnter2D(Collider2D other)
	{
		if(!other.CompareTag("Grid")){
			if(other.gameObject.GetComponent<Pawn>() != null && !other.CompareTag("Parry"))
			{
				other.gameObject.GetComponent<Pawn>().ApplyDamages(m_infos.damages);
				Destructor();
			}else if(other.gameObject.GetComponent<Projectile>() == null)
			{
				Destructor();
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if(other.gameObject.GetComponent<Pawn>() != null)
		{
			other.gameObject.GetComponent<Pawn>().ApplyDamages(m_infos.damages);
			Destructor();
		}else if(other.gameObject.GetComponent<Projectile>() == null && !m_infos.bounce)
		{
			Destructor();
		}
	}
}

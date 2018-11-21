using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

	[Header("Properties")]
	[Tooltip("Weapon's fire rate, shots per second")][SerializeField] private float mFlt_fireRate;
	[SerializeField] private uint mN_projPerShot;
	[SerializeField] private float mFlt_spawnRadius;
	[SerializeField] private float mFlt_dispersion;
	private float mFlt_coolDown;

	[Header("References")]
	[Tooltip("Weapon's projectile prefab reference")][SerializeField] private GameObject m_bulletPrefab;//TO DO : Convert to "Projectile" object reference instead
	[Tooltip("Weapon's barrel transform reference")][SerializeField] private Transform m_barrelRef;
	[Tooltip("Owner gameobject reference")][SerializeField] private GameObject m_ownerRef;
	private List<Projectile> mL_firedBullets;
	public sProjectileInfos m_projectileInfos;

	// Use this for initialization
	private void Start ()
	{
		Initialization();
	}
	
	// Update is called once per frame
	protected virtual void Update ()
	{
		if(mFlt_coolDown > 0f)
		{
			mFlt_coolDown -= Time.deltaTime;
		}
	}

	public void TryShot(Vector3 direction)
	{
		if(mFlt_coolDown <= 0.0f)
		{
			mFlt_coolDown = 1/mFlt_fireRate;
			Shot(direction);
		}else
		{
			Debug.Log("[Info] Cannot shoot, weapon still in cooldown.");
		}
	}

	private void Shot(Vector3 direction)
	{
		uint nbProj = (mN_projPerShot > 0) ? mN_projPerShot : 1;
		Vector3 position = Vector3.zero;
		GameObject newProjectile;

		for(int i = 0 ; i < nbProj ; i ++)
		{
			position = Random.insideUnitCircle * mFlt_spawnRadius;
			newProjectile = Instantiate(m_bulletPrefab, m_barrelRef.position + position, m_barrelRef.rotation, null);
			mL_firedBullets.Add(newProjectile.GetComponent<Projectile>());
			mL_firedBullets[mL_firedBullets.Count-1].Initialize(	new Vector3(
																				direction.x + Random.Range(0, mFlt_dispersion),
																				direction.y + Random.Range(0, mFlt_dispersion),
																				0f)
																				);
		}
	}

		//INITIALIZATION

	private void Initialization()
	{
		mL_firedBullets = new List<Projectile>();
		mFlt_coolDown = 0.0f;
	}

		//ACCESSORS

	public Transform BarrelRef()
	{
		return m_barrelRef;
	}
}

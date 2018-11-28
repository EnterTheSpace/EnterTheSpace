using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

	[Header("Properties")]
    [Tooltip("Weapon's fire rate, shots per second"), SerializeField] private float fireRate;
    [SerializeField] private uint projPerShot;
    [SerializeField] private float spawnRadius;
    [SerializeField] private float dispersion = 0;
    private float coolDown;

    [Header("References")]
    [Tooltip("Weapon's projectile prefab reference"), SerializeField] private GameObject m_bulletPrefab;//TO DO : Convert to "Projectile" object reference instead
    [Tooltip("Weapon's barrel transform reference"), SerializeField] private Transform m_barrelRef;
    [Tooltip("Owner gameobject reference"), SerializeField] private GameObject m_ownerRef;
    private List<Projectile> mL_firedBullets;
    public ProjectileInfos m_projectileInfos;

	// Use this for initialization
    private void Start ()
    {
        Initialization();
    }

    // Update is called once per frame
    protected virtual void Update ()
    {
        if(coolDown > 0f)
        {
            coolDown -= Time.deltaTime;
        }
    }

    public void TryShot(Vector3 direction)
    {
        if(coolDown <= 0.0f)
        {
            coolDown = 1/fireRate;
            Shot(direction);
        }else
        {
            Debug.Log("[Info] Cannot shoot, weapon still in cooldown.");
        }
    }

    private void Shot(Vector3 direction)
    {
        uint nbProj = (projPerShot > 0) ? projPerShot : 1;
        Vector3 position = Vector3.zero;
        GameObject newProjectile;

        for(int i = 0 ; i < nbProj ; i ++)
        {
            position = Random.insideUnitCircle * spawnRadius;
            newProjectile = Instantiate(m_bulletPrefab, m_barrelRef.position + position, m_barrelRef.rotation, null);
            mL_firedBullets.Add(newProjectile.GetComponent<Projectile>());
            mL_firedBullets[mL_firedBullets.Count-1].Initialization(m_projectileInfos, new Vector3(
                                                                                    direction.x + Random.Range(0, dispersion),
                                                                                    direction.y + Random.Range(0, dispersion),
                                                                                    0f)
                                                                    );
        }
    }

        //INITIALIZATION

    private void Initialization()
    {
        mL_firedBullets = new List<Projectile>();
        coolDown = 0.0f;
    }

        //ACCESSORS

    public Transform BarrelRef()
    {
        return m_barrelRef;
    }
}

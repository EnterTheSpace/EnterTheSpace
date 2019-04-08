using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour {

	[Header("Properties")]
    [Tooltip("Wether the weapon is single shot or full auto"), SerializeField] private bool singleShot;
    [Tooltip("Shot(s) per second"), SerializeField, Range(0f, 20f)] public float fireRate;
    [SerializeField] public uint projPerShot;
    [SerializeField] private float spawnRadius;
    [SerializeField, Range(0f, 360f)] public float dispersion = 0;
    [SerializeField] public float distance;
    private float coolDown;

    [Header("References")]
    [Tooltip("Weapon's projectile prefab reference"), SerializeField] private GameObject m_bulletPrefab;//TO DO : Convert to "Projectile" object reference instead
    [Tooltip("Weapon's sprite reference"), SerializeField] public GameObject m_weaponSprite;
    [Tooltip("Weapon's barrel transform reference"), SerializeField] private Transform m_barrelRef;
    [Tooltip("Owner gameobject reference"), SerializeField] private GameObject m_ownerRef;
    private List<Projectile> mL_firedBullets;
    public ProjectileInfos m_projectileInfos;
    public GameObject[] blastParticles;
    public AudioClip[] sounds;

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

    //TO DO : Create an overloaded function of TryShot() with Vector3 direction parameter
    public bool TryShot(bool newShot)//NOTE : This param is mostly for the player
    {
        if(singleShot)//If the weapon is in singleshot mode and the shooter hasn't released the trigger between two shots the weapon doesn't shoot
        {
            if(!newShot)
                return false;
        }

        if(coolDown <= 0.0f){
            Shot();

            return true;
        }else{
            Debug.Log("[Info] Cannot shoot, weapon still in cooldown.");

            return false;
        }
    }

    //TO DO : Create an overloaded function of Shot() with Vector3 direction parameter
    private void Shot()
    {
        if(this.GetComponent<AudioSource>() != null)
            this.GetComponent<AudioSource>().PlayOneShot(sounds[Random.Range(0, sounds.Length - 1)]);

        if (blastParticles != null) {
            GameObject temp = Instantiate(blastParticles[Random.Range(0, blastParticles.Length - 1)], m_barrelRef);

            temp.transform.localPosition += new Vector3(.1f, 0f);
            temp.transform.localScale = new Vector3(3f, 3f);
            Destroy(temp, 1f);
        }
            //audioPlayer.TryPlayInRndSource(new List<AudioSource>(GetComponents<AudioSource>()), sounds[0]);
        coolDown = 1/fireRate;
        
        uint nbProj = (projPerShot > 0) ? projPerShot : 1;
        float angleStep = (nbProj % 2 == 0) ? (dispersion / nbProj) : (dispersion / (nbProj - 1));

		float shotRayAngleA = (-dispersion)/2 - m_barrelRef.eulerAngles.z + 90f;
        float shotRayAngleB = (dispersion)/2 - m_barrelRef.eulerAngles.z + 90f;

        Vector3 position = Vector3.zero;
        Vector3 target = Vector3.zero;
        GameObject newProjectile;

        for(int i = 0 ; i < nbProj ; i ++)
        {
            position = Random.insideUnitCircle * spawnRadius;

            if(i % 2 == 0)
            {
                target = new Vector3(Mathf.Sin(shotRayAngleB * Mathf.Deg2Rad), Mathf.Cos(shotRayAngleB * Mathf.Deg2Rad), 0f);
                shotRayAngleB -= angleStep;
            }else
            {   
                target = new Vector3(Mathf.Sin(shotRayAngleA * Mathf.Deg2Rad), Mathf.Cos(shotRayAngleA * Mathf.Deg2Rad), 0f);
                shotRayAngleA += angleStep;
            }
            newProjectile = Instantiate(m_bulletPrefab, m_barrelRef.position + position, m_barrelRef.rotation, null);
            mL_firedBullets.Add(newProjectile.GetComponent<Projectile>());
            
            mL_firedBullets[mL_firedBullets.Count-1].Initialization(m_projectileInfos, target, this.GetComponent<Pawn>());
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

    public Vector3 DirFromAngle(float angleInDegrees, bool isGlobal)
    {
        if(!isGlobal){
            angleInDegrees -= m_weaponSprite.transform.eulerAngles.z;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), 0f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using PolyNav;

[RequireComponent(typeof(FieldOfView),typeof(Following),typeof(Escaping))]
[RequireComponent(typeof(Attacking))]
public class EnnemyManager : Pawn {

	//SerializeFields
	[Header("Object"),LabelOverride("Player reference"), SerializeField]
	[Tooltip("Object to follow.")]
	private GameObject player;
	[Header("Collision"),SerializeField, LabelOverride("Have collision?")]
	[Tooltip("Have collision with player.")]
	private bool hasCollision = true;
	[Header("Path"),LabelOverride("Update delay"),SerializeField]
	[Tooltip("Delay in second for updating the path.")]
	private float updatePathDelay = 1f;
    [Header("Drop"), LabelOverride("Item dropped"), SerializeField]
    [Tooltip("Item dropped when the ennemy is dead.")]
    private GameObject dropPrefab;
    [Header("Weapon"), LabelOverride("Has Weapon?"), SerializeField]
    private bool hasWeapon;
    public AudioClip[] atkSounds;

    //Privates
    private PolyNavAgent nav;
	private float currentTime;
	private FieldOfView fow;
	private Escaping escape;
	private Following follow;
	private Attacking attack;
	private Transform visibleTargets;
	private Transform lastVisibleTarget;
    private float escapeDistance;
    private bool isAttacking = false;

	// Use this for initialization
	void Start () {
        escapeDistance = 0;
		nav = GetComponent<PolyNavAgent>();
		if(!hasCollision)
			nav.GetComponent<CircleCollider2D>().isTrigger = true;
		fow = GetComponent<FieldOfView>();
		escape = GetComponent<Escaping>();
		follow = GetComponent<Following>();
		attack = GetComponent<Attacking>();
		visibleTargets = null;
        Initialization();
	}
	
	// Update is called once per frame
	void Update () {
        if(nav.velocity.x != 0 || nav.velocity.y != 0f)
            GetComponent<Animator>().SetBool("isMoving", true);
        else
            GetComponent<Animator>().SetBool("isMoving", false);
        

        if (follow != null && !isAttacking)
        { 
		    currentTime += Time.deltaTime;

            if (!escape.isEscaping)
                escapeDistance = follow.minDistance;
            else
                escapeDistance = escape.escapeDistance;
		
            if(player != null) {
                //Si l'ennemie est à la bonne distance
                if (Vector2.Distance(transform.position, player.transform.position) >= follow.minDistance + escapeDistance) {
                    //Si l'ennemis peut bouger & que le player est dans la zone
                    if (currentTime > updatePathDelay) {
                        currentTime = follow.Follow(player, nav, currentTime);
                    }
                } else {    //S'il n'ai pas à la bonne distance
                    if (follow.isFollowingPlayer) {   // et que l'ennemis à un chemin à suivre
                        follow.isFollowingPlayer = false;
                        nav.Stop();
                        print("Path cleared");
                    }
                }
                if (follow.isOverlapping) {
                    GetComponent<AimController>().enabled = true;
                    GetComponent<AimController>().SetTarget(player.transform);
                } else {
                    GetComponent<AimController>().SetTarget(lastVisibleTarget);
                    GetComponent<AimController>().enabled = false;
                }
            }

        }
        SpriteOrientation();
    }

    void SpriteOrientation() {
        AimController aim = GetComponent<AimController>();
        WeaponController weap = GetComponent<WeaponController>();
        Vector3 diff = GetComponent<AimController>().GetTargetDirection();

        if (diff.x < 0f)// aiming left
        {
            this.GetComponent<SpriteRenderer>().flipX = false;//Flip the character body sprite
            if (aim.aimingTrans.localScale.x == -1f)
                aim.aimingTrans.localScale = new Vector3(1f, 1f, 1f);
            
            if (weap.m_weaponSprite.GetComponent<SpriteRenderer>().flipX) {
                weap.m_weaponSprite.GetComponent<SpriteRenderer>().flipX = false;
                weap.m_weaponSprite.GetComponent<SpriteRenderer>().flipY = true;
                weap.BarrelRef().transform.localPosition = new Vector3(weap.BarrelRef().transform.localPosition.x * (-1f), -0.01f);
            }
            if (nav.velocity.x != 0f || nav.velocity.y != 0f) {//If walking
                aim.aimingTrans.localPosition = new Vector3(-0.217f, 0.11f, 0f);
            } else {
                aim.aimingTrans.localPosition = new Vector3(0.068f, 0.102f, 0f);
            }
        }
        else if (diff.x > 0f)// aiming right
        {
            this.GetComponent<SpriteRenderer>().flipX = true;
            if (aim.aimingTrans.localScale.x == 1f)
                aim.aimingTrans.localScale = new Vector3(-1f, 1f, 1f);
            
            if (!weap.m_weaponSprite.GetComponent<SpriteRenderer>().flipX) {
                weap.m_weaponSprite.GetComponent<SpriteRenderer>().flipX = true;
                weap.m_weaponSprite.GetComponent<SpriteRenderer>().flipY = false;
                weap.BarrelRef().transform.localPosition = new Vector3(weap.BarrelRef().transform.localPosition.x * (-1f), -0.01f);
            }
            if (nav.velocity.x != 0f || nav.velocity.y != 0f) {//If walking
                aim.aimingTrans.localPosition = new Vector3(0.22f, 0.109f, 0f);
            } else {
                aim.aimingTrans.localPosition = new Vector3(-0.067f, 0.099f, 0f);
            }
        }
    }

	void FixedUpdate(){

        if (escape.isEscaping){
            escape.Escape(player, nav);

            if(follow != null) {
                if (follow.isOverlapping) {
                    visibleTargets = fow.FindVisibleTargets();
                    if (visibleTargets != null) {
                        if (hasWeapon) {
                            GetComponent<WeaponController>().TryShot(true);
                        } else if(!isAttacking){
                            GetComponent<Animator>().SetBool("isAttacking", true);
                            isAttacking = true;
                            StartCoroutine(WaitUnitAttack());
                        }
                        lastVisibleTarget = visibleTargets;
                    } else {
                        GetComponent<Animator>().SetBool("isAttacking", false);
                        isAttacking = false;
                        if (lastVisibleTarget != null) {
                            nav.SetDestination(lastVisibleTarget.position);
                        }
                    }
                }
            }
        }
    }

    //Triggered from animation event
    public void AnimAttack()
    {
        Transform Target = fow.FindVisibleTargets();
        if (Target != null) {
            player.GetComponent<Player>().ApplyDamages(GetComponent<WeaponController>().m_projectileInfos.damages);
        }
    }

    //Triggered from animation event
    public void SoundAttack() {
        GetComponent<AudioSource>().PlayOneShot(atkSounds[Random.Range(0, atkSounds.Length - 1)]);
    }

    IEnumerator WaitUnitAttack()
    {
        yield return new WaitForSeconds(1);
        nav.Stop();
    }

    public override void ApplyDamages(float damages)
    {
        health -= damages;

        health = Mathf.Clamp(health, 0f, maxHealth);

        healthBar.health = health / maxHealth;

        if(!GetComponent<AudioSource>().isPlaying)
            this.GetComponent<AudioSource>().PlayOneShot(hitSounds[Random.Range(0, hitSounds.Length - 1)]);
        if (GetComponent<Animation>())
            GetComponent<Animation>().Play("Hit");
        if (health <= 0f){
            follow = null;
            nav.Stop();
            GetComponent<Animator>().SetTrigger("isDead");
            healthBar.gameObject.SetActive(false);
            if (GetComponent<AimController>() != null)
                GetComponent<AimController>().aimingTrans.gameObject.SetActive(false);
            GetComponent<Collider2D>().enabled = false;
            GetComponent<AudioSource>().PlayOneShot(deathSounds[Random.Range(0, deathSounds.Length - 1)]);
            if (dropPrefab != null)
                Instantiate(dropPrefab, transform.position, Quaternion.identity, null);
            Invoke("DestroyPawn", 1f);
        }
    }

    public override void DestroyPawn() {
        base.DestroyPawn();
    }
}

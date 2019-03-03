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
    [Header("Weapon"), LabelOverride("Has Weapon?"), SerializeField]
    private bool hasWeapon;


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
        if(nav.velocity.x != 0)
            GetComponent<Animator>().SetBool("isMoving", true);
        else
            GetComponent<Animator>().SetBool("isMoving", false);

        if (follow != null){ 
		    currentTime += Time.deltaTime;

            if (!escape.isEscaping)
                escapeDistance = follow.minDistance;
            else
                escapeDistance = escape.escapeDistance;
		
		    //Si l'ennemie est à la bonne distance
		    if(Vector2.Distance(transform.position,player.transform.position)>=follow.minDistance+escapeDistance){	
			    //Si l'ennemis peut bouger & que le player est dans la zone
			    if(currentTime > updatePathDelay){
				    currentTime = follow.Follow(player,nav,currentTime);
                }
            }
		    else{	//S'il n'ai pas à la bonne distance
			    if(follow.isFollowingPlayer){   // et que l'ennemis à un chemin à suivre
                    follow.isFollowingPlayer = false;
                    nav.Stop();
                    print("Path cleared");
			    }
		    }

            if (follow.isOverlapping)
            {
                GetComponent<AimController>().enabled = true;
                GetComponent<AimController>().SetTarget(player.transform);
            }
            else
            {
                GetComponent<AimController>().SetTarget(lastVisibleTarget);
                GetComponent<AimController>().enabled = false;
            }
        }
        if (hasWeapon)
            SpriteOrientation();
    }

    void SpriteOrientation()
    {
        Vector3 diff = GetComponent<AimController>().GetTargetDirection();
        if (diff.x < 0f)//Player aiming left
        {
            if (!GetComponent<WeaponController>().m_weaponSprite.GetComponent<SpriteRenderer>().flipY)
            {
                GetComponent<WeaponController>().m_weaponSprite.GetComponent<SpriteRenderer>().flipY = true;
                GetComponent<WeaponController>().BarrelRef().localPosition -= new Vector3(0f, 0.06f, 0f);
            }
            this.GetComponent<SpriteRenderer>().flipX = false;//Flip the character body sprite
        }
        else if (diff.x > 0f)//Player aiming right
        {
            if (GetComponent<WeaponController>().m_weaponSprite.GetComponent<SpriteRenderer>().flipY)
            {
                GetComponent<WeaponController>().m_weaponSprite.GetComponent<SpriteRenderer>().flipY = false;
                GetComponent<WeaponController>().BarrelRef().localPosition += new Vector3(0f, 0.06f, 0f);

            }
            this.GetComponent<SpriteRenderer>().flipX = true;//Restore the character body and the gun sprites orientation
        }
    }

	void FixedUpdate(){

        if (escape.isEscaping){
            escape.Escape(player, nav);

            if (follow.isOverlapping)
            {
                visibleTargets = fow.FindVisibleTargets();
                if (visibleTargets != null)
                {
                    if (hasWeapon)
                    {
                        GetComponent<WeaponController>().TryShot(true);
                    } 
                    else
                    {

                    }
                    lastVisibleTarget = visibleTargets;
                }
                else
                {
                    if (lastVisibleTarget != null)
                    {
                        nav.SetDestination(lastVisibleTarget.position);
                    }
                }
            }
        }
	}

    public override void ApplyDamages(float damages)
    {
        base.ApplyDamages(damages);
        if (health <= 0f){
            follow = null;
            nav.Stop();
            gameObject.GetComponent<Animator>().SetTrigger("isDead");
        }
    }
}

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
        if(follow != null){ 
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
			    //attack.Attack(player.transform.position,follow.minDistance);
		    }
		    else{	//S'il n'ai pas à la bonne distance
			    if(follow.isFollowingPlayer){	// et que l'ennemis à un chemin à suivre
				    nav.Stop();
				    follow.isFollowingPlayer = false;
				    print("Path cleared");
			    }
		    }
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

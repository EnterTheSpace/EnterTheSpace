//Add IgnoreCollider in layer to ignore the collision with ennemy
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using PolyNav;

public class EnnemyManager : Pawn {

	//SerializeFields
	[Header("Object"),LabelOverride("Player reference"), SerializeField]
	[Tooltip("Object to follow.")]
	private GameObject player;
	[SerializeField]
	private bool hasCollision = true;
	[Header("Path"),LabelOverride("Update delay"),SerializeField]
	[Tooltip("Delay in second for updating the path.")]
	private float updatePathDelay = 1f;
	[Header("Radius"),LabelOverride("Has radius ?"),SerializeField]
	private bool hasRadius = true;
	[SerializeField,Range(.1f,20f), Hide("hasRadius",true)]
	private float rangeRadius = 5f;
	[SerializeField, LabelOverride("Stopping distance"),Hide("hasRadius",true)]
	[Tooltip("Distance at which ennemy stops from object.")]
	private float minDistance = .2f;
	[Header("Escape"), LabelOverride("Can escape ?"), Hide("hasRadius",true), SerializeField]
	[Tooltip("If ennemy can escape from object.")]
	private bool isEscaping = false;
	[SerializeField, LabelOverride("Escape Distance"), Hide("isEscaping")]
	private float escapeDistance=.1f;
	
	
	//Privates
	private PolyNavAgent nav;
	private float currentTime = 0;
	private Collider2D[] colliders;
	private bool isPlayerIn;
	private bool isFollowingPlayer;
	private RaycastHit2D hit;
	

	// Use this for initialization
	void Start () {
		nav = GetComponent<PolyNavAgent>();
		//Physics2D.IgnoreLayerCollision(8,9);
	}
	
	// Update is called once per frame
	void Update () {
		currentTime += Time.deltaTime;

		//Si l'ennemie est à la bonne distance
		if(Vector2.Distance(transform.position,player.transform.position)>=minDistance){	
			//Si l'ennemis peut bouger & que le player est dans la zone
			if(currentTime > updatePathDelay && colliders.Length!=0){
				if(hasRadius){	//Si le radius est activé
					foreach(Collider2D col in colliders){	//Cherche le bon collider et le suit
						if(col!=null){
							if(col.CompareTag("Player")){
								print("Following");
								nav.SetDestination(player.transform.position);
								isFollowingPlayer = true;
								currentTime = 0;
							}
						}
					}
				}
				else if(!hasRadius){	//Si le radius n'est pas activé
					nav.SetDestination(player.transform.position);
					currentTime = 0;
				}
			}
		}
		else{	//S'il n'ai pas à la bonne distance
			if(isFollowingPlayer){	// et que l'ennemis à un chemin à suivre
				nav.Stop();
				isFollowingPlayer = false;
				print("Path cleared");
			}
			//s'il peut fuir
			if(isEscaping){
				if(Vector2.Distance(transform.position,player.transform.position)<minDistance-escapeDistance){
					nav.SetDestination((transform.position-player.transform.position).normalized*10);
					//print((transform.position-player.transform.position).normalized);
					//hit = Physics2D.Raycast(transform.position, -Vector2.up);
					/*if(Physics2D.Raycast(transform.position, (transform.position-(player.transform.position*5)))){
						float distanceToGround = hit.distance;
						Debug.DrawRay(transform.position, (transform.position-(player.transform.position*5)) , Color.green);
						Debug.Log(hit.collider);
					}*/
					print("Escaping");
				}
			}
		}
	}

	void FixedUpdate(){
		if(hasRadius)
			colliders = Physics2D.OverlapCircleAll(this.transform.position,rangeRadius);
	}

	 void OnDrawGizmosSelected()
     {
		if(hasRadius){
			UnityEditor.Handles.color = Color.green;
			UnityEditor.Handles.DrawWireDisc(transform.position , transform.forward, rangeRadius);
			UnityEditor.Handles.color = Color.gray;
			UnityEditor.Handles.DrawWireDisc(transform.position , transform.forward, minDistance);
		}
     }
}

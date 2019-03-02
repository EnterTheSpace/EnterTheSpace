using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using PolyNav;

[RequireComponent(typeof(FieldOfView), typeof(Following), typeof(Escaping))]
public class Boss : Pawn {

    //SerializeFields
    [Header("Object"), LabelOverride("Player reference"), SerializeField]
    [Tooltip("Object to follow.")]
    private GameObject player;
    [Header("Collision"), SerializeField, LabelOverride("Have collision?")]
    [Tooltip("Have collision with player.")]
    private bool hasCollision = true;
    [Header("Path"), LabelOverride("Update delay"), SerializeField]
    [Tooltip("Delay in second for updating the path.")]
    private float updatePathDelay = 1f;
    [SerializeField] private SpriteRenderer spriteRender;
    [SerializeField] private Cooldown atkCD;
    [SerializeField] private Cooldown jmpMoveTime;

    //Privates
    private PolyNavAgent nav;
    private float currentTime;
    private FieldOfView fow;
    private Escaping escape;
    private Following follow;
    private Transform visibleTargets;
    private Transform lastVisibleTarget;

    private bool isAttacking;
    private Vector2 startJmpPos;

    // Use this for initialization
    void Start() {
        Initialization();
        nav = GetComponent<PolyNavAgent>();
        if (!hasCollision)
            nav.GetComponent<CircleCollider2D>().isTrigger = true;
        fow = GetComponent<FieldOfView>();
        escape = GetComponent<Escaping>();
        follow = GetComponent<Following>();
        visibleTargets = null;
    }

    // Update is called once per frame
    void Update() {
        if (!atkCD.Ready()) {
            atkCD.Decrease(Time.deltaTime);
        } else if(!isAttacking){
            Attack();
        }

        currentTime += Time.deltaTime;
        //Si l'ennemie est à la bonne distance
        if (!isAttacking) {
            if (Vector2.Distance(transform.position, player.transform.position) >= follow.minDistance + escape.escapeDistance) {
                //Si l'ennemis peut bouger & que le player est dans la zone
                if (currentTime > updatePathDelay) {
                    currentTime = follow.Follow(player, nav, currentTime);
                    if (player.transform.position.x > this.transform.position.x) {
                        spriteRender.flipX = true;
                    } else {
                        spriteRender.flipX = false;
                    }
                }
            } else {    //S'il n'ai pas à la bonne distance
                if (follow.isFollowingPlayer) { // et que l'ennemis à un chemin à suivre
                    nav.Stop();
                    follow.isFollowingPlayer = false;
                    print("Path cleared");
                }
            }
        } else {
            MoveTo();
        }
    }

    void FixedUpdate() {
        bool canEscape = true;
        if (escape.isEscaping) {
            if (canEscape)
                escape.Escape(player, nav);

            if (follow.isOverlapping) {
                visibleTargets = fow.FindVisibleTargets();
                if (visibleTargets != null) {
                    lastVisibleTarget = visibleTargets;
                } else {
                    if (lastVisibleTarget != null) {
                        nav.SetDestination(lastVisibleTarget.position);
                        canEscape = false;
                    }
                }
            }
        }
    }

    public void Attack() {
        isAttacking = true;
        startJmpPos = this.transform.position;
        jmpMoveTime.Reset();
        atkCD.Reset();
        spriteRender.GetComponent<Animator>().SetTrigger("atkJump");
    }

    public void MoveTo() {
        if (!jmpMoveTime.Ready()) {
            jmpMoveTime.Decrease(Time.deltaTime);

            this.transform.position = Vector2.Lerp(startJmpPos, player.transform.position, jmpMoveTime.Value() / jmpMoveTime.duration);
        } else {
            spriteRender.GetComponent<Animator>().SetTrigger("fallHitGround");
            isAttacking = false;
        }
    }
}

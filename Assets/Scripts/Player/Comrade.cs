﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using PolyNav;

[RequireComponent(typeof(FieldOfView), typeof(Following), typeof(Escaping))]
public class Comrade : Pawn {

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

    void Start() {
        escapeDistance = 0;
        nav = GetComponent<PolyNavAgent>();
        if (!hasCollision)
            nav.GetComponent<CircleCollider2D>().isTrigger = true;
        fow = GetComponent<FieldOfView>();
        escape = GetComponent<Escaping>();
        follow = GetComponent<Following>();
        attack = GetComponent<Attacking>();
        visibleTargets = null;
        Initialization();
    }
    
    void Update() {
        if (nav.velocity.x != 0 || nav.velocity.y != 0f)
            GetComponent<Animator>().SetBool("isMoving", true);
        else
            GetComponent<Animator>().SetBool("isMoving", false);

        if (follow != null && !isAttacking) {
            currentTime += Time.deltaTime;

            if (!escape.isEscaping)
                escapeDistance = follow.minDistance;
            else
                escapeDistance = escape.escapeDistance;

            if (player != null) {
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
                    }
                }

                EnnemyManager[] temp = FindObjectsOfType<EnnemyManager>();

                foreach (EnnemyManager enemy in temp) {
                    if (Vector2.Distance(transform.position, player.transform.position) <= fow.viewRadius) {
                        GetComponent<AimController>().enabled = true;
                        GetComponent<AimController>().SetTarget(enemy.transform);
                        break;
                    } else {
                        GetComponent<AimController>().SetTarget(player.transform);
                        GetComponent<AimController>().enabled = false;
                    }
                }
            }

        }
        SpriteOrientation();
    }

    void FixedUpdate() {

        if (escape.isEscaping) {
            escape.Escape(player, nav);

            if (follow != null) {
                if (follow.isOverlapping) {
                    visibleTargets = fow.FindVisibleTargets();
                    if (visibleTargets != null) {
                        if (hasWeapon) {
                            GetComponent<WeaponController>().TryShot(true);
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
                weap.BarrelRef().transform.localPosition = new Vector3(0.342f, -0.032f);
            }
            if (nav.velocity.x != 0f || nav.velocity.y != 0f) {//If walking
                aim.aimingTrans.localPosition = new Vector3(0f, -0.08f, 0f);
            } else {
                aim.aimingTrans.localPosition = new Vector3(0.038f, -0.068f, 0f);
            }
        } else if (diff.x > 0f)// aiming right
          {
            this.GetComponent<SpriteRenderer>().flipX = true;
            if (aim.aimingTrans.localScale.x == 1f)
                aim.aimingTrans.localScale = new Vector3(-1f, 1f, 1f);

            if (!weap.m_weaponSprite.GetComponent<SpriteRenderer>().flipX) {
                weap.m_weaponSprite.GetComponent<SpriteRenderer>().flipX = true;
                weap.m_weaponSprite.GetComponent<SpriteRenderer>().flipY = false;
                weap.BarrelRef().transform.localPosition = new Vector3(-0.354f, 0.03f);
            }
            if (nav.velocity.x != 0f || nav.velocity.y != 0f) {//If walking
                aim.aimingTrans.localPosition = new Vector3(-0.019f, -0.068f, 0f);
            } else {
                aim.aimingTrans.localPosition = new Vector3(-0.04f, -0.035f, 0f);
            }
        }
    }
}
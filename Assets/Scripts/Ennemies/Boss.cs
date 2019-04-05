using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using PolyNav;

[System.Serializable]
public struct Attack {
    [System.Serializable]
    public struct AtkPhase {
        public string name;
        public float duration;
    }
    public string name;
    public AtkPhase[] phases;
    public int currentPhase;
    public Cooldown cd;
    public AudioClip[] sound;
}


[RequireComponent(typeof(FieldOfView), typeof(Following), typeof(Escaping))]
[RequireComponent(typeof(Attacking))]
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
    [SerializeField] WeaponController weap;

    [SerializeField] private Cooldown atkCD;
    public Attack[] attacks;
    private int currentAttack;
    private bool isAttacking;
    
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

    private SpriteRenderer spriteRender;

    // Use this for initialization
    void Start() {
        atkCD.Reset();

        escapeDistance = 0;
        nav = GetComponent<PolyNavAgent>();
        if (!hasCollision)
            nav.GetComponent<CircleCollider2D>().isTrigger = true;
        spriteRender = this.GetComponent<SpriteRenderer>();
        fow = GetComponent<FieldOfView>();
        escape = GetComponent<Escaping>();
        follow = GetComponent<Following>();
        attack = GetComponent<Attacking>();
        visibleTargets = null;
        Initialization();
    }

    // Update is called once per frame
    void Update() {
        print(isAttacking);
        if (!isAttacking) {
            if (!atkCD.Ready()) {
                atkCD.Decrease(Time.deltaTime);
            }else {
                Attack();
            }

            if (follow != null) {
                currentTime += Time.deltaTime;

                if (player.transform.position.x > transform.position.x)
                    spriteRender.flipX = true;
                else
                    spriteRender.flipY = false;

                if (!escape.isEscaping)
                    escapeDistance = follow.minDistance;
                else
                    escapeDistance = escape.escapeDistance;

                //Si l'ennemie est à la bonne distance
                if (Vector2.Distance(transform.position, player.transform.position) >= follow.minDistance + escapeDistance) {
                    //Si l'ennemis peut bouger & que le player est dans la zone
                    if (currentTime > updatePathDelay) {
                        currentTime = follow.Follow(player, nav, currentTime);
                    }
                    //attack.Attack(player.transform.position,follow.minDistance);
                } else {    //S'il n'ai pas à la bonne distance
                    if (follow.isFollowingPlayer) { // et que l'ennemis à un chemin à suivre
                        nav.Stop();
                        follow.isFollowingPlayer = false;
                        print("Path cleared");
                    }
                }
            }
        } else {
            HandleAttack();
        }
    }


    public override void ApplyDamages(float damages) {
        if(Random.Range(0, 3) == 2)
            this.GetComponent<AudioSource>().PlayOneShot(hitSounds[Random.Range(0, hitSounds.Length - 1)], .5f);
        if (GetComponent<Animation>() != null)
            GetComponent<Animation>().Play("Hit");
        health -= damages;

        health = Mathf.Clamp(health, 0f, maxHealth);

        healthBar.health = health / maxHealth;
        if (health <= 0f)
            Destroy(this.gameObject);
    }

    void FixedUpdate() {
        if (!isAttacking) {
            if (escape.isEscaping) {
                escape.Escape(player, nav);

                if (follow.isOverlapping) {
                    visibleTargets = fow.FindVisibleTargets();
                    if (visibleTargets != null) {
                        lastVisibleTarget = visibleTargets;
                    } else {
                        if (lastVisibleTarget != null) {
                            nav.SetDestination(lastVisibleTarget.position);
                        }
                    }
                }
            }
        }
    }

    public void Attack() {
        currentAttack = Random.Range(0, attacks.Length - 1);
        for (int i = 0; i < attacks[currentAttack].sound.Length; i++) {
            this.GetComponent<AudioSource>().PlayOneShot(attacks[currentAttack].sound[i]);
        }
        isAttacking = true;
        attacks[currentAttack].cd.SetNew(attacks[currentAttack].phases[attacks[currentAttack].currentPhase].duration);
        attacks[currentAttack].cd.Reset();
        spriteRender.GetComponent<Animator>().SetTrigger(attacks[currentAttack].name);
    }

    public void HandleAttack() {
        if (attacks[currentAttack].cd.Ready()) {
            if (NextAtkPhase()) {
                spriteRender.GetComponent<Animator>().SetTrigger(attacks[currentAttack].name);
                print("played : " + attacks[currentAttack].phases[attacks[currentAttack].currentPhase].name);
            }
        } else {
            if (attacks[currentAttack].name == "shaka") {
                weap.TryShot(true);
            }
            weap.BarrelRef().Rotate(new Vector3(0f, 0f, 1f));
            attacks[currentAttack].cd.Decrease(Time.deltaTime);
        }
    }

    public bool NextAtkPhase() {
        if(attacks[currentAttack].currentPhase < attacks[currentAttack].phases.Length - 1) {
            attacks[currentAttack].currentPhase++;
            attacks[currentAttack].cd.SetNew(attacks[currentAttack].phases[attacks[currentAttack].currentPhase].duration);

            return true;
        } else {
            spriteRender.GetComponent<Animator>().SetTrigger("idle");
            isAttacking = false;
            atkCD.Reset();

            return false;
        }
    }
}

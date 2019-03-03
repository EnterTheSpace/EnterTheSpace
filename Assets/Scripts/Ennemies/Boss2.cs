using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using PolyNav;

[RequireComponent(typeof(FieldOfView), typeof(Following), typeof(Escaping))]
public class Boss2 : Pawn {

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
    public float timeBeforeAttackAgain = 10f;
    public SpriteRenderer spriteRender;
    public float damage = 40;


    //Privates
    private PolyNavAgent nav;
    private float currentTime;
    private float currentTimeAttack;
    private FieldOfView fow;
    private Escaping escape;
    public Following follow;
    private Attacking attack;
    private Transform visibleTargets;
    private Transform lastVisibleTarget;
    private float escapeDistance;
    private bool isAttacking = false;
    public float lastMinDistance;
    private bool isTriggered;
    private bool isColliding;
    private bool takedDamage;

    // Use this for initialization
    void Start()
    {
        takedDamage = false;
        isTriggered = false;
        isColliding = false;
        Initialization();
        currentTimeAttack = timeBeforeAttackAgain;
        escapeDistance = 0;
        nav = GetComponent<PolyNavAgent>();
        if (!hasCollision)
            nav.GetComponent<CircleCollider2D>().isTrigger = true;
        fow = GetComponent<FieldOfView>();
        escape = GetComponent<Escaping>();
        follow = GetComponent<Following>();
        attack = GetComponent<Attacking>();
        visibleTargets = null;
        lastMinDistance = follow.minDistance;
    }

    // Update is called once per frame
    void Update()
    {
        currentTimeAttack += Time.deltaTime;
        if (isAttacking)
        {
            //nav.SetDestination(lastVisibleTarget.position);
            if (spriteRender.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Sheep_Jump") &&
                    spriteRender.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                follow.minDistance = 0;
                if (isTriggered){
                    spriteRender.GetComponent<Animator>().SetTrigger("beginFall");
                    spriteRender.GetComponent<Animator>().ResetTrigger("atkJump");
                }
            }

            if (spriteRender.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Sheep_Falling") &&
                    spriteRender.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                if ((isTriggered || isColliding) && !takedDamage)
                {
                    player.GetComponent<Player>().ApplyDamages(damage);
                    takedDamage = true;
                }
            }
        }

            if (follow != null)
            {
                currentTime += Time.deltaTime;

                if (!escape.isEscaping)
                    escapeDistance = follow.minDistance;
                else
                    escapeDistance = escape.escapeDistance;

                //Si l'ennemie est à la bonne distance
                if (Vector2.Distance(transform.position, player.transform.position) >= follow.minDistance + escapeDistance)
                {
                    //Si l'ennemis peut bouger & que le player est dans la zone
                    if (currentTime > updatePathDelay)
                    {
                        currentTime = follow.Follow(player, nav, currentTime);
                        if (player.transform.position.x > this.transform.position.x)
                        {
                            spriteRender.flipX = true;
                        }
                        else
                        {
                            spriteRender.flipX = false;
                        }
                    }
                }
                else
                {   //S'il n'ai pas à la bonne distance
                    if (follow.isFollowingPlayer)
                    {   // et que l'ennemis à un chemin à suivre
                        follow.isFollowingPlayer = false;
                        nav.Stop();
                        print("Path cleared");
                    }
                }
            }
        
    }

    void FixedUpdate()
    {

        if (escape.isEscaping)
        {
            escape.Escape(player, nav);

            if (follow.isOverlapping)
            {
                visibleTargets = fow.FindVisibleTargets();
                if (visibleTargets != null)
                {
                    lastVisibleTarget = visibleTargets;
                    if (currentTimeAttack > timeBeforeAttackAgain)
                    {
                        spriteRender.GetComponent<Animator>().SetTrigger("atkJump");
                        spriteRender.GetComponent<Animator>().ResetTrigger("fallHitGround");
                        GetComponent<CircleCollider2D>().isTrigger = true;
                        isAttacking = true;
                        currentTimeAttack = 0;
                        takedDamage = false;
                    }
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
        if (health <= 0f)
        {
            follow = null;
            nav.Stop();
            gameObject.GetComponent<Animator>().SetTrigger("isDead");
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            isColliding = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            isColliding = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            isTriggered = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            isTriggered = false;
    }
}

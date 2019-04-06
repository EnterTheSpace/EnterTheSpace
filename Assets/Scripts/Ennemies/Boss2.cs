using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using PolyNav;

[RequireComponent(typeof(FieldOfView), typeof(Following), typeof(Escaping))]
public class Boss2 : Pawn {

    public AudioClip[] jmpSound;
    public GameObject jmpParticles;
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
    public bool isTriggered;
    private bool isColliding;
    private bool takedDamage;
    private bool animIsPlaying;

    // Use this for initialization
    void Start()
    {
        animIsPlaying = false;
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
                animIsPlaying = true;
                follow.minDistance = 0;
                if (isTriggered){
                    spriteRender.GetComponent<Animator>().SetTrigger("beginFall");
                    spriteRender.GetComponent<Animator>().ResetTrigger("atkJump");
                    StartCoroutine(WaitFalling());
                }
            }
        }

        if (follow != null && !animIsPlaying)
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

    IEnumerator WaitFalling()
    {
        nav.Stop();
        yield return new WaitForSeconds(2);
        animIsPlaying = false;
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
                        if (!isAttacking) {
                            spriteRender.GetComponent<Animator>().SetTrigger("atkJump");
                            Invoke("JumpSound", spriteRender.GetComponent<Animator>().speed * 1.5f);
                            spriteRender.GetComponent<Animator>().ResetTrigger("fallHitGround");
                            GetComponent<CircleCollider2D>().enabled = true;
                            spriteRender.GetComponent<CircleCollider2D>().enabled = false;
                            isAttacking = true;
                            currentTimeAttack = 0;
                            takedDamage = false;
                            StartCoroutine(WaitFalling());
                        }
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

    public override void ApplyDamages(float damages) {
        if (Random.Range(0, 3) == 2)
            this.GetComponent<AudioSource>().PlayOneShot(hitSounds[Random.Range(0, hitSounds.Length - 1)], .5f);
        health -= damages;

        health = Mathf.Clamp(health, 0f, maxHealth);

        healthBar.health = health / maxHealth;
        if (health <= 0f)
        {
            follow = null;
            nav.Stop();
            gameObject.GetComponent<Animator>().SetTrigger("isDead");
        }
    }

    public void JumpSound() {
        GetComponent<AudioSource>().PlayOneShot(jmpSound[Random.Range(0, jmpSound.Length - 1)]);
        if (jmpParticles != null) {
            GameObject temp = Instantiate(jmpParticles, transform.GetChild(0).position + new Vector3(0f, 0.3f), Quaternion.identity, null);

            Destroy(temp, 1f);
        }
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
            player.GetComponent<Player>().ApplyDamages(damage);
    }
}

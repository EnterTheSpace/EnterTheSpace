using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ProjectileInfos
{
    public float speed;
    public float damages;
    [HideInInspector] public Vector3 direction;
    public float destroyer;
    public bool bounce;
}

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    private ProjectileInfos infos;
            //REFERENCES
    protected Rigidbody2D rigidBody;
    protected Collider2D cCollider;

    private void Awake()
    {
        rigidBody = this.GetComponent<Rigidbody2D>();
        cCollider = this.GetComponent<Collider2D>();
    }

    public void Initialization(ProjectileInfos infos, Vector3 direction)
    {
        this.infos = infos;

        this.infos.direction = direction.normalized;

        if(this.infos.bounce)
            this.cCollider.isTrigger = false;
        else
            this.cCollider.isTrigger = true;
        this.rigidBody.velocity = this.infos.direction * this.infos.speed;

        Invoke("Destructor", this.infos.destroyer);
    }

    public void Destructor()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Grid")){
            if(other.gameObject.GetComponent<Pawn>() != null && !other.CompareTag("Parry"))
            {
                other.gameObject.GetComponent<Pawn>().ApplyDamages(this.infos.damages);
                Destructor();
            }else if(other.gameObject.GetComponent<Projectile>() == null)
            {
                Destructor();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.GetComponent<Pawn>() != null)
        {
            other.gameObject.GetComponent<Pawn>().ApplyDamages(this.infos.damages);
            Destructor();
        }else if(other.gameObject.GetComponent<Projectile>() == null && !this.infos.bounce)
        {
            Destructor();
        }
    }
}

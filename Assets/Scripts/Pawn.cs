using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pawn : MonoBehaviour 
{
    [SerializeField] protected float health = 100;
    public float Health
    {
        get {return health;}
        set {health = value;}
    }
    [SerializeField, LabelOverride("Health bar widget")] protected HealthBar healthBar;

    protected virtual void Initialization()
    {
        //Init
    }

    public virtual void ApplyDamages(float damages)
    {
        health -= damages;
        health = Mathf.Clamp(health, 0f, 100f);

        healthBar.health = health * 0.01f;
        if(health <= 0f)
            Destroy(this.gameObject);
    }
}

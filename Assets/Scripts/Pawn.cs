﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pawn : MonoBehaviour 
{
    public AudioClip[] deathSounds;
    public AudioClip[] hitSounds;
    [SerializeField] protected float health = 100;
    public float Health
    {
        get {return health;}
        set {health = value;}
    }
    protected float maxHealth;
    [SerializeField, LabelOverride("Health bar widget")] protected HealthBar healthBar;

    protected virtual void Initialization()
    {
        maxHealth = health;
    }

    public virtual void ApplyDamages(float damages) {
        this.GetComponent<AudioSource>().PlayOneShot(hitSounds[Random.Range(0, hitSounds.Length - 1)]);
        health -= damages;

        health = Mathf.Clamp(health, 0f, maxHealth);

        healthBar.health = health / maxHealth;
        if(health <= 0f)
            Destroy(this.gameObject);
    }

    public void DestroyPawn()
    {
        Destroy(this.gameObject);
    }
}

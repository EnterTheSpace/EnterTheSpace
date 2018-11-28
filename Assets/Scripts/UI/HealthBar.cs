using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    public float health {set; get;}
    private Slider healthBar;

    private void Start()
    {
        healthBar = this.GetComponent<Slider>();
        health = 1f;
    }

    // Update is called once per frame
    void Update ()
    {
        healthBar.value = health;
        if(health < 0f)
            health = 0f;
    }
}

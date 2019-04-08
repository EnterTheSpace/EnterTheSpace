using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : Interactable
{
    public AudioClip collect;
    public int value;

    public override void Interact() {
        GameObject.Find("Player").GetComponent<InventoryController>().money += value;
        GetComponent<Animator>().SetTrigger("pick");
        GetComponent<AudioSource>().PlayOneShot(collect);
        Invoke("Release", 0.3f);
    }

    public override void Release() {
        Destroy(this.gameObject);
    }
}

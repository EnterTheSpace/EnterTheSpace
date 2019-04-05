using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eventBoss : MonoBehaviour
{
    public Boss2 boss;
    public Player player;
    public AudioClip[] landSounds;

    public void OnEndFalling()
    {
        boss.GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = true;
        if(!GetComponent<AudioSource>().isPlaying)
            GetComponent<AudioSource>().PlayOneShot(landSounds[Random.Range(0, landSounds.Length-1)]);
        GetComponent<Animator>().SetTrigger("fallHitGround");
        GetComponent<Animator>().ResetTrigger("beginFall");
        boss.follow.minDistance = boss.lastMinDistance;
    }

    
}

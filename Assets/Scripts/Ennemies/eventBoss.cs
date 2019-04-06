using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eventBoss : MonoBehaviour
{
    public Boss2 boss;
    public Player player;
    public AudioClip[] landSounds;
    public GameObject[] landParticles;

    public void OnEndFalling()
    {
        boss.GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = true;
        if(!GetComponent<AudioSource>().isPlaying)
            GetComponent<AudioSource>().PlayOneShot(landSounds[Random.Range(0, landSounds.Length-1)]);

        if (landParticles != null) {
            GameObject temp = Instantiate(landParticles[Random.Range(0, landParticles.Length - 1)], transform);

            temp.transform.localPosition = new Vector3(-0.04f, -1.25f);
            temp.transform.localScale = new Vector3(6f, 4f);
            temp.GetComponent<Animator>().speed = 2f;
            Destroy(temp, 1f);
        }
        GetComponent<Animator>().SetTrigger("fallHitGround");
        GetComponent<Animator>().ResetTrigger("beginFall");
        boss.follow.minDistance = boss.lastMinDistance;
    }
}

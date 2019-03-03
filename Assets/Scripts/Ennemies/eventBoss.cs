using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eventBoss : MonoBehaviour
{
    public Boss2 boss;
    public Player player;

    public void OnEndFalling()
    {
        boss.GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = true;
        GetComponent<Animator>().SetTrigger("fallHitGround");
        GetComponent<Animator>().ResetTrigger("beginFall");
        boss.follow.minDistance = boss.lastMinDistance;
    }

    
}

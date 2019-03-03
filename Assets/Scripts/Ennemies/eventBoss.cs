using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eventBoss : MonoBehaviour
{
    public Boss2 boss;

    public void OnEndFalling()
    {
        boss.GetComponent<CircleCollider2D>().isTrigger = false;
        GetComponent<Animator>().SetTrigger("fallHitGround");
        GetComponent<Animator>().ResetTrigger("beginFall");
        boss.follow.minDistance = boss.lastMinDistance;
    }
}

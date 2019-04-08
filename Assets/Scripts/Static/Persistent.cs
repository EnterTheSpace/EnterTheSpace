using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInfos {
    public uint projetPerShot;
    public float dispersion;
    public float fireRate;
    public ProjectileInfos projInfos;
}

public static class Persistent {
    public static string weapInfos;
    public static bool parry;
    public static Sprite weapSprite;
    public static float dashLength = 0f;
    public static List<Sprite> items = new List<Sprite>();
}

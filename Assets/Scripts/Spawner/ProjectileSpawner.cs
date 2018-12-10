using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : WeaponController
{

	protected override void Update()
	{
		base.Update();
		TryShot(true);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : Weapon
{

	protected override void Update()
	{
		base.Update();
		TryShot(this.transform.up);
	}
}

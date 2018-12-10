using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(WeaponController))]
public class FieldOfViewWeapon : Editor
{

	private void OnSceneGUI()
	{
		WeaponController wp = (WeaponController)target;
		Handles.color = Color.green;

		Vector3 viewAngleA = wp.DirFromAngle((-wp.dispersion)/2 + 90f, false);
		Vector3 viewAngleB = wp.DirFromAngle((wp.dispersion)/2 + 90f, false);
		Vector3 viewStraight = wp.DirFromAngle(90f, false);
	
		//mDebug.Log(wp.BarrelRef().right);
		Handles.DrawWireArc (wp.BarrelRef().position, wp.BarrelRef().forward, viewAngleB, wp.dispersion, wp.distance);
		//Handles.DrawLine(wp.BarrelRef().position, wp.BarrelRef().position + wp.BarrelRef().right * wp.distance);
		Handles.DrawLine(wp.BarrelRef().position, wp.BarrelRef().position + viewAngleA * wp.distance);
		Handles.DrawLine(wp.BarrelRef().position, wp.BarrelRef().position + viewAngleB * wp.distance);
		Handles.DrawLine(wp.BarrelRef().position, wp.BarrelRef().position + viewStraight * wp.distance);

		Handles.color = Color.red;
        uint nbProj = (wp.projPerShot > 0) ? wp.projPerShot : 1;
        float angleStep = (nbProj % 2 == 0) ? (wp.dispersion / nbProj): (wp.dispersion / (nbProj - 1) );

		float shotRayAngleA = (-wp.dispersion)/2 - wp.BarrelRef().eulerAngles.z + 90f;
        float shotRayAngleB = (wp.dispersion)/2 - wp.BarrelRef().eulerAngles.z + 90f;

        for(int i = 0 ; i < nbProj ; i ++)
        {
			if(i%2 == 0)
			{
				Handles.DrawLine(wp.BarrelRef().position, wp.BarrelRef().position + new Vector3(
																						Mathf.Sin(shotRayAngleB * Mathf.Deg2Rad),
																						Mathf.Cos(shotRayAngleB * Mathf.Deg2Rad),
																						0f) * wp.distance
								);
				shotRayAngleB -= angleStep;
			}else
			{
				Handles.DrawLine(wp.BarrelRef().position, wp.BarrelRef().position + new Vector3(
																						Mathf.Sin(shotRayAngleA * Mathf.Deg2Rad),
																						Mathf.Cos(shotRayAngleA * Mathf.Deg2Rad),
																						0f) * wp.distance
								);
				shotRayAngleA += angleStep;
			}
        }
	}
}

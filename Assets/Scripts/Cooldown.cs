using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cooldown{

	public float duration;

	private float cooldown;

	public void SetException(float value)
	{
		cooldown = value;
	}

	public void Reset()
	{
		cooldown = duration;
	}

	public bool Ready()
	{
		return (cooldown <= 0f);
	}

	public void Decrease(float amount)
	{
		cooldown -= amount;
	}

    public float Value() {
        return cooldown;
    }
}

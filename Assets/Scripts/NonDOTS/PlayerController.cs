using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public static PlayerController instance;

	Rigidbody rb;

	public float maxHp = 100;
	public float hp = 100;
	public float maxCooldown = .5f;
	public float cooldown = 0;
	public float speed = 10;

	public float invincibleTime = 1f;

	public float expGainRate = 1f;

	public float exp = 0f;
	public float maxExp = 100f;

	public BombType bombType = BombType.Mine;

	public float mineDamage = 1;
	public float mineRadius = 1;

	public float grenadeDamage = 1;
	public float grenadeRadius = 1;

	public float dynamiteDamage = 1;
	public float dynamiteRadius = 1;

	private void Awake()
	{
		instance = this;
		rb = GetComponent<Rigidbody>();

	}

	void Update()
    {
		Vector3 dir = new Vector3 { x = Input.GetAxisRaw("Horizontal"), y = 0, z = Input.GetAxisRaw("Vertical") };
		dir.Normalize();
		dir = dir * speed * Time.deltaTime;
		transform.Translate(dir);

		if(Input.GetButtonDown("Submit") && cooldown < .01f)
		{
			cooldown = maxCooldown;
			// TODO : spawn bomb
		}
    }
}

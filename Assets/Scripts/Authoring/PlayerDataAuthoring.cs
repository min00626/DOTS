using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class PlayerDataAuthoring : MonoBehaviour
{
	public class Baker : Baker<PlayerDataAuthoring>
	{
		public override void Bake(PlayerDataAuthoring authoring)
		{
			Entity entity = GetEntity(TransformUsageFlags.Dynamic);
			AddComponent(entity, new PlayerData
			{
				maxHp = 100f,
				hp = 100f,
				maxCooldown = .5f,
				cooldown = 0f,
				speed = 13f,
				bombType = BombType.Mine,
				mineDamage = 1f,
				mineRadius = 1f,
				grenadeDamage = 1f,
				grenadeRadius = 1f,
				dynamiteDamage = 1f,
				dynamiteRadius = 1f,
				invincibleTime = 1f,
				maxExp = 100f,
				exp = 0f,
				expGainRate = 1f,
			});
		}
	}
}

public struct PlayerData : IComponentData
{
	public float maxHp;
	public float hp;
	public float maxCooldown;
	public float cooldown;
	public float speed;

	public float invincibleTime;

	public float expGainRate;

	public float exp;
	public float maxExp;

	public BombType bombType;

	public float mineDamage;
	public float mineRadius;

	public float grenadeDamage;
	public float grenadeRadius;

	public float dynamiteDamage;
	public float dynamiteRadius;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.UI;
using Unity.VisualScripting;
using System;

public class EnemyAuthoring : MonoBehaviour
{
	public float hp;
	public float attack;

	public class Baker : Baker<EnemyAuthoring>
	{
		public override void Bake(EnemyAuthoring authoring)
		{
			Entity entity = GetEntity(TransformUsageFlags.Dynamic);
			AddComponent(entity, new Enemy
			{
				hp = authoring.hp,
				maxHp = authoring.hp,
				attack = authoring.attack,
			});
		}
	}
}

public struct Enemy : IComponentData
{
	public float hp;
	public float maxHp;
	public float attack;
}

public class EnemyHpBar : IComponentData
{
	public Image image;
}

public struct DamagedEnemyTag : IComponentData
{

}
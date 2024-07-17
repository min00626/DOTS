using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpawnEnemyConfigAuthoring : MonoBehaviour
{
	public GameObject enemyPrefab;
	public int amount;

	public class Baker : Baker<SpawnEnemyConfigAuthoring>
	{
		public override void Bake(SpawnEnemyConfigAuthoring authoring)
		{
			Entity entity = GetEntity(TransformUsageFlags.None);
			AddComponent(entity, new SpawnEnemyConfig
			{
				enemyPrefabEntity = GetEntity(authoring.enemyPrefab, TransformUsageFlags.Dynamic),
				amount = authoring.amount
			});
		}
	}
}

public struct SpawnEnemyConfig : IComponentData
{
	public Entity enemyPrefabEntity;
	public int amount;
}
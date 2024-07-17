using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpawnVfxConfigAuthoring : MonoBehaviour
{
	public GameObject vfxBombExplosionPrefab;
	public GameObject vfxEnemyExplosionPrefab;

	public class Baker : Baker<SpawnVfxConfigAuthoring>
	{
		public override void Bake(SpawnVfxConfigAuthoring authoring)
		{
			Entity entity = GetEntity(TransformUsageFlags.None);
			AddComponent(entity, new SpawnVfxConfig
			{
				vfxBombExplosionEntity = GetEntity(authoring.vfxBombExplosionPrefab, TransformUsageFlags.None),
				vfxEnemyExplosionEntity = GetEntity(authoring.vfxEnemyExplosionPrefab, TransformUsageFlags.None),
			});
		}
	}
}

public struct SpawnVfxConfig : IComponentData
{
	public Entity vfxBombExplosionEntity;
	public Entity vfxEnemyExplosionEntity;
}

public struct VfxTimer : IComponentData
{
	public float timer;
}
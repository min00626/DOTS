using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class SpawnBombConfigAuthoring : MonoBehaviour
{
	public GameObject minePrefab;
	public GameObject grenadePrefab;
	public GameObject dynamitePrefab;

	public class Baker : Baker<SpawnBombConfigAuthoring>
	{
		public override void Bake(SpawnBombConfigAuthoring authoring)
		{
			Entity entity = GetEntity(TransformUsageFlags.None);
			AddComponent(entity, new SpawnBombConfig
			{
				minePrefabEntity = GetEntity(authoring.minePrefab, TransformUsageFlags.Dynamic),
				grenadePrefabEntity = GetEntity(authoring.grenadePrefab, TransformUsageFlags.Dynamic),
				dynamitePrefabEntity = GetEntity(authoring.dynamitePrefab, TransformUsageFlags.Dynamic)
			});
		}
	}
}
 
public struct SpawnBombConfig : IComponentData
{
	public Entity minePrefabEntity;
	public Entity grenadePrefabEntity;
	public Entity dynamitePrefabEntity;
}
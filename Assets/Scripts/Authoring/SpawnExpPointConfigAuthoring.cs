using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SpawnExpPointConfigAuthroing : MonoBehaviour
{
	public GameObject expPointPrefab;

	public class Baker : Baker<SpawnExpPointConfigAuthroing>
	{
		public override void Bake(SpawnExpPointConfigAuthroing authoring)
		{
			Entity entity = GetEntity(TransformUsageFlags.None);
			AddComponent(entity, new SpawnExpPointConfig
			{
				expPointPrefabEntity = GetEntity(authoring.expPointPrefab, TransformUsageFlags.Dynamic),
			});
		}
	}
}

public struct SpawnExpPointConfig : IComponentData
{
	public Entity expPointPrefabEntity;
}
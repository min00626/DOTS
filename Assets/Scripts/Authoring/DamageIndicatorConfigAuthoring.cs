using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class DamageIndicatorConfigAuthoring : MonoBehaviour
{
	[SerializeField] GameObject glyphPrefab;
	[SerializeField] float glyphWidth = 1;

	public class Baker : Baker<DamageIndicatorConfigAuthoring>
	{
		public override void Bake(DamageIndicatorConfigAuthoring authoring)
		{
			Entity entity = GetEntity(TransformUsageFlags.None);
			AddComponent(entity, new DamageIndicatorConfig
			{
				glyphEntity = GetEntity(authoring.glyphPrefab, TransformUsageFlags.Dynamic),
				glyphWidth = authoring.glyphWidth,

			});
		}
	}
}

public struct DamageIndicatorConfig : IComponentData
{
	public Entity glyphEntity;
	public float glyphWidth;
}
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ExpPointAuthoring : MonoBehaviour
{
	public class Baker : Baker<ExpPointAuthoring>
	{
		public override void Bake(ExpPointAuthoring authoring)
		{
			Entity entity = GetEntity(TransformUsageFlags.Dynamic);
			AddComponent(entity, new ExpPoint
			{
				exp = 10f
			});
		}
	}
}

public struct ExpPoint : IComponentData
{
	public float exp;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System.Threading;

public class DynamiteAuthoring : MonoBehaviour
{
	public class Baker : Baker<DynamiteAuthoring>
	{
		public override void Bake(DynamiteAuthoring authoring)
		{
			Entity entity = GetEntity(TransformUsageFlags.None);
			AddComponent(entity, new Dynamite());
		}
	}


}

public struct Dynamite : IComponentData
{ }
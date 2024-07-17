using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System.Threading;

public class GrenadeAuthoring : MonoBehaviour
{
	public class Baker : Baker<GrenadeAuthoring>
	{
		public override void Bake(GrenadeAuthoring authoring)
		{
			Entity entity = GetEntity(TransformUsageFlags.Dynamic);
			AddComponent(entity, new Grenade());
		}
	}


}

public struct Grenade : IComponentData
{ }
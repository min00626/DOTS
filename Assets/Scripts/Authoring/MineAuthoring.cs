using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System.Threading;

public class MineAuthoring : MonoBehaviour
{
	public class Baker : Baker<MineAuthoring>
	{
		public override void Bake(MineAuthoring authoring)
		{
			Entity entity = GetEntity(TransformUsageFlags.None);
			AddComponent(entity, new Mine());
		}
	}


}

public struct Mine : IComponentData
{ }
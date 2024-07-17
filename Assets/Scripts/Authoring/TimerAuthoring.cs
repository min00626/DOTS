using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class TimerAuthoring : MonoBehaviour
{
	public float timer;
	public class Baker : Baker<TimerAuthoring>
	{
		public override void Bake(TimerAuthoring authoring)
		{
			Entity entity = GetEntity(TransformUsageFlags.Dynamic);
			AddComponent(entity, new Timer { timer = authoring.timer  });
		}
	}
}

public struct Timer : IComponentData
{
	public float timer;
}

public struct BombData : IComponentData
{
	public float radius;
	public float damage;
}

public enum BombType { Mine, Grenade, Dynamite}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class InputControlAuthoring : MonoBehaviour
{
	private class Baker : Baker<InputControlAuthoring>
	{
		public override void Bake(InputControlAuthoring authoring)
		{
			Entity entity = GetEntity(TransformUsageFlags.Dynamic);
			AddComponent(entity, new InputControl
			{
				direction = new float2(),
				confirm = false,
				changeWeapon = false,
			});
		}
	}
}

public struct InputControl : IComponentData
{
	public float2 direction;
	public bool confirm;
	public bool changeWeapon;
}

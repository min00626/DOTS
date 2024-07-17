using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct EnemyControlSystem : ISystem
{
	private EntityQuery entityQuery;

	public void OnCreate(ref SystemState state)
	{
		entityQuery = state.GetEntityQuery(ComponentType.ReadOnly<Player>(), ComponentType.ReadOnly<LocalTransform>());

		state.RequireForUpdate<Enemy>();
		state.RequireForUpdate(entityQuery);
	}

	public void OnUpdate(ref SystemState state)
	{
		Entity player = entityQuery.GetSingletonEntity();
		LocalTransform playerLocalTransform = state.EntityManager.GetComponentData<LocalTransform>(player);
		new ControlEnemyJob { 
			deltaTime = SystemAPI.Time.DeltaTime,
			playerLocalTransform = playerLocalTransform,
		}.Schedule();
	}

	[BurstCompile]
	[WithAll(typeof(Enemy))]
	public partial struct ControlEnemyJob : IJobEntity
	{
		public float deltaTime;
		public LocalTransform playerLocalTransform;

		public void Execute(ref LocalTransform localTransform)
		{

			float3 dir = playerLocalTransform.Position - localTransform.Position;
			dir.y = 0;
			dir = math.normalizesafe(dir);

			//TODO : make enemy look at player
			localTransform.Rotation = quaternion.LookRotationSafe(dir, math.up());
			float distance = math.length(playerLocalTransform.Position - localTransform.Position);
			if (distance < 3f)
			{
				return;
			}
			localTransform = localTransform.Translate(dir * deltaTime * 4);

		}
	}
}

using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using static UnityEngine.EventSystems.EventTrigger;
using Unity.Physics;
using Unity.Burst;


public partial struct SpawnEnemySystem : ISystem
{
	private EntityQuery entityQuery;

	public void OnCreate(ref SystemState state)
	{
		entityQuery = state.GetEntityQuery(ComponentType.ReadOnly<Player>(), ComponentType.ReadOnly<LocalTransform>());

		state.RequireForUpdate<SpawnEnemyConfig>();
		state.RequireForUpdate(entityQuery);
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		SpawnEnemyConfig spawnEnemyConfig = SystemAPI.GetSingleton<SpawnEnemyConfig>();

		if (!entityQuery.IsEmpty)
		{
			/*
			PhysicsJoint joint = new PhysicsJoint();
			FixedList128Bytes<Constraint> constraints = new FixedList128Bytes<Constraint> {
				new Constraint
				{
					ConstrainedAxes = new bool3{x=false, y=true, z=false},
					Type = ConstraintType.Linear,
					Min = 0,
					Max = 0,
					SpringFrequency = Constraint.DefaultSpringFrequency,
					DampingRatio = Constraint.DefaultDampingRatio
				},
				new Constraint
				{
					ConstrainedAxes =new bool3{x=true, y = false, z = true},
					Type = ConstraintType.Angular,
					Min = 0,
					Max = 0,
					SpringFrequency = Constraint.DefaultSpringFrequency,
					DampingRatio = Constraint.DefaultDampingRatio
				}
			};

			joint.SetConstraints(constraints);
			*/
			Entity playerEntity = entityQuery.GetSingletonEntity();
			LocalTransform playerTransform = state.EntityManager.GetComponentData<LocalTransform>(playerEntity);

			var ecb = new EntityCommandBuffer(Allocator.Temp);

			for (int i = 0; i < spawnEnemyConfig.amount; i++)
			{
				Entity enemyEntity = ecb.Instantiate(spawnEnemyConfig.enemyPrefabEntity);

				float2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
				float3 offset = new float3(randomDirection.x, 0, randomDirection.y) * 20;

				float3 enemyPosition = playerTransform.Position + offset;

				ecb.SetComponent(enemyEntity, new LocalTransform
				{
					Position = enemyPosition,
					Rotation = quaternion.identity,
					Scale = 1.0f
				});

				/*
				ecb.AddComponent(enemyEntity, joint);

				PhysicsConstrainedBodyPair pcbp = new PhysicsConstrainedBodyPair(enemyEntity, default, false);
				ecb.AddComponent(enemyEntity, pcbp);
				*/
			}
			SystemAPI.SetSingleton<SpawnEnemyConfig>(new SpawnEnemyConfig
			{
				enemyPrefabEntity = spawnEnemyConfig.enemyPrefabEntity,
				amount = 0
			});

			ecb.Playback(state.EntityManager);
			ecb.Dispose();
		}
	}
}


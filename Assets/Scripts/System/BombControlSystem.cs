using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial struct BombControlSystem : ISystem
{
	private EntityQuery enemyQuery;

	public void OnCreate(ref SystemState state)
	{
		enemyQuery = state.GetEntityQuery(ComponentType.ReadOnly<Enemy>(), ComponentType.ReadOnly<LocalTransform>());

		state.RequireForUpdate<BombData>();
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		SpawnVfxConfig spawnVfxConfig = SystemAPI.GetSingleton<SpawnVfxConfig>();

		EntityCommandBuffer jobEcb = new EntityCommandBuffer(Allocator.TempJob, PlaybackPolicy.MultiPlayback);

		NativeArray<Entity> entityArray = enemyQuery.ToEntityArray(Allocator.TempJob);
		NativeArray<LocalTransform> enemyTransformArray = enemyQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
		NativeArray<Enemy> enemyArray = enemyQuery.ToComponentDataArray<Enemy>(Allocator.TempJob);

		JobHandle mineControlJobHandle = new MineControlJob
		{
			enemyEntities = entityArray,
			enemyTransforms = enemyTransformArray,
			enemyData = enemyArray,
			ecb = jobEcb,
			spawnVfxConfig = spawnVfxConfig,
		}.Schedule(state.Dependency);

		mineControlJobHandle.Complete();

		if (!jobEcb.IsEmpty)
		{
			jobEcb.Playback(state.EntityManager);
		}

		JobHandle grenadeAndDynamiteControlJobHandle = new GrenadeAndDynamiteControlJob
		{
			enemyEntities = entityArray,
			enemyTransforms = enemyTransformArray,
			enemyData = enemyArray,
			ecb = jobEcb,
			deltaTime = SystemAPI.Time.DeltaTime,
			spawnVfxConfig = spawnVfxConfig,
		}.Schedule(mineControlJobHandle);

		grenadeAndDynamiteControlJobHandle.Complete();

		enemyArray.Dispose(grenadeAndDynamiteControlJobHandle);
		enemyTransformArray.Dispose(grenadeAndDynamiteControlJobHandle);
		entityArray.Dispose(grenadeAndDynamiteControlJobHandle);

		if (!jobEcb.IsEmpty)
		{
			jobEcb.Playback(state.EntityManager);
		}

		jobEcb.Dispose();
	}

	[BurstCompile]
	[WithAll(typeof(Mine))]
	public partial struct MineControlJob : IJobEntity
	{
		[ReadOnly] public NativeArray<Entity> enemyEntities;
		[ReadOnly] public NativeArray<LocalTransform> enemyTransforms;
		[ReadOnly] public NativeArray<Enemy> enemyData;

		internal EntityCommandBuffer ecb;

		public SpawnVfxConfig spawnVfxConfig;

		public void Execute(Entity mineEntity, in LocalTransform mineTransform, in BombData bombData)
		{

			var enemiesInBombRadius = new NativeList<int>(Allocator.Temp);
			bool isDetonated = false;

			for (int i = 0; i < enemyTransforms.Length; i++)
			{
				float distance = math.distance(mineTransform.Position, enemyTransforms[i].Position);
				if (distance < bombData.radius)
				{
					enemiesInBombRadius.Add(i);
				}
				if (distance <= 2.5f)
				{
					isDetonated = true;
				}
			}
			 
			if (isDetonated)
			{
				foreach (int i in enemiesInBombRadius)
				{
					ecb.SetComponent(enemyEntities[i], new Enemy { hp = enemyData[i].hp - bombData.damage, maxHp = enemyData[i].maxHp });
					ecb.AddComponent(enemyEntities[i], new DamagedEnemyTag());
				}
				ecb.DestroyEntity(mineEntity);
				Entity e = ecb.Instantiate(spawnVfxConfig.vfxBombExplosionEntity);
				ecb.SetComponent(e, new LocalTransform { Position = mineTransform.Position, Rotation = Quaternion.identity, Scale = 1f });
				ecb.AddComponent(e, new VfxTimer { timer = 1.5f });
			}
			enemiesInBombRadius.Dispose();
		}
	}

	[BurstCompile]
	[WithAny(typeof(Dynamite), typeof(Grenade))]
	public partial struct GrenadeAndDynamiteControlJob : IJobEntity
	{
		[ReadOnly] public NativeArray<Entity> enemyEntities;
		[ReadOnly] public NativeArray<LocalTransform> enemyTransforms;
		[ReadOnly] public NativeArray<Enemy> enemyData;

		internal EntityCommandBuffer ecb;

		public SpawnVfxConfig spawnVfxConfig;

		public float deltaTime;

		public void Execute(Entity dynamiteEntity, in LocalTransform dynamiteTransform, ref Timer bombTimer, in BombData bombData)
		{
			bombTimer.timer -= deltaTime;
			if (bombTimer.timer <= 0)
			{
				for (int i = 0; i < enemyTransforms.Length; i++)
				{
					if(math.distance(dynamiteTransform.Position, enemyTransforms[i].Position) < bombData.radius)
					{
						ecb.SetComponent(enemyEntities[i], new Enemy { hp = enemyData[i].hp - bombData.damage, maxHp = enemyData[i].maxHp });
						ecb.AddComponent(enemyEntities[i], new DamagedEnemyTag());
					}
				}
				ecb.DestroyEntity(dynamiteEntity);
				Entity e = ecb.Instantiate(spawnVfxConfig.vfxBombExplosionEntity);
				ecb.SetComponent(e, new LocalTransform { Position = dynamiteTransform.Position, Rotation = Quaternion.identity, Scale = 1f });
				ecb.AddComponent(e, new VfxTimer { timer = 1.5f });
			}
		}
	}
}

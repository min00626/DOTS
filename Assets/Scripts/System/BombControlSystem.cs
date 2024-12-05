using EcsDamageBubbles.Config;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

public partial struct BombControlSystem : ISystem
{
	private EntityQuery enemyQuery;

	public void OnCreate(ref SystemState state)
	{
		enemyQuery = state.GetEntityQuery(ComponentType.ReadOnly<Enemy>(), ComponentType.ReadOnly<LocalTransform>());

		state.RequireForUpdate<BombData>();
		state.RequireForUpdate<SpawnVfxConfig>();
		state.RequireForUpdate<SpawnExpPointConfig>();
		state.RequireForUpdate<DamageIndicatorConfig>();
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		SpawnVfxConfig spawnVfxConfig = SystemAPI.GetSingleton<SpawnVfxConfig>();
		SpawnExpPointConfig spawnExpPointConfig = SystemAPI.GetSingleton<SpawnExpPointConfig>();
		DamageIndicatorConfig damageIndicatorConfig = SystemAPI.GetSingleton<DamageIndicatorConfig>();

		EntityCommandBuffer jobEcb = new EntityCommandBuffer(Allocator.TempJob, PlaybackPolicy.MultiPlayback);

		NativeArray<Entity> entityArray = enemyQuery.ToEntityArray(Allocator.TempJob);
		NativeArray<LocalTransform> enemyTransformArray = enemyQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
		NativeArray<Enemy> enemyArray = enemyQuery.ToComponentDataArray<Enemy>(Allocator.TempJob);

		JobHandle distanceBombControlJob = new DistanceBombControlJob
		{
			enemyEntities = entityArray,
			enemyTransforms = enemyTransformArray,
			enemyData = enemyArray,
			ecb = jobEcb,
			spawnVfxConfig = spawnVfxConfig,
			spawnExpPointConfig = spawnExpPointConfig,
			damageIndicatorConfig = damageIndicatorConfig,
		}.Schedule(state.Dependency);

		distanceBombControlJob.Complete();

		if (!jobEcb.IsEmpty)
		{
			jobEcb.Playback(state.EntityManager);
		}

		JobHandle timerBombControlJob = new TimerBombControlJob
		{
			enemyEntities = entityArray,
			enemyTransforms = enemyTransformArray,
			enemyData = enemyArray,
			ecb = jobEcb,
			deltaTime = SystemAPI.Time.DeltaTime,
			spawnVfxConfig = spawnVfxConfig,
			spawnExpPointConfig = spawnExpPointConfig,
			damageIndicatorConfig = damageIndicatorConfig,
		}.Schedule(distanceBombControlJob);

		timerBombControlJob.Complete();

		enemyArray.Dispose(timerBombControlJob);
		enemyTransformArray.Dispose(timerBombControlJob);
		entityArray.Dispose(timerBombControlJob);

		if (!jobEcb.IsEmpty)
		{
			jobEcb.Playback(state.EntityManager);
		}

		jobEcb.Dispose();
	}

	[BurstCompile]
	[WithAny(typeof(Mine), typeof(Grenade))]
	public partial struct DistanceBombControlJob : IJobEntity
	{
		[ReadOnly] public NativeArray<Entity> enemyEntities;
		[ReadOnly] public NativeArray<LocalTransform> enemyTransforms;
		[ReadOnly] public NativeArray<Enemy> enemyData;

		public EntityCommandBuffer ecb;

		[ReadOnly] public SpawnVfxConfig spawnVfxConfig;
		[ReadOnly] public SpawnExpPointConfig spawnExpPointConfig;
		[ReadOnly] public DamageIndicatorConfig damageIndicatorConfig;

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
				Entity glyphEntity = damageIndicatorConfig.glyphEntity;
				foreach (int i in enemiesInBombRadius)
				{
					int damage = (int)bombData.damage;
					float newHp = enemyData[i].hp - bombData.damage;

					if (newHp > 0)
					{
						ecb.SetComponent(enemyEntities[i], new Enemy { hp = newHp, maxHp = enemyData[i].maxHp, attack = enemyData[i].attack });
					}
					else
					{
						ProcessDeadEnemy(spawnExpPointConfig, spawnVfxConfig, ecb, enemyTransforms[i], enemyEntities[i]);
					}
					SpawnGlyph(damageIndicatorConfig, damage, enemyTransforms[i], ecb);
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
	public partial struct TimerBombControlJob : IJobEntity
	{
		[ReadOnly] public NativeArray<Entity> enemyEntities;
		[ReadOnly] public NativeArray<LocalTransform> enemyTransforms;
		[ReadOnly] public NativeArray<Enemy> enemyData;

		internal EntityCommandBuffer ecb;

		[ReadOnly] public SpawnVfxConfig spawnVfxConfig;
		[ReadOnly] public SpawnExpPointConfig spawnExpPointConfig;
		[ReadOnly] public DamageIndicatorConfig damageIndicatorConfig;

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
						int damage = (int)bombData.damage;
						float newHp = enemyData[i].hp - bombData.damage;
                        if (newHp>0)
                        {
							ecb.SetComponent(enemyEntities[i], new Enemy { hp = newHp, maxHp = enemyData[i].maxHp, attack = enemyData[i].attack });
						}
						else
						{
							ProcessDeadEnemy(spawnExpPointConfig, spawnVfxConfig, ecb, enemyTransforms[i], enemyEntities[i]);
						}
						SpawnGlyph(damageIndicatorConfig, damage, enemyTransforms[i], ecb);
					}
				}
				ecb.DestroyEntity(dynamiteEntity);
				Entity e = ecb.Instantiate(spawnVfxConfig.vfxBombExplosionEntity);
				ecb.SetComponent(e, new LocalTransform { Position = dynamiteTransform.Position, Rotation = Quaternion.identity, Scale = 1f });
				ecb.AddComponent(e, new VfxTimer { timer = 1.5f });
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static void ProcessDeadEnemy(SpawnExpPointConfig spawnExpPointConfig, SpawnVfxConfig spawnVfxConfig, EntityCommandBuffer ecb, LocalTransform enemyTransform, Entity enemyEntity)
	{
		// spawn exp
		Entity expPointEntity = ecb.Instantiate(spawnExpPointConfig.expPointPrefabEntity);
		ecb.SetComponent(expPointEntity, new LocalTransform
		{
			Position = new float3
			{
				x = enemyTransform.Position.x,
				y = 3,
				z = enemyTransform.Position.z
			},
			Rotation = Quaternion.identity,
			Scale = 1
		});
		/*
		Vector2 expVelocityVec = UnityEngine.Random.insideUnitCircle;
		ecb.SetComponent(expPointEntity, new PhysicsVelocity
		{
			Linear = new float3 { x = expVelocityVec.x * 5, y = 25, z = expVelocityVec.y * 5 },
			Angular = float3.zero
		});
		*/
		ecb.SetComponent(expPointEntity, new PhysicsVelocity { Linear = new float3 { x = 0, y = 25, z = 0 }, Angular = float3.zero });

		ecb.DestroyEntity(enemyEntity);
		Entity enemyExplosionEntty = ecb.Instantiate(spawnVfxConfig.vfxEnemyExplosionEntity);
		ecb.SetComponent(enemyExplosionEntty, new LocalTransform { Position = enemyTransform.Position, Rotation = Quaternion.identity, Scale = 1f });
		ecb.AddComponent(enemyExplosionEntty, new VfxTimer { timer = 1.5f });
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static void SpawnGlyph(DamageIndicatorConfig damageIndicatorConfig, int damage, LocalTransform glyphTransform, EntityCommandBuffer ecb)
	{
		glyphTransform.Position.y += 5;
		glyphTransform.Scale = 10;
		while (damage > 0)
		{
			var digit = damage % 10;
			damage /= 10;
			var glyph = ecb.Instantiate(damageIndicatorConfig.glyphEntity);
			ecb.SetComponent(glyph, glyphTransform);
			glyphTransform.Position.x -= damageIndicatorConfig.glyphWidth;
			ecb.AddComponent(glyph, new GlyphIdFloatOverride { Value = digit });
			//ecb.SetComponent(glyph, new GlyphColorOverride { Color = damageIndicatorConfig. });
			ecb.AddComponent(glyph, new VfxTimer { timer = .7f });
		}
	}
}

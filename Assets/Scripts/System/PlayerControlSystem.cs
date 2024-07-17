using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Unity.VisualScripting;
using Unity.Physics;

public partial struct PlayerControlSystem : ISystem
{
	InputControl inputControl;
	EntityQuery enemyEntityQuery;

	public void OnCreate(ref SystemState state)
	{
		state.RequireForUpdate<InputControl>();
		enemyEntityQuery = state.GetEntityQuery(ComponentType.ReadOnly<Enemy>(), ComponentType.ReadOnly<LocalTransform>());
	}

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

		InputControl inputControl = SystemAPI.GetSingleton<InputControl>();

		foreach (var (localTransform, playerData, timer) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PlayerData>, RefRW<Timer>>().WithAll<Player>()) {
			float3 dir = new float3 { x = inputControl.direction.x, y = 0, z = inputControl.direction.y } * playerData.ValueRW.speed * SystemAPI.Time.DeltaTime;
			if (math.length(dir) > 0.0001f)
			{
				localTransform.ValueRW.Position += dir;

				quaternion targetRotation = quaternion.LookRotationSafe(dir, math.up());
				localTransform.ValueRW.Rotation = targetRotation;
			}

			if (inputControl.changeWeapon)
			{
				playerData.ValueRW.bombType = (BombType)((int)(playerData.ValueRW.bombType + 1) % 3);
				inputControl.changeWeapon = false;
			}

			playerData.ValueRW.cooldown = math.max(0, playerData.ValueRW.cooldown - SystemAPI.Time.DeltaTime);

			if (inputControl.confirm && playerData.ValueRW.cooldown <= 0.01f)
			{
				SpawnBombConfig spawnBombConfig = SystemAPI.GetSingleton<SpawnBombConfig>();
				Entity bombEntity;
				switch (playerData.ValueRW.bombType)
				{
					case BombType.Mine:
						bombEntity = ecb.Instantiate(spawnBombConfig.minePrefabEntity);
						ecb.AddComponent(bombEntity, new BombData
						{
							damage = 10 * playerData.ValueRW.mineDamage,
							radius = 5 * playerData.ValueRW.mineRadius,
						});
						break;
					case BombType.Grenade:
						bombEntity = ecb.Instantiate(spawnBombConfig.grenadePrefabEntity);
						ecb.AddComponent(bombEntity, new BombData
						{
							damage = 10 * playerData.ValueRW.grenadeDamage,
							radius = 5 * playerData.ValueRW.grenadeRadius,
						});
						float3 bombVelocity = math.normalize(math.forward(localTransform.ValueRO.Rotation) + new float3(0, .5f, 0)) * 25;
						ecb.SetComponent(bombEntity, new PhysicsVelocity
						{
							Linear = bombVelocity,
							Angular = float3.zero
						});
						break;
					case BombType.Dynamite:
						bombEntity = ecb.Instantiate(spawnBombConfig.dynamitePrefabEntity);
						ecb.AddComponent(bombEntity, new BombData
						{
							damage = 10 * playerData.ValueRW.dynamiteDamage,
							radius = 5 * playerData.ValueRW.dynamiteRadius,
						});
						break;
					default:
						bombEntity = ecb.Instantiate(spawnBombConfig.minePrefabEntity);
						break;
				}

				ecb.SetComponent(bombEntity, new LocalTransform
				{
					Position = localTransform.ValueRO.Position,
					Rotation = quaternion.identity,
					Scale = 1.0f
				});
				
				playerData.ValueRW.cooldown = playerData.ValueRW.maxCooldown;
			}

			inputControl.confirm = false;
			SystemAPI.SetSingleton(inputControl);

			timer.ValueRW.timer = math.max(0, timer.ValueRW.timer - SystemAPI.Time.DeltaTime);
			if (timer.ValueRO.timer <= .01f)
			{
				float damage = 0;
				foreach (var (enemy, enemyTransform) in SystemAPI.Query<RefRO<Enemy>, RefRO<LocalTransform>>())
				{
					if (math.length(localTransform.ValueRO.Position - enemyTransform.ValueRO.Position) < 3.2f)
					{
						damage += enemy.ValueRO.attack;
					}
				}
				if(damage != 0)
				{
					timer.ValueRW.timer = playerData.ValueRO.invincibleTime;
					playerData.ValueRW.hp -= damage;
				}
			}
		}

		if(!ecb.IsEmpty) ecb.Playback(state.EntityManager);
		ecb.Dispose();

	}

}

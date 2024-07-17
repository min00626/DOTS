using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

[UpdateAfter(typeof(BombControlSystem))]
public partial class HpBarControlSystem : SystemBase
{
	const float t = .98f;
	protected override void OnUpdate()
	{
		SpawnVfxConfig spawnVfxConfig = SystemAPI.GetSingleton<SpawnVfxConfig>();
		SpawnExpPointConfig spawnExpPointConfig = SystemAPI.GetSingleton<SpawnExpPointConfig>();

		EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

		foreach (var (enemyHp, enemyLocalTransform, enemyEntity) in SystemAPI.Query<RefRO<Enemy>, RefRO<LocalTransform>>().WithAll<DamagedEnemyTag>().WithEntityAccess())
		{
			if (enemyHp.ValueRO.hp <= 0)
			{
				Entity expPointEntity = ecb.Instantiate(spawnExpPointConfig.expPointPrefabEntity);
				ecb.SetComponent(expPointEntity, new LocalTransform
				{
					Position = new float3
					{
						x = enemyLocalTransform.ValueRO.Position.x,
						y = 3,
						z = enemyLocalTransform.ValueRO.Position.z
					},
					Rotation = Quaternion.identity,
					Scale = 1
				});
				Vector2 expVelocityVec = UnityEngine.Random.insideUnitCircle;
				ecb.SetComponent(expPointEntity, new PhysicsVelocity
				{
					Linear = new float3 { x = expVelocityVec.x * 5, y = 25, z = expVelocityVec.y * 5 },
					Angular = float3.zero
				});

				ecb.DestroyEntity(enemyEntity);
				Entity e = ecb.Instantiate(spawnVfxConfig.vfxEnemyExplosionEntity);
				ecb.SetComponent(e, new LocalTransform { Position = enemyLocalTransform.ValueRO.Position, Rotation = Quaternion.identity, Scale = 1f });
				ecb.AddComponent(e, new VfxTimer { timer = 1.5f });
				if (SystemAPI.ManagedAPI.HasComponent<EnemyHpBar>(enemyEntity))
				{
					HpBarManager.instance.DisposeHpBarInstance(SystemAPI.ManagedAPI.GetComponent<EnemyHpBar>(enemyEntity).image);
				}



				continue;
			}

			EnemyHpBar enemyHpBar;
			if (!SystemAPI.ManagedAPI.HasComponent<EnemyHpBar>(enemyEntity))
			{
				Image image = HpBarManager.instance.GetHpBarInstance();
				image.fillAmount = math.max(enemyHp.ValueRO.hp / enemyHp.ValueRO.maxHp, image.fillAmount * t);
				enemyHpBar = new EnemyHpBar { image = image };
				ecb.AddComponent(enemyEntity, enemyHpBar);
			}
			else
			{
				enemyHpBar = SystemAPI.ManagedAPI.GetComponent<EnemyHpBar>(enemyEntity);
				enemyHpBar.image.fillAmount = math.max(enemyHp.ValueRO.hp / enemyHp.ValueRO.maxHp, enemyHpBar.image.fillAmount * t);
			}

			Vector3 worldPosition = enemyLocalTransform.ValueRO.Position;
			worldPosition.y += 5;
			Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

			RectTransform rectTransform = enemyHpBar.image.GetComponent<RectTransform>();
			// Convert screen position to canvas space
			Vector2 canvasPosition;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(
				rectTransform.parent as RectTransform, screenPosition, null, out canvasPosition);

			rectTransform.anchoredPosition = canvasPosition;


		}
		ecb.Playback(EntityManager);
		ecb.Dispose();
	}
}

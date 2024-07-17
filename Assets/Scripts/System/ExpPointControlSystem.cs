using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;

public partial struct ExpPointControlSystem : ISystem
{
	EntityQuery playerEntityQuery;

    public void OnCreate(ref SystemState state)
    {
		playerEntityQuery = state.GetEntityQuery(ComponentType.ReadOnly<LocalTransform>(), ComponentType.ReadWrite<PlayerData>());
		state.RequireForUpdate<ExpPoint> ();
		state.RequireForUpdate<Player> ();
    }

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		LocalTransform playerLocalTransform = playerEntityQuery.GetSingleton<LocalTransform>();
		PlayerData playerData = playerEntityQuery.GetSingleton<PlayerData>();

		EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

		float gainedExp = 0;

		foreach (var (expLocalTransform, expPoint, expEntity) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<ExpPoint>>().WithEntityAccess()) { 
			if(math.length(playerLocalTransform.Position - expLocalTransform.ValueRO.Position) < 2f)
			{
				gainedExp += expPoint.ValueRO.exp * playerData.expGainRate;
				ecb.DestroyEntity(expEntity);
			}
		}
		playerData.exp += gainedExp;

		playerEntityQuery.SetSingleton<PlayerData>(playerData);

		ecb.Playback(state.EntityManager);
		ecb.Dispose();
	}
}

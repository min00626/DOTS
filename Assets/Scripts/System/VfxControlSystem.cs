using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;

public partial struct VfxControlSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
		state.RequireForUpdate<Timer>();
    }

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

		foreach(var (vfxTimer, entity) in SystemAPI.Query<RefRW<VfxTimer>>().WithEntityAccess())
		{
			vfxTimer.ValueRW.timer -= SystemAPI.Time.DeltaTime;
			if(vfxTimer.ValueRW.timer <= 0 ) ecb.DestroyEntity(entity);
		}

		ecb.Playback(state.EntityManager);
		ecb.Dispose();
	}
}

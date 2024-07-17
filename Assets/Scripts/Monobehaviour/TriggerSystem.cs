using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Physics;
using UnityEngine;
/*
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
public partial struct EnemyTriggerEventSystem : ISystem
{
	public void OnUpdate(ref SystemState state)
	{
		var triggerJob = new TriggerJob();
		triggerJob.Schedule(SystemAPI.GetSingleton<SimulationSingleton>(), state.Dependency);
	}

	public partial struct TriggerJob : ITriggerEventsJob
	{
		public void Execute(TriggerEvent triggerEvent)
		{
			Debug.Log("Trigger");
		}
	}
}
*/
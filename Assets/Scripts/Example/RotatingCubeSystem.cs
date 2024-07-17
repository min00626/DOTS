using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Burst;

public partial struct RotatingCubeSystem : ISystem
{

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<RotateSpeed>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Use this
        /*
        foreach(var rotatingCubeAspect in SystemAPI.Query<RotatingCubeAspect>())
        {
            rotatingCubeAspect.Roatate(SystemAPI.Time.DeltaTime);
        }
        */
        
        // Or Use this
        RotatingCubeJob rotatingCubeJob = new RotatingCubeJob
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };
        rotatingCubeJob.Schedule();
    }

    [BurstCompile]
    [WithNone(typeof(Player))]
    public partial struct RotatingCubeJob : IJobEntity
    {
        public float deltaTime;

        public void Execute(ref LocalTransform localTransform, in RotateSpeed rotateSpeed)
        {
			localTransform = localTransform.RotateY(rotateSpeed.value * deltaTime);
		}
	}
}

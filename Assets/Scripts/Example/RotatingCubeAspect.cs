using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public readonly partial struct RotatingCubeAspect : IAspect
{
	public readonly RefRW<LocalTransform> localTransform;
	public readonly RefRO<RotateSpeed> rotateSpeed;

	public void Roatate(float deltaTime)
	{
		localTransform.ValueRW = localTransform.ValueRO.RotateY(rotateSpeed.ValueRO.value * deltaTime);

	}
}

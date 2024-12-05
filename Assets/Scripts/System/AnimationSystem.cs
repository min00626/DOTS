using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Collections;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(PresentationSystemGroup), OrderFirst = true)]
public partial struct AnimationSystem : ISystem
{
	public void OnUpdate(ref SystemState state)
	{
		var ecb = new EntityCommandBuffer(Allocator.Temp);

		foreach(var (animatorPrefab, entity) in SystemAPI.Query<AnimatorPrefab>().WithNone<AnimatorReference>().WithEntityAccess())
		{
			GameObject instance = Object.Instantiate(animatorPrefab.animatorPrefab);
			ecb.AddComponent(entity, new AnimatorReference { animator = instance.GetComponent<Animator>() });
		}

		foreach(var (animatorReference, physicsVelocity, localTransform) in SystemAPI.Query<AnimatorReference, RefRO<PhysicsVelocity>, RefRO<LocalTransform>>())
		{
			if(math.length(physicsVelocity.ValueRO.Linear) > 1f)
			{
				animatorReference.animator.SetBool("IsMoving", true);
			}
			else
			{
				animatorReference.animator.SetBool("IsMoving", false);
			}
			animatorReference.animator.transform.position = new Vector3(localTransform.ValueRO.Position.x, 0, localTransform.ValueRO.Position.z);
			animatorReference.animator.transform.rotation = localTransform.ValueRO.Rotation;
		}

		ecb.Playback(state.EntityManager);
		ecb.Dispose();
	}
}

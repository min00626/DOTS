using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class AnimatorPrefabAuthoring : MonoBehaviour
{
	[SerializeField] GameObject animatorPrefab;

	public class Baker : Baker<AnimatorPrefabAuthoring>
	{
		public override void Bake(AnimatorPrefabAuthoring authoring)
		{
			Entity entity = GetEntity(TransformUsageFlags.Dynamic);
			AddComponentObject(entity, new AnimatorPrefab { 
				animatorPrefab = authoring.animatorPrefab
			});
		}
	}
}

public class AnimatorPrefab : IComponentData
{
	public GameObject animatorPrefab;
}

public class AnimatorReference : IComponentData
{
	public Animator animator;
}
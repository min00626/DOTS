using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	readonly float3 offset = new float3 {x=0,y=15, z=-7 };
	//public float3 min;
	//public float3 max;

	private Entity player;
	EntityManager entityManager;

	private void Awake()
	{
		entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
	}

	private void Start()
	{
		EntityQuery query = entityManager.CreateEntityQuery(ComponentType.ReadOnly(typeof(Player)), ComponentType.ReadOnly(typeof(LocalTransform)));
		player = query.GetSingletonEntity();
	}

	void LateUpdate()
	{
		if (entityManager == null || player == null)
		{
			return;
		}

		LocalTransform entPos = entityManager.GetComponentData<LocalTransform>(player);
		transform.position = entPos.Position + offset;
	}
}
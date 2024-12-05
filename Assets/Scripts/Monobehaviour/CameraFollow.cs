using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	readonly float3 offset = new float3 {x=0,y=11.3f, z=-10 };
	//public float3 min;
	//public float3 max;

	private Entity player;
	EntityManager entityManager;
	EntityQuery query;

	private void Awake()
	{
		entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
	}

	private void Start()
	{
		query = entityManager.CreateEntityQuery(ComponentType.ReadOnly(typeof(Player)), ComponentType.ReadOnly(typeof(LocalTransform)));
		try
		{
			player = query.GetSingletonEntity();
		}
		catch { }
	}

	void LateUpdate()
	{
		if (entityManager == null)
		{
			return;
		}
		if(player == null)
		{
			try
			{
				player = query.GetSingletonEntity();
			}
			catch { return; }
		}

		LocalTransform entPos = entityManager.GetComponentData<LocalTransform>(player);
		transform.position = entPos.Position + offset;
	}
}
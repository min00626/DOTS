using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class SpawnEnemyManager : MonoBehaviour
{
	EntityManager entityManager;
	SpawnEnemyConfig spawnEnemyConfig;

	private void Awake()
	{
		entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
	}

	private void Start()
	{
		EntityQuery query = entityManager.CreateEntityQuery(ComponentType.ReadWrite(typeof(SpawnEnemyConfig)));
		spawnEnemyConfig = entityManager.GetComponentData<SpawnEnemyConfig>(query.GetSingletonEntity());
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}

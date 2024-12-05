using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;

public class InputControlManager : MonoBehaviour
{
    EntityManager entityManager;
	Entity inputControlEntity;

	bool lastConfirmValue = false;
	bool lastChangeWeaponValue = false;


	void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

		EntityArchetype archetype = entityManager.CreateArchetype(typeof(InputControl));
		inputControlEntity = entityManager.CreateEntity(archetype);

		// 기본값 설정 (예: 모든 값 초기화)
		entityManager.SetComponentData(inputControlEntity, new InputControl
		{
			direction = new float2 { x=0, y=0},
			confirm = false
		});
	}

    void Update()
    {
		if (entityManager.HasComponent<InputControl>(inputControlEntity))
		{
			InputControl inputControl = entityManager.GetComponentData<InputControl>(inputControlEntity);
			float2 dir = new float2 { x = Input.GetAxisRaw("Horizontal"), y = Input.GetAxisRaw("Vertical") };
			math.normalizesafe(dir);

			inputControl.direction = dir;
			bool currentConfirmValue = Input.GetButton("Submit");
			if (!lastConfirmValue && currentConfirmValue)
				inputControl.confirm = true;
			lastConfirmValue = currentConfirmValue;

			bool currentChangeWeaponValue = Input.GetButton("ChangeWeapon");
			if (!lastChangeWeaponValue && currentChangeWeaponValue)
				inputControl.changeWeapon = true;
			lastChangeWeaponValue = currentChangeWeaponValue;

			entityManager.SetComponentData(inputControlEntity, inputControl);
		}
	}
}

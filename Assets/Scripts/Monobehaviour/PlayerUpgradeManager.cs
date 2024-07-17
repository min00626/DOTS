using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class PlayerUpgradeManager : MonoBehaviour
{
    public static PlayerUpgradeManager instance;

	EntityManager entityManager;

	private void Awake()
	{
		instance = this;
		entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
	}


}

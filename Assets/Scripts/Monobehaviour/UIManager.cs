using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEngine.UI;
using Unity.Mathematics;
using TMPro;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
	public static UIManager instance;

	[SerializeField] EventSystem eventSystem;

	[SerializeField] Image playerHpBar;
	[SerializeField] Image playerExpBar;

	[SerializeField] CanvasGroup playerUpgradePanel;
	List<Button> playerUpgradeButtons = new List<Button>();
	List<TMP_Text> playerUpgradeButtonTexts = new List<TMP_Text>();

	EntityManager entityManager;
	EntityQuery entityQuery;

	private Dictionary<int, System.Action> upgradeActions;
	private Dictionary<int, string> upgradeTexts;

	private void Awake()
	{
		instance = this;

		entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
		entityQuery = entityManager.CreateEntityQuery(ComponentType.ReadWrite<PlayerData>());

		InitializeUpgradeOptions();

	}

	private void InitializeUpgradeOptions()
	{
		upgradeActions = new Dictionary<int, System.Action>
		{
			{ 0, IncreaseMineDamage },
			{ 1, IncreaseMineRadius },
			{ 2, IncreaseDynamiteDamage },
			{ 3, IncreaseDynamiteRadius },
			{ 4, IncreaseGrenadeDamage },
			{ 5, IncreaseGrenadeRadius },
			{ 6, IncreasePlayerSpeed },
			{ 7, IncreasePlayerExpGain }
		};

		upgradeTexts = new Dictionary<int, string>
		{
			{ 0, "Increase Mine Damage" },
			{ 1, "Increase Mine Radius" },
			{ 2, "Increase Dynamite Damage" },
			{ 3, "Increase Dynamite Radius" },
			{ 4, "Increase Grenade Damage" },
			{ 5, "Increase Grenade Radius" },
			{ 6, "Increase Player Speed" },
			{ 7, "Increase Player Exp Gain" }
		};
	}
	private void Start()
	{
		playerUpgradePanel.alpha = 0;
		playerUpgradePanel.interactable = false;

		playerUpgradePanel.GetComponentsInChildren<Button>(playerUpgradeButtons);
		foreach (var button in playerUpgradeButtons) { 
			playerUpgradeButtonTexts.Add(button.GetComponentInChildren<TMP_Text>());
		}
	}

	private void Update()
	{
		if(entityQuery.TryGetSingleton<PlayerData>(out PlayerData playerData))
		{
			playerHpBar.fillAmount += (playerData.hp / playerData.maxHp - playerHpBar.fillAmount) * Time.deltaTime * 3;
			playerExpBar.fillAmount += (playerData.exp / playerData.maxExp - playerExpBar.fillAmount) * Time.deltaTime * 3;

			while (playerData.exp >= playerData.maxExp) {
				playerData.exp -= playerData.maxExp;
				OpenPlayerUpgradePanel();
			}
			entityManager.SetComponentData(entityQuery.GetSingletonEntity(), playerData);
		}

	}

	public void OpenPlayerUpgradePanel()
	{
		eventSystem.SetSelectedGameObject(playerUpgradeButtons[0].gameObject);
		Time.timeScale = 0;
		playerUpgradePanel.alpha = 1f;
		playerUpgradePanel.interactable = true;

		int[] uniqueNumbers = RandomIntegers.GetUniqueRandomNumbers(3, 0, 7);
		for (int i = 0; i < 3; i++)
		{
			int upgradeIndex = uniqueNumbers[i];
			playerUpgradeButtons[i].onClick.RemoveAllListeners();

			if (upgradeActions.TryGetValue(upgradeIndex, out var action) && upgradeTexts.TryGetValue(upgradeIndex, out var text))
			{
				playerUpgradeButtons[i].onClick.AddListener(action.Invoke);
				playerUpgradeButtonTexts[i].text = text;
			}
		}
	}

	const float bombDamageIncreaseRate = .3f;
	const float bombRadiusIncreaseRate = .3f;
	const float playerSpeedIncreaseRate = .2f;
	const float playerExpGainIncreaseRate = .2f;

	void ClosePlayerUpgradePanel()
	{
		Time.timeScale = 1;
		playerUpgradePanel.alpha = 0f;
		playerUpgradePanel.interactable = false;
		eventSystem.SetSelectedGameObject(playerUpgradeButtons[0].gameObject);
	}

	public void IncreaseMineDamage()
	{
		if (entityQuery.TryGetSingleton<PlayerData>(out PlayerData playerData))
		{
			playerData.mineDamage += bombDamageIncreaseRate;
			entityQuery.SetSingleton<PlayerData>(playerData);
		}
		ClosePlayerUpgradePanel();
	}

	public void IncreaseMineRadius()
	{
		if (entityQuery.TryGetSingleton<PlayerData>(out PlayerData playerData))
		{
			playerData.mineRadius += bombRadiusIncreaseRate;
			entityQuery.SetSingleton<PlayerData>(playerData);
		}
		ClosePlayerUpgradePanel();
	}

	public void IncreaseDynamiteDamage()
	{
		if (entityQuery.TryGetSingleton<PlayerData>(out PlayerData playerData))
		{
			playerData.dynamiteDamage += bombDamageIncreaseRate;
			entityQuery.SetSingleton<PlayerData>(playerData);
		}
		ClosePlayerUpgradePanel();
	}

	public void IncreaseDynamiteRadius()
	{
		if (entityQuery.TryGetSingleton<PlayerData>(out PlayerData playerData))
		{
			playerData.dynamiteRadius += bombRadiusIncreaseRate;
			entityQuery.SetSingleton<PlayerData>(playerData);
		}
		ClosePlayerUpgradePanel();
	}

	public void IncreaseGrenadeDamage()
	{
		if (entityQuery.TryGetSingleton<PlayerData>(out PlayerData playerData))
		{
			playerData.grenadeDamage += bombDamageIncreaseRate;
			entityQuery.SetSingleton<PlayerData>(playerData);
		}
		ClosePlayerUpgradePanel();
	}

	public void IncreaseGrenadeRadius()
	{
		if (entityQuery.TryGetSingleton<PlayerData>(out PlayerData playerData))
		{
			playerData.grenadeRadius += bombRadiusIncreaseRate;
			entityQuery.SetSingleton<PlayerData>(playerData);
		}
		ClosePlayerUpgradePanel();
	}

	public void IncreasePlayerSpeed()
	{
		if (entityQuery.TryGetSingleton<PlayerData>(out PlayerData playerData))
		{
			playerData.speed += playerSpeedIncreaseRate;
			entityQuery.SetSingleton<PlayerData>(playerData);
		}
		ClosePlayerUpgradePanel();
	}

	public void IncreasePlayerExpGain()
	{
		if (entityQuery.TryGetSingleton<PlayerData>(out PlayerData playerData))
		{
			playerData.expGainRate += playerExpGainIncreaseRate;
			entityQuery.SetSingleton<PlayerData>(playerData);
		}
		ClosePlayerUpgradePanel();
	}
}

using UnityEngine;

public class EnemyController : MonoBehaviour
{
	public float moveSpeed = 5f; // 적의 이동 속도

	private Transform playerTransform;

	void Start()
	{
		// PlayerController.instance.transform을 통해 플레이어의 Transform을 가져옴
		if (PlayerController.instance != null)
		{
			playerTransform = PlayerController.instance.transform;
		}
		else
		{
			Debug.LogError("PlayerController instance is not set.");
		}
	}

	void Update()
	{
		if (playerTransform != null)
		{
			// 플레이어의 위치를 향하도록 적의 방향을 조정
			Vector3 direction = (playerTransform.position - transform.position).normalized;
			direction.y = 0;
			Quaternion lookRotation = Quaternion.LookRotation(direction);
			transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * moveSpeed);

			// 플레이어 방향으로 적을 이동
			transform.position += direction * moveSpeed * Time.deltaTime;
		}
	}
}

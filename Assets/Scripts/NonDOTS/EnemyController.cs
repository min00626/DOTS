using UnityEngine;

public class EnemyController : MonoBehaviour
{
	public float moveSpeed = 5f; // ���� �̵� �ӵ�

	private Transform playerTransform;

	void Start()
	{
		// PlayerController.instance.transform�� ���� �÷��̾��� Transform�� ������
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
			// �÷��̾��� ��ġ�� ���ϵ��� ���� ������ ����
			Vector3 direction = (playerTransform.position - transform.position).normalized;
			direction.y = 0;
			Quaternion lookRotation = Quaternion.LookRotation(direction);
			transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * moveSpeed);

			// �÷��̾� �������� ���� �̵�
			transform.position += direction * moveSpeed * Time.deltaTime;
		}
	}
}

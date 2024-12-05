using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;

    [SerializeField] int count;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 random = Random.insideUnitCircle * 50;
            GameObject enemy = Instantiate(enemyPrefab, PlayerController.instance.transform.position + new Vector3(random.x, 0, random.y), Quaternion.identity);
            enemy.AddComponent<EnemyController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

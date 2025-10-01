using System;
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //Needs instance of pool and script in prefab that does the activating/disabling 
    public EnemyPool enemyPool;
    //Needs a function Reset() (e.g. sprite's animation, any states except it's)
    private EnemyMovement enemyInstance;

    public float spawnInterval = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnEnemyCoroutine());
    }

    IEnumerator SpawnEnemyCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            SpawnObject();
        }
    }
    // Update is called once per frame
    void Update()
    {

    }

    void SpawnObject()
    {
        GameObject enemy = enemyPool.GetPoolObject();
        if (enemy != null)
        {
            enemy.SetActive(true);
            enemyInstance = enemy.GetComponent<EnemyMovement>();
            enemyInstance.Reset();
            enemy.transform.position = transform.position;
        }
    }
}

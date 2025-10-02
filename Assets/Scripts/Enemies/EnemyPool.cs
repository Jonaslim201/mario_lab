using System.Collections.Generic;
using UnityEngine;

//Needs the prefab of the object youre pooling

public class EnemyPool : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;
    private int MaxPoolSzie = 20;

    private List<GameObject> pool;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pool = new List<GameObject>();
    }

    // Update is called once per  frame
    void Update()
    {

    }

    public GameObject GetPoolObject()
    {
        Debug.Log(pool.Count);
        //Look for in active object
        foreach (GameObject obj in pool)
        {
            //Check for inactive object
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        //No inactive object create object and add to pool
        if (pool.Count < MaxPoolSzie)
        {
            return createObject();
        }
        //Limit reached
        return null;
    }

    private GameObject createObject()
    {
        GameObject obj = Instantiate(enemyPrefab, transform);
        obj.SetActive(false);
        pool.Add(obj);
        return obj;
    }

    public void Reset()
    {
        foreach (GameObject obj in pool)
        {
            Destroy(obj);
        }
        pool.Clear();
    }
}

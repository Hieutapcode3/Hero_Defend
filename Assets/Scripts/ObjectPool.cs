using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }
    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;
    public static ObjectPool instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            if (poolDictionary.ContainsKey(pool.tag))
            {
                Debug.LogWarning("Duplicate tag found: " + pool.tag + ". Skipping this pool.");
                continue;
            }

            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.transform.SetParent(transform);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (poolDictionary == null || !poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist or poolDictionary is not initialized.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = Quaternion.Euler(0, 0, 0);

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}

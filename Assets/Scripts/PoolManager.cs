using UnityEngine;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;
    [System.Serializable]
    public class Pool
    {
        public string parentName;
        public GameObject prefab;
        public int poolSize;
        public List<GameObject> pooledObjects;
    }

    [SerializeField] private List<Pool> _pools;
    [SerializeField] private Dictionary<string, Pool> _poolsDictionary = new Dictionary<string, Pool>();

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        GameObject obj;
        foreach (Pool pool in _pools)
        {
            GameObject parent = new GameObject(pool.parentName);

            for (int i = 0; i < pool.poolSize; i++)
            {
                obj = Instantiate(pool.prefab);
                obj.transform.SetParent(parent.transform);
                obj.SetActive(false);
                pool.pooledObjects.Add(obj);
            }
        }

        foreach (Pool pool in _pools)
        {
            _poolsDictionary.Add(pool.parentName, pool);
        }
    }

    public GameObject GetPooledObject(string poolName, Vector3 position, Quaternion rotation)
    {
        for (int i = 0; i < _poolsDictionary[poolName].poolSize; i++)
        {
            if(!_poolsDictionary[poolName].pooledObjects[i].activeInHierarchy)
            {
                GameObject objectToSpawn;
                objectToSpawn = _poolsDictionary[poolName].pooledObjects[i];
                objectToSpawn.transform.position = position;
                objectToSpawn.transform.rotation = rotation;
                return objectToSpawn;
            }            
        }

        Debug.Log("No hay nada");
        return null;
    }
}

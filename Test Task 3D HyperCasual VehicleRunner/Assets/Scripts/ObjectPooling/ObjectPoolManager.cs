using UnityEngine;
using System.Collections.Generic;

public class ObjectPoolManager : MonoBehaviour
{
    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();
    public void CreatePool(string tag, GameObject prefab, int size)
    {
        if (poolDictionary.ContainsKey(tag)) return;
        Queue<GameObject> objectsPool = new Queue<GameObject> ();
        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            objectsPool.Enqueue(obj);
        }
        poolDictionary.Add(tag, objectsPool);
    }
    
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if(!poolDictionary.ContainsKey(tag) || poolDictionary[tag].Count == 0)
        {
            Debug.LogWarning($"Pull with {tag} is empty or null");
            return null;
        }
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        return objectToSpawn;
    }

    public void ReturnToPool(string tag, GameObject objectToReturn)
    {
        if (!poolDictionary.ContainsKey(tag)) return;
        objectToReturn.SetActive(false);
        poolDictionary[tag].Enqueue(objectToReturn);
    }
}

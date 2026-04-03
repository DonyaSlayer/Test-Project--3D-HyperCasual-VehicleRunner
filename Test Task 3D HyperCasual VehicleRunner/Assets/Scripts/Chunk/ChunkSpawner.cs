using UnityEngine;
using System.Collections.Generic;

public class ChunkSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] _chunkPrefabs;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _chunkLength;
    [SerializeField] private int _chunksOnScreen;

    private List<GameObject> _activeChunks = new List<GameObject>();
    private Dictionary<int, Queue<GameObject>> _inactiveChunks = new Dictionary<int, Queue<GameObject>>();
    private Dictionary<GameObject, int> _chunkIdMap = new Dictionary<GameObject, int>();
    private float _spawnZ = 0f;

    private void Start()
    {
        for (int i = 0; i < _chunkPrefabs.Length; i++)
        {
            _inactiveChunks[i] = new Queue<GameObject>();
        }

        for (int i = 0; i < _chunksOnScreen; i++)
        {
            SpawnRandomChunk();
        }
    }

    private void Update()
    {
        if (_playerTransform.position.z - _chunkLength > _activeChunks[0].transform.position.z)
        {
            RecycleChunk();
        }
    }

    private void SpawnRandomChunk()
    {
        int randomIndex = Random.Range(0, _chunkPrefabs.Length);
        GameObject newChunk;
        if (_inactiveChunks[randomIndex].Count > 0)
        {
            newChunk = _inactiveChunks[randomIndex].Dequeue();
            newChunk.SetActive(true);
            newChunk.transform.position = new Vector3 (0,0,_spawnZ);
        }
        else
        {
            newChunk = Instantiate(_chunkPrefabs[randomIndex], new Vector3(0,0,_spawnZ), transform.rotation);
            newChunk.transform.SetParent(transform);
            _chunkIdMap[newChunk] = randomIndex;
        }
        _activeChunks.Add(newChunk);
        _spawnZ += _chunkLength;
    }

    private void RecycleChunk()
    {
        GameObject oldChunk = _activeChunks[0];
        _activeChunks.RemoveAt(0);
        oldChunk.SetActive(false);
        int chunkTypeIndex = _chunkIdMap[oldChunk];
        _inactiveChunks[chunkTypeIndex].Enqueue(oldChunk);
        SpawnRandomChunk();
    }
}

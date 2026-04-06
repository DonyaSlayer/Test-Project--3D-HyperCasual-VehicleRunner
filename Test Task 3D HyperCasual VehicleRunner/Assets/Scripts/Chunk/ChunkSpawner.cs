using UnityEngine;
using System.Collections.Generic;

public class ChunkSpawner : MonoBehaviour
{
    [Header ("Level Settings")]
    [SerializeField] private GameObject _startFinishChunkPrefab;
    [SerializeField] private int _lvlLength;

    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] _chunkPrefabs;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _chunkLength;
    [SerializeField] private int _chunksOnScreen;
    [SerializeField] private int _copiesPerPrefab;
    private List<GameObject> _activeChunks = new List<GameObject>();
    private Dictionary<int, Queue<GameObject>> _inactiveChunks = new Dictionary<int, Queue<GameObject>>();
    private Dictionary<GameObject, int> _chunkIdMap = new Dictionary<GameObject, int>();
    private float _spawnZ = 0f;
    private int _spawnedChunksCount;
    private bool _isLvlFinished = false;   
    

    private void Start()
    {
        SpwanStartChunk();
        int maxNeedPerType = _chunksOnScreen +1;
        for (int i = 0; i < _chunkPrefabs.Length; i++)
        {
            _inactiveChunks[i] = new Queue<GameObject>();
            for (int j = 0; j < maxNeedPerType; j++)
            {
                GameObject chunk = Instantiate(_chunkPrefabs[i], Vector3.zero, transform.rotation);
                chunk.transform.SetParent(transform);
                chunk.SetActive(false);
                _chunkIdMap[chunk] = i;
                _inactiveChunks[i].Enqueue(chunk);
            }
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

    private void SpwanStartChunk()
    {
        GameObject startChunk = Instantiate(_startFinishChunkPrefab, new Vector3(0,0, _spawnZ), Quaternion.identity);
        startChunk.transform.SetParent(transform);
        _activeChunks.Add(startChunk);
        _spawnZ += _chunkLength;
    }

    private void SpawnRandomChunk()
    {
        if (_isLvlFinished) return;
        if (_spawnedChunksCount >= _lvlLength)
        {
            SpawnFinishChunk();
            return;
        }
        int randomIndex = Random.Range(0, _chunkPrefabs.Length);
        GameObject newChunk = _inactiveChunks[randomIndex].Dequeue();
        newChunk.SetActive(true);
        newChunk.transform.position = new Vector3(0, 0, _spawnZ);
        _activeChunks.Add(newChunk);
        _spawnZ += _chunkLength;
        _spawnedChunksCount++;
    }
    private void SpawnFinishChunk()
    {
        GameObject finishChunk = Instantiate(_startFinishChunkPrefab, new Vector3(0, 0, _spawnZ), Quaternion.Euler(0,180,0));
        finishChunk.transform.SetParent(transform);
        _activeChunks.Add(finishChunk);
        _spawnZ += _chunkLength;
        _isLvlFinished = true;
    }
    private void RecycleChunk()
    {
        GameObject oldChunk = _activeChunks[0];
        _activeChunks.RemoveAt(0);
        oldChunk.SetActive(false);
        if(_chunkIdMap.TryGetValue(oldChunk, out int chunkTypeIndex))
        {
            _inactiveChunks[chunkTypeIndex].Enqueue(oldChunk);
        }
        else
        {
            Destroy(oldChunk);
        }
        if (!_isLvlFinished)
        {
            SpawnRandomChunk();
        }
    }
}

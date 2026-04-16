using UnityEngine;
using System.Collections.Generic;
using Zenject;

public class ChunkSpawner : MonoBehaviour
{
    [Header ("Level Settings")]
    [SerializeField] private GameObject _startFinishChunkPrefab;
    [SerializeField] private int _lvlLength;
    [SerializeField] private EnemySpawner _enemySpawner;

    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] _chunkPrefabs;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _chunkLength;
    [SerializeField] private int _chunksOnScreen;
    [SerializeField] private int _copiesPerPrefab;
    [SerializeField] private UILevelProgress _lvlProgressUI;

    private List<GameObject> _activeChunks = new List<GameObject>();
    private Dictionary<int, Queue<GameObject>> _inactiveChunks = new Dictionary<int, Queue<GameObject>>();
    private Dictionary<GameObject, int> _chunkIdMap = new Dictionary<GameObject, int>();
    private GameStateController _gameStateController;
    private float _spawnZ = 0f;
    private int _spawnedChunksCount;
    private bool _isLvlFinished = false;

    [Inject]
    public void Construct(GameStateController gameStateController)
    {
        _gameStateController = gameStateController;
    }

    private void Start()
    {
        if (_gameStateController != null)
        {
            _gameStateController.OnGameStateChanged += HandleStateChange;
        }
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
        BuildLevel();
    }

    private void BuildLevel()
    {
        SpawnStartChunk();
        for (int i = 0; i < _chunksOnScreen; i++)
        {
            SpawnRandomChunk();
        }

        if (_lvlProgressUI != null && _playerTransform != null)
        {
            float startZ = _playerTransform.position.z;
            float totalDistance = (_lvlLength + 1) * _chunkLength;
            float endZ = startZ + totalDistance;
            _lvlProgressUI.Initialize(_playerTransform, startZ, endZ);
        }
    }

    private void Update()
    {
        if (_activeChunks.Count == 0) return;
        if (_playerTransform.position.z - _chunkLength > _activeChunks[0].transform.position.z)
        {
            RecycleChunk();
        }
    }

    private void SpawnStartChunk()
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
        if(_enemySpawner != null)
        {
            _enemySpawner.SpawnEnemiesOnChunk(newChunk.transform.position, _spawnedChunksCount);
        }
        _activeChunks.Add(newChunk);
        _spawnZ += _chunkLength;
        _spawnedChunksCount++;
    }
    private void SpawnFinishChunk()
    {
        GameObject finishChunk = Instantiate(_startFinishChunkPrefab, new Vector3(0, 0, _spawnZ), Quaternion.Euler(0,180,0));
        finishChunk.transform.SetParent(transform);
        FinishLine finishLine = finishChunk.GetComponentInChildren<FinishLine>();
        if(finishLine != null) finishLine.ActivateOnFinishChunk(_gameStateController);
        _activeChunks.Add(finishChunk);
        _spawnZ += _chunkLength;
        _isLvlFinished = true;
    }
    private void RecycleChunk()
    {
        GameObject oldChunk = _activeChunks[0];
        _activeChunks.RemoveAt(0);
       ReturnChunkToPool(oldChunk);
        if (!_isLvlFinished)
        {
            SpawnRandomChunk();
        }
    }

    private void ReturnChunkToPool(GameObject chunk)
    {
        chunk.SetActive(false);
        if (_chunkIdMap.TryGetValue(chunk, out int chunkTypeIndex))
        {
            _inactiveChunks[chunkTypeIndex].Enqueue(chunk);
        }
        else
        {
            Destroy(chunk);
        }
    }

    private void HandleStateChange(GameState state)
    {
        if (state == GameState.Menu)
        {
            ResetLevel();
        }
    }
    private void ResetLevel()
    {
        foreach (GameObject chunk in _activeChunks)
        {
            ReturnChunkToPool(chunk);
        }
        _activeChunks.Clear();
        _spawnZ = 0;
        _spawnedChunksCount = 0;
        _isLvlFinished = false;
        BuildLevel();
    }

    private void OnDestroy()
    {
        if(_gameStateController != null)
        {
            _gameStateController.OnGameStateChanged -= HandleStateChange;
        }
    }
}

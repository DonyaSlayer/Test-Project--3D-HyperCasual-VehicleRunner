using UnityEngine;
using Zenject;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private AnimationCurve _difficultyCurve;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyVfxPrefab;
    [SerializeField] private GameObject _damageTextPrefab;
    [SerializeField] private float _spawnWidth; 
    [SerializeField] private float _spawnLengthOffset;
    [SerializeField] private GameObject _coinTextPrefab;

    private ObjectPoolManager _poolManager;
    private GameStateController _gameStateController;
    private CoinManager _coinManager;

    [Inject]
    public void Construct (ObjectPoolManager poolManager, GameStateController gameStateController, CoinManager coinManager)
    {
        _poolManager = poolManager;
        _gameStateController = gameStateController;
        _coinManager = coinManager;
    }

    private void Start()
    {
        if (_enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab isn't assigned into EnemySpawner");
            return;
        }
        _poolManager.CreatePool(GameConstants.Pools.Enemy, _enemyPrefab, 50);
        if(_enemyVfxPrefab != null)
        {
            _poolManager.CreatePool(GameConstants.Pools.EnemyVFX, _enemyVfxPrefab, 20);
        }
        if (_damageTextPrefab != null)
        {
            _poolManager.CreatePool(GameConstants.Pools.DamageText, _damageTextPrefab, 30);
        }
        if (_coinTextPrefab != null)
        {
            _poolManager.CreatePool(GameConstants.Pools.CoinText, _coinTextPrefab, 20);
        }
    }
    public void SpawnEnemiesOnChunk(Vector3 chunkCenter, int currentChunkIndex)
    {
        int enemiesToSpawn = Mathf.RoundToInt(_difficultyCurve.Evaluate(currentChunkIndex));
        for(int i = 0; i < enemiesToSpawn; i++)
        {
            Vector3 randomPos = GetRandomPositionOnChunk(chunkCenter);
            GameObject enemyObj = _poolManager.SpawnFromPool(GameConstants.Pools.Enemy,randomPos, Quaternion.Euler(0,180,0));
            if (enemyObj != null)
            {
                Enemy enemy = enemyObj.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.Initialize(_playerTransform, _poolManager, _gameStateController, _coinManager);
                }
            }
        }
    }

    private Vector3 GetRandomPositionOnChunk(Vector3 chunkCenter)
    {
        float randomX = Random.Range(-_spawnWidth / 2f, _spawnWidth / 2f);
        float randomZ = Random.Range(-_spawnLengthOffset, _spawnLengthOffset);
        return new Vector3(chunkCenter.x + randomX, chunkCenter.y, chunkCenter.z + randomZ);
    }
}

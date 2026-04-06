using UnityEngine;
using Zenject;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private string _enemyPoolTag = "Enemy";
    [SerializeField] private AnimationCurve _difficultyCurve;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private float _spawnWidth; 
    [SerializeField] private float _spawnLengthOffset;

    private ObjectPoolManager _poolManager;

    [Inject]
    public void Construct (ObjectPoolManager poolManager)
    {
        _poolManager = poolManager;
    }

    private void Start()
    {
        if (_enemyPrefab == null)
        {
            Debug.LogError("Enemy prefab isn't assigned into EnemySpawner");
            return;
        }
        _poolManager.CreatePool(_enemyPoolTag, _enemyPrefab, 50);
    }
    public void SpawnEnemiesOnChunk(Vector3 chunkCenter, int currentChunkIndex)
    {
        int enemiesToSpawn = Mathf.RoundToInt(_difficultyCurve.Evaluate(currentChunkIndex));
        for(int i = 0; i < enemiesToSpawn; i++)
        {
            Vector3 randomPos = GetRandomPositionOnChunk(chunkCenter);
            GameObject enemyObj = _poolManager.SpawnFromPool(_enemyPoolTag,randomPos, Quaternion.Euler(0,180,0));
            if (enemyObj != null)
            {
                Enemy enemy = enemyObj.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.Initiialize(_playerTransform, _poolManager);
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

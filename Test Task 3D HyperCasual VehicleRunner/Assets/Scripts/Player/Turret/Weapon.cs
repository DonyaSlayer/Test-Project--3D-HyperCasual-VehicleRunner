using UnityEngine;
using Zenject;

public class Weapon : MonoBehaviour
{
    [SerializeField] private float fireRate;
    [SerializeField] private GameObject bulletPrefab;
    private float _nextFireTime;
    private InputHandler _inputHandler;
    private ObjectPoolManager _poolManager;
    private bool _isShooting;

    [Inject]
    public void Construct(InputHandler inputHandler, ObjectPoolManager poolManager)
    {
        _inputHandler = inputHandler;
        _poolManager = poolManager;
    }

    private void Start()
    {
        _inputHandler.OnPositionHandlerChanged += (pos) => _isShooting = true;
        _poolManager.CreatePool("Bullet", bulletPrefab, 30);
    }

    private void Update()
    {
        if (_isShooting && Time.time >= _nextFireTime)
        {
            Fire();
            _nextFireTime = Time.time + fireRate;
        }
        _isShooting=false;
    }

    private void Fire()
    {
        GameObject bulletObj = _poolManager.SpawnFromPool("Bullet", transform.position, transform.rotation);
        if (bulletObj != null)
        {
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            if (bullet != null) bullet.Init(_poolManager);
        }
    }
}

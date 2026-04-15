using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float _bulletSpeed;
    private float _bulletLifetime;
    private int _bulletDmg;
    private float _timer;
    private TrailRenderer _trail;
    private ObjectPoolManager _poolManager;

    public void Init(ObjectPoolManager poolManager, float speed, float lifetime, int damage)
    {
        _poolManager = poolManager;
        _bulletSpeed = speed;
        _bulletLifetime = lifetime;
        _bulletDmg = damage;
        _trail = GetComponent<TrailRenderer>();
        _timer = _bulletLifetime;
        if (_trail != null) _trail.Clear();
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * _bulletSpeed * Time.deltaTime);
        _timer -= Time.deltaTime;
        if(_timer <= 0)
        {
            _poolManager.ReturnToPool(GameConstants.Pools.Bullet, gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameConstants.Tags.Enemy))
        {
            IDamageable target = other.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(_bulletDmg);
            }
            _poolManager.ReturnToPool(GameConstants.Pools.Bullet, gameObject);
        }
    }
}

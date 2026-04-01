using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float bulletLifetime;
    private float _timer;
    private TrailRenderer _trail;
    private ObjectPoolManager _poolManager;

    public void Init(ObjectPoolManager poolManager)
    {
        _poolManager = poolManager;
        _trail = GetComponent<TrailRenderer>();
    }

    private void OnEnable()
    {
        _timer = bulletLifetime;
        if(_trail != null) _trail.Clear();
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * bulletSpeed * Time.deltaTime);
        _timer -= Time.deltaTime;
        if(_timer <= 0)
        {
            _poolManager.ReturnToPool("Bullet", gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        _poolManager.ReturnToPool("Bullet", gameObject);
    }
}

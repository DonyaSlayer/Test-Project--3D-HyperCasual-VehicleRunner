using UnityEngine;
using Zenject;
using DG.Tweening;

public class Weapon : MonoBehaviour
{
    [SerializeField] private float fireRate;
    [SerializeField] private GameObject bulletPrefab;

    [Header("Visual")]
    [SerializeField] private Transform _turretMesh;
    [SerializeField] private ParticleSystem _shootFlash;
    [SerializeField] private float _recoilStrength = 0.2f;
    [SerializeField] private float _recoilDuration = 0.1f;
    private Vector3 _originalLocalPos;
    private float _nextFireTime;
    private InputHandler _inputHandler;
    private ObjectPoolManager _poolManager;
    private GameStateController _gameStateController;
    private bool _isShooting;
    private bool _canShoot = false;

    [Inject]
    public void Construct(InputHandler inputHandler, ObjectPoolManager poolManager, GameStateController gameStateController)
    {
        _inputHandler = inputHandler;
        _poolManager = poolManager;
        _gameStateController = gameStateController;
    }

    private void Start()
    {
        _inputHandler.OnPositionHandlerChanged += HandleInput;
        _gameStateController.OnGameStateChanged += HandleGameStateChange;
        _poolManager.CreatePool("Bullet", bulletPrefab, 30);
        if(_turretMesh != null)
        {
            _originalLocalPos = _turretMesh.localPosition;
        }
    }

    private void Update()
    {
        if (!_canShoot) return;
        if (_isShooting && Time.time >= _nextFireTime)
        {
            Fire();
            _nextFireTime = Time.time + fireRate;
        }
        _isShooting=false;
    }
    private void HandleGameStateChange(GameState newState)
    {
        _canShoot = (newState == GameState.Playing);
        if (_canShoot)
        {
            _nextFireTime = Time.time + 0.2f;
        }
        if (!_canShoot) _isShooting = false;
    }
    private void HandleInput (Vector2 pos)
    {
        _isShooting = true;
    }

    private void Fire()
    {
        GameObject bulletObj = _poolManager.SpawnFromPool("Bullet", transform.position, transform.rotation);
        if (bulletObj != null)
        {
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            if (bullet != null) bullet.Init(_poolManager);
        }
        PlayShootFeedback();
    }

    private void PlayShootFeedback()
    {
        if(_shootFlash != null)
        {
            _shootFlash.Play();
        }
        if(_turretMesh != null)
        {
            _turretMesh.DOComplete();
            _turretMesh.localPosition = _originalLocalPos;
            _turretMesh.DOPunchPosition(new Vector3(0, 0, -_recoilStrength), _recoilDuration, vibrato: 1, elasticity: 0);
        }
    }

    private void OnDestroy()
    {
        if(_inputHandler != null)
        {
            _inputHandler.OnPositionHandlerChanged -=HandleInput;
        }
        if (_gameStateController != null)
        {
            _gameStateController.OnGameStateChanged -= HandleGameStateChange;
        }
    }
}

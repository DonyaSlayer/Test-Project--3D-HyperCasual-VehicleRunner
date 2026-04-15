using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int _maxHp;
    [SerializeField] private DamageFeedback _damageFeedback;
    [SerializeField] private CinemachineImpulseSource _impulseSource;

    [Header("UI")]
    [SerializeField] private UIWorldHealthBar _healthBar;
    [SerializeField] private Transform _textSpawnPoint;

    [Header("Death Settings")]
    [SerializeField] private GameObject _deadCarPrefab;
    [SerializeField] private GameObject _explosionVfxPrefab;
    private int _currentHP;
    private GameStateController _gameStateController;
    private ObjectPoolManager _poolManager;
    private bool _driftTextToSide;
    private GameObject _spawnedDeadCar;
    private bool _isDead = false;

    [Inject]
    public void Construct(GameStateController gameStateController, ObjectPoolManager poolManager)
    {
        _gameStateController = gameStateController; 
        _poolManager = poolManager;
    }

    private void Start()
    {
        _currentHP = _maxHp;
        _gameStateController.OnGameStateChanged += HandleStateChange;
        if (_healthBar != null)
        {
            _healthBar.Setup(_maxHp);
            bool isPlaying = _gameStateController.CurrentState == GameState.Playing;
            _healthBar.DoVisible(isPlaying);
        }
    }

    private void HandleStateChange(GameState state)
    {
        if (state == GameState.Menu)
        {
            _currentHP = _maxHp;
            _isDead = false;
            if(_healthBar != null) _healthBar.UpdateHealth(_currentHP);
            gameObject.SetActive(true);
            if (_spawnedDeadCar != null)
            {
                Destroy(_spawnedDeadCar);
                _spawnedDeadCar = null;
            }
        }
        if(_healthBar != null)
        {
            _healthBar.DoVisible(state == GameState.Playing);
        }
    }

    public void TakeDamage(int amount)
    {
        if (_gameStateController.CurrentState != GameState.Playing || _isDead) return;
        _currentHP -= amount;
        if(_healthBar != null) _healthBar.UpdateHealth(_currentHP);
        SpawnDamageText(amount);
        if (_damageFeedback != null)
        {
            _damageFeedback.PlayFeedback();
        }
        if (_impulseSource != null)
        {
            _impulseSource.GenerateImpulse();
        }
        Debug.Log($"Car got damaged by {amount}. {_currentHP} HP Left");
        if (_currentHP <= 0) 
        {
            _isDead = true;
            Die(); 
        }
    }

    private void SpawnDamageText(int amount)
    {
        if(_poolManager == null || _textSpawnPoint == null) return;
        GameObject textObj = _poolManager.SpawnFromPool("DamageText", _textSpawnPoint.position, Quaternion.identity);
        if (textObj != null)
        {
            UIFloatingText floatingText = textObj.GetComponent<UIFloatingText>();
            if (floatingText != null)
            {
                floatingText.Init(_poolManager, amount, _driftTextToSide);
                _driftTextToSide = !_driftTextToSide;
            }
        }
    }

    private void Die()
    {
        if(_explosionVfxPrefab != null)
        {
            Instantiate(_explosionVfxPrefab, transform.position, transform.rotation);
        }
        if(_deadCarPrefab != null)
        {
            _spawnedDeadCar = Instantiate(_deadCarPrefab, transform.position, transform.rotation);
        }
        if (_impulseSource != null)
        {
            _impulseSource.GenerateImpulse(new Vector3(3, 3, 0));
        }
        if (_gameStateController != null)
        {
            _gameStateController.ChangeState(GameState.Lose);
        }
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (_gameStateController != null)
            _gameStateController.OnGameStateChanged -= HandleStateChange;
    }
}

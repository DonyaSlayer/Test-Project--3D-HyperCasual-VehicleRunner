using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using System.Collections;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.XR;

public enum EnemyState { Idle, Seeking, Attacking, Dying}

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Config")]
    [SerializeField] private EnemyConfig _config;

    [Header("References")]
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _exclamationMark;
    [SerializeField] private DamageFeedback _damageFeedback;

    [Header("UI")]
    [SerializeField] private UIWorldHealthBar _healthBar;
    [SerializeField] private Transform _textSpawnPoint;
    [SerializeField] private Transform _coinTextSpawnPoint;
    [SerializeField] private float _hpBarVisibleDistance;

    //Internal state
    private EnemyState _currentState;
    private int _currentHp;
    private Transform _playerTarget;
    private IDamageable _playerDamageable;
    private ObjectPoolManager _poolManager;
    private GameStateController _gameStateController;
    private CoinManager _coinManager;
    private bool _driftTextToSide = true;
    private bool _isHPBarVisible = false;

    //Attack variables
    private bool _useFirstAttack = true;
    private bool _isAtackOnCooldown = false;
    private bool _isAlerting = false;

    //Pushing Settings
    private bool _isPushedBack = false;
    private float _pushBackForce = 0f;
    private Vector3 _pushDirection;

    //UniTask token
    private CancellationTokenSource _cts;

    //Animations Hashing
    private readonly int _AnimIdle = Animator.StringToHash("Idle");
    private readonly int _AnimRun = Animator.StringToHash("Running");
    private readonly int _AnimAttack1 = Animator.StringToHash("Attack1");
    private readonly int _AnimAttack2 = Animator.StringToHash("Attack2");
    private readonly int _AnimDeath = Animator.StringToHash("Death");

    public void Initialize(Transform player, ObjectPoolManager poolManager, GameStateController gameStateController, CoinManager coinManager)
    {
        _playerTarget = player;
        if (_playerTarget != null)
        {
            _playerDamageable = player.GetComponent<IDamageable>();
        }
        _poolManager = poolManager;
        if (_gameStateController != null)
        {
            _gameStateController.OnGameStateChanged -= HandleGameStateChange;
        }
        _gameStateController = gameStateController;
        if (_gameStateController != null)
        {
            _gameStateController.OnGameStateChanged += HandleGameStateChange;
        }
        _coinManager = coinManager;
    }

    private void OnEnable()
    {
        _cts = new CancellationTokenSource();
        _currentHp = _config != null? _config.MaxHP : 20;
        _isPushedBack = false;
        _isAtackOnCooldown = false;
        _isAlerting = false;
        _exclamationMark.SetActive(false);
        _currentState = EnemyState.Idle;
        _animator.Play(_AnimIdle);
        _isHPBarVisible = false;
        if(_healthBar != null && _config != null)
        {
            _healthBar.Setup(_config.MaxHP);
        }
        
    }

    private void OnDisable()
    {
        CancelTasks();
        if(_gameStateController != null)
        {
            _gameStateController.OnGameStateChanged -= HandleGameStateChange;
        }
    }

    private void CancelTasks()
    {
        if(_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }

    public void TakeDamage(int amount)
    {
        if (_currentState == EnemyState.Dying) return;
        _currentHp -= amount;
        if(_healthBar != null) _healthBar.UpdateHealth(_currentHp);
        SpawnDamageText(amount);
        if(_damageFeedback != null) _damageFeedback.PlayFeedback();
        if(_currentHp <= 0) Die();
        
    }

    private void Update()
    {
        HandlePushBack();
        if(!CanAct()) return;
        if (CheckForDespawn()) return;
        float distanceToPlayer = Vector3.Distance(transform.position,_playerTarget.position);
        UpdateHPBarVisibility(distanceToPlayer);
        HandleCurrentState(distanceToPlayer);
    }

    private void HandleCurrentState(float distance)
    {
        switch (_currentState)
        {
            case EnemyState.Idle: HandleIdleState(distance); break;
            case EnemyState.Seeking: HandleSeekingState(distance); break;
            case EnemyState.Attacking: HandleAtackingState(distance); break;
        }
    }

    private void HandleIdleState(float distance)
    {
        if (distance <= _config.DetectionRadius && !_isAlerting && _cts != null)
        {
            _isAlerting = true;
            ShowExclamationAndSeekAsync(_cts.Token).Forget();
        }
    }
    private void HandleSeekingState(float distance)
    {
        LookAtPlayer();
        if(distance > _config.AttackRadius)
        {
            transform.position = Vector3.MoveTowards(transform.position, _playerTarget.position, _config.MoveSpeed * Time.deltaTime);
        }
        else
        {
            ChangeState(EnemyState.Attacking);
        }
    }
    private void HandleAtackingState(float distance)
    {
        LookAtPlayer();
        if (distance > _config.AttackRadius + 1f)
        {
            ChangeState(EnemyState.Seeking);
        }
        else if (!_isAtackOnCooldown && _cts != null)
        {
            AttackRoutineAsync(_cts.Token).Forget();
        }
    }
    private void Die()
    {
        _currentState = EnemyState.Dying;
        _animator.CrossFade(_AnimDeath, 0.1f);
        CancelTasks();
        int coinsToDrop = Random.Range(_config.MinCoinDrop, _config.MaxCoinDrop);
        if(_coinManager != null) _coinManager.AddCoinsForRun(coinsToDrop);
        SpawnCoinText(coinsToDrop);
        GameObject vfx = _poolManager.SpawnFromPool(GameConstants.Pools.EnemyVFX, transform.position, Quaternion.identity);
        if(vfx != null)
        {
            vfx.GetComponent<VfxPoolReturner>()?.Init(_poolManager, GameConstants.Pools.EnemyVFX, 5f);
            vfx.GetComponent<ParticleSystem>()?.Play();
        }
        ReturnToPoolAsync().Forget();
    }

    //UniTask methods

    private async UniTaskVoid ShowExclamationAndSeekAsync(CancellationToken token)
    {
        _exclamationMark.SetActive(true);
        bool isCancelled = await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f), cancellationToken: token).SuppressCancellationThrow();
        if (isCancelled) return;
        _exclamationMark.SetActive(false);
        ChangeState(EnemyState.Seeking);
    }
    private async UniTaskVoid AttackRoutineAsync(CancellationToken token)
    {
        _isAtackOnCooldown = true;
        if (_useFirstAttack) _animator.CrossFade(_AnimAttack1, 0.1f);
        else _animator.CrossFade(_AnimAttack2, 0.1f);
        _useFirstAttack = !_useFirstAttack;
        if (_playerDamageable != null)
        {
            _playerDamageable.TakeDamage(_config.AttackDamage);
        }
        bool isCancelled = await UniTask.Delay(System.TimeSpan.FromSeconds(_config.AttackCooldown), cancellationToken: token).SuppressCancellationThrow();
        if (!isCancelled) _isAtackOnCooldown = false;
    }
    private async UniTaskVoid ReturnToPoolAsync()
    {
        bool isCancelled = await UniTask.Delay(System.TimeSpan.FromSeconds(2.5f), cancellationToken: this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();
        if (!isCancelled)
        {
            _poolManager.ReturnToPool(GameConstants.Pools.Enemy, gameObject);
        }
    }
    //Addictional methods

    private void HandleGameStateChange(GameState state)
    {
        if (state == GameState.Win || state == GameState.Menu || state == GameState.Lose)
        {
            CancelTasks();
            _poolManager.ReturnToPool(GameConstants.Pools.Enemy, gameObject);
        }
    }
    private void ChangeState(EnemyState newState)
    {
        if (_currentState == EnemyState.Dying) return;
        _currentState = newState;
        switch (newState)
        {
            case EnemyState.Idle: _animator.CrossFade(_AnimIdle, 0.2f); break;
            case EnemyState.Seeking: _animator.CrossFade(_AnimRun, 0.2f); break;
        }
    }

    private void LookAtPlayer()
    {
        Vector3 direction = (_playerTarget.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    private void SpawnDamageText(int amount)
    {
        GameObject textObj = _poolManager.SpawnFromPool(GameConstants.Pools.DamageText, _textSpawnPoint.position, Quaternion.identity);
        if(textObj != null)
        {
            UIFloatingText floatingText = textObj.GetComponent<UIFloatingText>();
            if(floatingText != null )
            {
                floatingText.Init(_poolManager, amount, _driftTextToSide);
                _driftTextToSide = !_driftTextToSide;
            }
        }
    }

    private void SpawnCoinText(int amount)
    {
        if (_poolManager == null || _coinTextSpawnPoint == null) return;
        GameObject textObj = _poolManager.SpawnFromPool(GameConstants.Pools.CoinText, _coinTextSpawnPoint.position + Vector3.up * 0.5f, Quaternion.identity);
        {
            UIFloatingText floatingText = textObj.GetComponent <UIFloatingText>();
            if(floatingText != null ) floatingText.InitCoin(_poolManager, amount);
        }
    }

    private void HandlePushBack()
    {
        if(!_isPushedBack) return;
        transform.Translate(_pushDirection * (_pushBackForce * Time.deltaTime), Space.World);
        _pushBackForce = Mathf.Lerp(_pushBackForce, 0, Time.deltaTime * 5f);
        if(_pushBackForce < 0.1f)
        {
            _isPushedBack = false;
        }
    }

    private bool CanAct()
    {
        return _gameStateController != null &&
            _gameStateController.CurrentState == GameState.Playing &&
            _currentState != EnemyState.Dying &&
            _playerTarget != null &&
            _config != null;
    }

    private bool CheckForDespawn()
    {
        if (_playerTarget.position.z - transform.position.z > 10f)
        {
            _poolManager.ReturnToPool(GameConstants.Pools.Enemy, gameObject);
            return true;
        }
        return false;
    }

    private void UpdateHPBarVisibility(float distanceToPlayer)
    {
        if(!_isHPBarVisible && distanceToPlayer <= _hpBarVisibleDistance)
        {
            _isHPBarVisible = true;
            if(_healthBar != null)
            {
                _healthBar.DoVisible(true);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameConstants.Tags.Player) && _currentState != EnemyState.Dying)
        {
            Vector3 hitDirection = (transform.position - other.transform.position).normalized;
            hitDirection.y = 0;
            _pushDirection = hitDirection + other.transform.forward * 0.5f;
            _isPushedBack = true;
            _pushBackForce = 15f;
            TakeDamage(_config.CarCollDamageTaken);
        }
    }
}

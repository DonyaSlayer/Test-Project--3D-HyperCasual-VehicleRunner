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
    [Header("Enemy Stats")]
    [SerializeField] private int _maxHP;
    [SerializeField] private int _attackDmg;
    [SerializeField] private int _carCollDamageTaken;

    [Header("Enemy Settings")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _detectionRadius;
    [SerializeField] private float _attackRadius;
    [SerializeField] private float _attackCooldown;

    [Header("References")]
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _exclamationMark;

    //Internal state
    private EnemyState _currentState;
    private int _currentHp;
    private Transform _playerTarget;
    private IDamageable _playerDamageable;
    private ObjectPoolManager _poolManager;
    private bool _useFirstAttack = true;
    private bool _isAtackOnCooldown = false;

    //Pushing Settings
    private bool _isPushedBack = false;
    private float _pushBackForce = 0f;
    private Vector3 _pushDirection;

    //UniTask token
    private CancellationTokenSource _cts;

    //Animations Hashing
    private readonly int _AnimIdle = Animator.StringToHash("Idle");
    private readonly int _AnimRun = Animator.StringToHash("Run");
    private readonly int _AnimAttack1 = Animator.StringToHash("Attack1");
    private readonly int _AnimAttack2 = Animator.StringToHash("Attack2");
    private readonly int _AnimDeath = Animator.StringToHash("Death");

    public void Initiialize(Transform player, ObjectPoolManager poolManager)
    {
        _playerTarget = player;
        if (_playerTarget != null)
        {
            _playerDamageable = player.GetComponent<IDamageable>();
        }
        _poolManager = poolManager;
    }

    private void OnEnable()
    {
        _cts = new CancellationTokenSource();
        _currentHp = _maxHP;
        _isPushedBack = false;
        _isAtackOnCooldown = false;
        _exclamationMark.SetActive(false);
        ChangeState(EnemyState.Idle);    
    }

    private void OnDisable()
    {
        CancelTasks();
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
        if(_currentHp <= 0)
        {
            Die();
        }
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position,_playerTarget.position);
        HandleCurrentState(distanceToPlayer);
    }

    private void HandleCurrentState(float distance)
    {
        switch (_currentState)
        {
            case EnemyState.Idle:
                HandleIdleState(distance);
                break;
            case EnemyState.Seeking:
                HandleSeekingState(distance);
                break;
            case EnemyState.Attacking:
                HandleAtackingState(distance);
                break;
        }
    }

    private void HandleIdleState(float distance)
    {
        if (distance <= _detectionRadius && _cts != null)
        {
            ShowExclamationAndSeekAsync(_cts.Token).Forget();
        }
    }
    private void HandleSeekingState(float distance)
    {
        LookAtPlayer();
        if(distance > _attackRadius)
        {
            transform.position = Vector3.MoveTowards(transform.position, _playerTarget.position, _moveSpeed * Time.deltaTime);
        }
        else
        {
            ChangeState(EnemyState.Attacking);
        }
    }
    private void HandleAtackingState(float distance)
    {
        LookAtPlayer();
        if (distance > _attackRadius + 1f)
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
    }

    //UniTask methods

    private async UniTaskVoid ShowExclamationAndSeekAsync(CancellationToken token)
    {
        ChangeState(EnemyState.Seeking);
        _exclamationMark.SetActive(true);
        bool isCancelled = await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f), cancellationToken: token).SuppressCancellationThrow();
        if (isCancelled) return;
        _exclamationMark.SetActive(false);
    }
    private async UniTaskVoid AttackRoutineAsync(CancellationToken token)
    {
        _isAtackOnCooldown = true;
        if (_useFirstAttack) _animator.SetTrigger(_AnimAttack1);
        else _animator.SetTrigger(_AnimAttack2);
        _useFirstAttack = !_useFirstAttack;
        if (_playerDamageable != null)
        {
            _playerDamageable.TakeDamage(_attackDmg);
        }
        bool isCancelled = await UniTask.Delay(System.TimeSpan.FromSeconds(_attackCooldown), cancellationToken: token).SuppressCancellationThrow();
        if (!isCancelled)
            _isAtackOnCooldown = false;
    }
    private async UniTaskVoid ReturnToPoolAsync()
    {
        bool isCancelled = await UniTask.Delay(System.TimeSpan.FromSeconds(2.5f), cancellationToken: this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();
        if (!isCancelled)
        {
            _poolManager.ReturnToPool("Enemy", gameObject);
        }
    }

    //Addictional methods

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
}

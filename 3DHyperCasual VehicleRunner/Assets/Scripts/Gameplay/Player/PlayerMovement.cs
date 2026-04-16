using UnityEngine;
using Zenject;

public class PlayerMovement : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private PlayerConfig _config;

    [Header("Visual")]
    [SerializeField] private Animator[] _wheelsAnimators;
    [SerializeField] private TrailRenderer[] _wheelsTrails;
    private GameStateController _gameStateController;
    private Rigidbody _rb;
    private bool _canMove = false;
    private Vector3 _startPosition;
    private Quaternion _startRotation;

    [Inject]
    public void Construct (GameStateController gameStateController)
    {
        _gameStateController = gameStateController;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _startPosition = transform.position;
        _startRotation = transform.rotation;
    }

    private void Start()
    {
        _gameStateController.OnGameStateChanged += HandleStateChange;
        SetWheelSpeed(0f);
    }

    private void HandleStateChange(GameState newState)
    {
        switch (newState)
        {
            case GameState.Playing: _canMove = true;
                SetWheelSpeed(1f);
                break;
            case GameState.Win:
            case GameState.Lose: _canMove = false;
                StopMovement();
                SetWheelSpeed(0f);
                break;
            case GameState.Menu: _canMove = false;
                ResetPlayer();
                SetWheelSpeed(0f);
                break;
        }
    }
    private void StopMovement()
    {
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }
    private void SetWheelSpeed(float animSpeed)
    {
        if (_wheelsAnimators == null) return;
        foreach (var wheel in _wheelsAnimators)
        {
            if(wheel != null)
            {
                wheel.speed = animSpeed;
            }
        }
    }
    private void ResetPlayer()
    {
        StopMovement();
        transform.position = _startPosition;
        transform.rotation = _startRotation;
        if(_wheelsTrails != null)
        {
            foreach (var trail in _wheelsTrails)
            {
                if(trail != null)
                {
                    trail.Clear();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (!_canMove) return;
        Vector3 movement = transform.forward * _config.MoveSpeed;
        _rb.linearVelocity = new Vector3(movement.x, _rb.linearVelocity.y, movement.z);
    }

    private void OnDestroy()
    {
        if (_gameStateController != null)
        {
            _gameStateController.OnGameStateChanged -= HandleStateChange;
        }
    }
}

using UnityEngine;
using Zenject;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
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
    }

    private void HandleStateChange(GameState newState)
    {
        _canMove = (newState == GameState.Playing);
        if (!_canMove)
        {
            _rb.linearVelocity = Vector3.zero;
        }
        if (newState == GameState.Menu)
        {
            ResetPlayer();
        }
    }
    private void ResetPlayer()
    {
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        transform.position = _startPosition;
        transform.rotation = _startRotation;
    }

    private void FixedUpdate()
    {
        if (!_canMove) return;
        Vector3 movement = transform.forward * speed;
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

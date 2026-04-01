using UnityEngine;
using Zenject;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    private GameStateController _gameStateController;
    private Rigidbody _rb;
    private bool _canMove = false;

    [Inject]
    public void Construct (GameStateController gameStateController)
    {
        _gameStateController = gameStateController;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _gameStateController.OnGameStateChanged += HandleStateChange;
        //TEST
        _gameStateController.ChangeState(GameState.Playing);
    }

    private void HandleStateChange(GameState newState)
    {
        _canMove = (newState == GameState.Playing);
        if (!_canMove)
        {
            _rb.linearVelocity = Vector3.zero;
        }
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

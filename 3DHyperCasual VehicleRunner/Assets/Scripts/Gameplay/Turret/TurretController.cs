using UnityEngine;
using Zenject;

public class TurretController : MonoBehaviour
{
    [SerializeField] private WeaponConfig _weaponConfig;
    private InputHandler _inputHandler;
    private GameStateController _gameStateController;
    private Camera _mainCamera;
    private Vector2 _screenPosition;
    private bool _isAiming = false;
    private bool _canAim = false;

    [Inject]
    public void Construct(InputHandler inputHandler, GameStateController gameStateController)
    {
        _inputHandler = inputHandler;
        _gameStateController = gameStateController;
    }

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        _inputHandler.OnPositionHandlerChanged += HandlePointerPosition;
        _gameStateController.OnGameStateChanged += HandleStateChange;
    }

    private void HandlePointerPosition(Vector2 position) 
    {
        _screenPosition = position;
        _isAiming = true;
    }

    private void HandleStateChange(GameState newState) 
    {
        _canAim = (newState == GameState.Playing);
    }

    private void Update()
    {
        if (!_canAim || !_isAiming) return;
        AimTurret();
        _isAiming = false;
    }

    private void AimTurret()
    {
        Plane groundPlane = new Plane(Vector3.up, transform.position);
        Ray ray = _mainCamera.ScreenPointToRay(_screenPosition);
        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 targetPoint = ray.GetPoint(enter);
            Vector3 direction = targetPoint - transform.position;
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _weaponConfig.RotationSpeed *Time.deltaTime);
            }
        }
    }

    private void OnDestroy()
    {
        if (_inputHandler != null)
        {
            _inputHandler.OnPositionHandlerChanged -= HandlePointerPosition;
        }
        if (_gameStateController != null)
        {
            _gameStateController.OnGameStateChanged -= HandleStateChange;
        }
    }
}

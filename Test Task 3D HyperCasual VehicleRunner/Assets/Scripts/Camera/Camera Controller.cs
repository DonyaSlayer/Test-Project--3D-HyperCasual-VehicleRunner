using Unity.Cinemachine;
using UnityEngine;
using Zenject;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _garageCam;
    [SerializeField] private CinemachineCamera _followCam;
    [SerializeField] private float _transitionTime;

    private GameStateController _gameStateController;

    [Inject]

    public void Construct(GameStateController gameStateController)
    {
        _gameStateController = gameStateController;
    }

    private void Start()
    {
        _gameStateController.OnGameStateChanged += HandleStateChange;
        ResetCamera();
    }

    public void StartGameSequence()
    {
        _garageCam.Priority = 5;
        _followCam.Priority = 10;
        StartCoroutine(WaitAndStartMovement());
    }

    private IEnumerator WaitAndStartMovement()
    {
        yield return new WaitForSeconds(_transitionTime);
        _gameStateController.ChangeState(GameState.Playing);
    }

    private void HandleStateChange(GameState state)
    {
        if (state == GameState.Menu)
        {
            ResetCamera();
        }
    }

    private void ResetCamera()
    {
        _garageCam.Priority = 10;
        _followCam.Priority = 5;
    }
    private void OnDestroy()
    {
        if (_gameStateController != null) 
            _gameStateController.OnGameStateChanged -= HandleStateChange;
    }
}

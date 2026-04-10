using Unity.Cinemachine;
using UnityEngine;
using Zenject;
using System.Collections;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _garageCam;
    [SerializeField] private CinemachineCamera _followCam;
    [SerializeField] private CinemachineCamera _orbitCam;
    [SerializeField] private float _transitionTime;
    [SerializeField] private float _orbitSpeed;

    private GameStateController _gameStateController;
    private CinemachineOrbitalFollow _orbitalFollow;

    [Inject]

    public void Construct(GameStateController gameStateController)
    {
        _gameStateController = gameStateController;
    }

    private void Start()
    {
        _gameStateController.OnGameStateChanged += HandleStateChange;
        if (_orbitCam != null)
        {
            _orbitalFollow = _orbitCam.GetComponent<CinemachineOrbitalFollow>();
        }
        ResetCamera();
    }
    private void Update()
    {
        if (_gameStateController != null && _gameStateController.CurrentState == GameState.Win)
        {
            if (_orbitalFollow != null)
            {
                _orbitalFollow.HorizontalAxis.Value += _orbitSpeed * Time.deltaTime;
            }
        }
    }
    public void StartGameSequence()
    {
        _garageCam.Priority = 5;
        _followCam.Priority = 10;
        _orbitCam.Priority = 5;
        StartCoroutine(WaitAndStartMovement());
    }

    private IEnumerator WaitAndStartMovement()
    {
        yield return new WaitForSeconds(_transitionTime);
        _gameStateController.ChangeState(GameState.Playing);
    }

    private void HandleStateChange(GameState state)
    {
        switch (state)
        {
            case GameState.Menu:
                ResetCamera(); break;
            case GameState.Win:
                ActivateOrbitCam(); break;
            case GameState.Lose:
                ActivateOrbitCam(); break;
        }
    }

    private void ActivateOrbitCam()
    {
        _orbitCam.Priority = 10;
        _garageCam.Priority = 5;
        _followCam.Priority = 5;
    }
    private void ResetCamera()
    {

        _garageCam.Priority = 10;
        _followCam.Priority = 5;
        _orbitCam.Priority = 5;
    }
    private void OnDestroy()
    {
        if (_gameStateController != null) 
            _gameStateController.OnGameStateChanged -= HandleStateChange;
    }
}

using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIMainMenu : MonoBehaviour
{
    [SerializeField] Button _startButton;
    [SerializeField] CameraController _cameraController;
    private GameStateController _gameStateController;

    [Inject]
    public void Construct(GameStateController gameStateController)
    {
        _gameStateController = gameStateController; 
    }

    private void Start()
    {
        _gameStateController.OnGameStateChanged += HandleStateChange;
        _startButton.onClick.AddListener(OnStartClicked);
    }

    private void HandleStateChange(GameState state)
    {
        if (state == GameState.Menu)
        {
            gameObject.SetActive(true);
            _startButton.interactable = true;
        }
        else if (state == GameState.Playing)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnStartClicked()
    {
        _startButton.interactable= false;
        _cameraController.StartGameSequence();
    }

    private void OnDestroy()
    {
        if ( _gameStateController != null ) 
            _gameStateController.OnGameStateChanged -= HandleStateChange;
    }
}

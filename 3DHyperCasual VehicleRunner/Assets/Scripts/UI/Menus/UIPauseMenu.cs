using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;

public class UIPauseMenu : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _homeButton;
    [SerializeField] private float _fadeDuration;

    [Header("References")]
    [SerializeField] private CameraController _cameraController;
    private GameStateController _gameStateController;

    [Inject]
    public void Construct(GameStateController gameStateController)
    {
        _gameStateController = gameStateController;
    }

    private void Start()
    {
        _continueButton.onClick.AddListener(ResumeGame);
        _restartButton.onClick.AddListener(RestartGame);
        _homeButton.onClick.AddListener(GoHome);
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }
    public void PauseGame()
    {
        if (_gameStateController.CurrentState != GameState.Playing) return;
        Time.timeScale = 0f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.DOFade(1f, _fadeDuration).SetUpdate(true);
    }
    private void ResumeGame()
    {
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.DOFade(0f, _fadeDuration).SetUpdate(true).OnComplete(() =>
        {
            Time.timeScale = 1f;
        });
    }
    private void RestartGame() 
    {
        Time.timeScale = 1f;
        _gameStateController.ChangeState(GameState.Menu);
        _cameraController.StartGameSequence();
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }
    private void GoHome()
    {
        Time.timeScale = 1f;
        _gameStateController.ChangeState(GameState.Menu);
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }
}

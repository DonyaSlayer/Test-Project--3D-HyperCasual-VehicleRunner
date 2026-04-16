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
        SetInteractable(false);
    }
    public void PauseGame()
    {
        if (_gameStateController.CurrentState != GameState.Playing) return;
        Time.timeScale = 0f;
        SetInteractable(true);
        _canvasGroup.DOKill();
        _canvasGroup.DOFade(1f, _fadeDuration).SetUpdate(true);
    }
    private void ResumeGame()
    {
        SetInteractable(false);
        _canvasGroup.DOKill();
        _canvasGroup.DOFade(0f, _fadeDuration).SetUpdate(true).OnComplete(() =>
        {
            Time.timeScale = 1f;
        });
    }
    private void RestartGame() 
    {
        ResetTimeAndHide();
        _gameStateController.ChangeState(GameState.Menu);
        _cameraController.StartGameSequence();
    }
    private void GoHome()
    {
        ResetTimeAndHide();
        _gameStateController.ChangeState(GameState.Menu);
    }
    private void ResetTimeAndHide()
    {
        Time.timeScale = 1f;
        _canvasGroup.DOKill();
        _canvasGroup.alpha = 0f;
        SetInteractable(false);
    }
    private void SetInteractable(bool state)
    {
        _canvasGroup.interactable = state;
        _canvasGroup.blocksRaycasts = state;
    }
    private void OnDestroy()
    {
        _canvasGroup?.DOKill();
    }
}

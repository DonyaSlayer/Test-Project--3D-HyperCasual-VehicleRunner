using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using DG.Tweening;

public class UIEndGameMenu : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _fadeDuration;
    [SerializeField] private Image _headerImage;
    [SerializeField] private TMP_Text _statusText;
    [SerializeField] private CameraController _cameraController;

    [Header("Win/Lose Settings")]
    [SerializeField] private Sprite _winSprite;
    [SerializeField] private string _winTitle;
    [SerializeField] private Color _winColor = Color.green;
    [SerializeField] private Sprite _loseSprite;
    [SerializeField] private string _loseTitle;
    [SerializeField] private Color _loseColor = Color.red;

    [Header("Buttons")]
    [SerializeField] private Button _homeButton;
    [SerializeField] private Button _restartButton;

    private GameStateController _gameStateController;

    [Inject] 
    public void Construct(GameStateController gameStateController)
    {
        _gameStateController = gameStateController;
    }

    private void Start()
    {
        _gameStateController.OnGameStateChanged += HandleStateChange;
        _homeButton.onClick.AddListener(OnHomeClicked);
        _restartButton.onClick.AddListener(OnRestartClicked);
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    private void HandleStateChange(GameState state)
    {
        _canvasGroup.DOKill();
        if(state == GameState.Win || state == GameState.Lose)
        {
            SetupVisual(state);
            _canvasGroup.DOFade(1f, _fadeDuration).SetDelay(1.5f).OnComplete(() =>
            {
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
            });
        }
        else
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.DOFade(0f, _fadeDuration);
        }
    }
    private void OnHomeClicked()
    {
        _gameStateController.ChangeState(GameState.Menu);
    }

    private void OnRestartClicked()
    {
        _gameStateController.ChangeState(GameState.Menu);
        _cameraController.StartGameSequence();
    }

    private void SetupVisual(GameState state)
    {
        if (state == GameState.Win)
        {
            _headerImage.sprite = _winSprite;
            _statusText.text = _winTitle;
            _statusText.color = _winColor;
        }
        else if (state == GameState.Lose)
        {
            _headerImage.sprite = _loseSprite;
            _statusText.text = _loseTitle;
            _statusText.color = _loseColor;
        }
    }

    private void OnDestroy()
    {
        if (_gameStateController != null) _gameStateController.OnGameStateChanged -= HandleStateChange;
    }
}

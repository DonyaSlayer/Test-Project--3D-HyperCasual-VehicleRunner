using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;
using DG.Tweening;

public class UIMainMenu : MonoBehaviour
{
    [Header("Global")]
    [SerializeField] private CanvasGroup _globalCanvasGroup;
    [SerializeField] private float _fadeDuration;
    [SerializeField] private float _panelSwitchSpeed;

    [Header("Main Menu")]
    [SerializeField] private CanvasGroup _mainMenuCG;
    [SerializeField] Button _startButton;
    [SerializeField] Button _recordsButton;
    [SerializeField] Button _exitButton;

    [Header("Records")]
    [SerializeField] private CanvasGroup _recordsCG;
    [SerializeField] private TMP_Text _maxDistText;
    [SerializeField] private TMP_Text _maxCoinsText;
    [SerializeField] Button _backButton;

    [Header("Refs")]
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
        _exitButton.onClick.AddListener(() => Application.Quit());
        _recordsButton.onClick.AddListener(OpenRecordsPanel);
        _backButton.onClick.AddListener(CloseRecordsPanel);
        if (PlayerPrefs.GetInt("AutoStartNextGame", 0) == 1)
        {
            PlayerPrefs.SetInt("AutoStartNextGame", 0);
            OnStartClicked();
            return;
        }
        ResetToMainMenu();
    }

    private void UpdateRecordsUI()
    {
        int maxDist = PlayerPrefs.GetInt("MaxDistance", 0);
        int maxCoins = PlayerPrefs.GetInt("MaxRunCoins", 0);
        _maxDistText.text = $"Max distance: {maxDist}m";
        _maxCoinsText.text = $"Max coins for run: {maxCoins}  <sprite=0>";
    }

    private void OpenRecordsPanel()
    {
        UpdateRecordsUI();
        _mainMenuCG.interactable = false;
        _mainMenuCG.blocksRaycasts = false;
        _mainMenuCG.DOFade(0f, _panelSwitchSpeed);
        _recordsCG.DOFade(1f, _panelSwitchSpeed).OnComplete(() =>
        {
            _recordsCG.interactable = true;
            _recordsCG.blocksRaycasts = true;
        });
    }

    private void CloseRecordsPanel()
    {
        _recordsCG.interactable = false;
        _recordsCG.blocksRaycasts = false;
        _recordsCG.DOFade(0f, _panelSwitchSpeed);
        _mainMenuCG.DOFade(1f, _panelSwitchSpeed).OnComplete(() =>
        {
            _mainMenuCG.interactable = true;
            _mainMenuCG.blocksRaycasts = true;
        });
    }

    private void ResetToMainMenu()
    {
        _mainMenuCG.alpha = 1f;
        _mainMenuCG.interactable = true;
        _mainMenuCG.blocksRaycasts = true;
        _recordsCG.alpha = 0f;
        _recordsCG.interactable = false;
        _recordsCG.blocksRaycasts = false;
        _globalCanvasGroup.alpha = 1f;
        _globalCanvasGroup.interactable = true;
        _globalCanvasGroup.blocksRaycasts= true;
        _startButton.interactable= true;
    }
    private void HandleStateChange(GameState state)
    {
        _globalCanvasGroup.DOKill();
        if (state == GameState.Menu)
        { 
            ResetToMainMenu();
            _globalCanvasGroup.DOFade(1f,_fadeDuration).OnComplete(() =>
            {
                _globalCanvasGroup.interactable = true;
                _globalCanvasGroup.blocksRaycasts = true;
            });
        }
        else 
        {
            _globalCanvasGroup.interactable = false;
            _globalCanvasGroup.blocksRaycasts = false;
            _globalCanvasGroup.DOFade(0f, _fadeDuration);
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

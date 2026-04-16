using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;
using TMPro;

public class UILevelProgress : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider _progressSlider;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _fadeDuration;
    [SerializeField] private TMP_Text _distanceText;

    private GameStateController _gameStateController;
    private Transform _playerTransform;
    private float _startZ;
    private float _endZ;
    private bool _isInitialized = false;

    [Inject]
    public void Construct(GameStateController gameStateController)
    {
        _gameStateController = gameStateController;
        _gameStateController.OnGameStateChanged += HandleStateChange;
    }

    public void Initialize(Transform player, float startZ, float endZ)
    {
        _playerTransform = player;
        _startZ = startZ;
        _endZ = endZ;
        _progressSlider.value = 0f;
        _distanceText.text = "0m";
        _isInitialized = true;
    }

    private void Start()
    {
        _canvasGroup.alpha = 0f;
    }
    private void HandleStateChange(GameState state)
    {
        _canvasGroup.DOKill();
        if(state == GameState.Playing)
        {
            _canvasGroup.DOFade(1f, _fadeDuration).SetEase(Ease.InOutQuad);
        }
        else
        {
            _canvasGroup.DOFade(0f, _fadeDuration).SetEase(Ease.InOutQuad);
            if (state == GameState.Win || state == GameState.Lose)
            {
                SaveMaxDistance();
            }
        }
    }

    private void Update()
    {
        if (!_isInitialized || _gameStateController.CurrentState != GameState.Playing || _playerTransform == null) return; 
        float currentZ = _playerTransform.position.z;
        float currentProgress = Mathf.InverseLerp(_startZ, _endZ, _playerTransform.position.z);
        _progressSlider.value = currentProgress;
        float distanceDriven = Mathf.Max(0, currentZ - _startZ);
        _distanceText.text = $"{Mathf.FloorToInt(distanceDriven)}m";
    }

    private void SaveMaxDistance()
    {
        if(_playerTransform == null) return;
        float distanceDriven = Mathf.Max(0, _playerTransform.position.z - _startZ);
        int currentDistInt = Mathf.FloorToInt(distanceDriven);
        int maxDist = PlayerPrefs.GetInt(GameConstants.Prefs.MaxDistance, 0);
        if(currentDistInt > maxDist)
        {
            PlayerPrefs.SetInt(GameConstants.Prefs.MaxDistance, currentDistInt);
            PlayerPrefs.Save();
        }
    }

    private void OnDestroy()
    {
        _canvasGroup?.DOKill();
        if(_gameStateController != null) _gameStateController.OnGameStateChanged -= HandleStateChange;  
    }
}

using UnityEngine;
using TMPro;
using DG.Tweening;
using Zenject;

public class UICoinCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text _totalCoinsText;
    [SerializeField] private CanvasGroup _menuCoinCG;
    [SerializeField] private TMP_Text _runCoinText;
    [SerializeField] private CanvasGroup _runCoinCG;
    private CoinManager _coinManager;
    private GameStateController _gameStateController;

    [Inject]
    public void Construct (CoinManager coinManager, GameStateController gameStateController)
    {
        _coinManager = coinManager;
        _gameStateController = gameStateController;
    }

    private void Start()
    {
        _coinManager.OnRunCoinsChanged += UpdateRunCoinsUI;
        _coinManager.OnTotalCoinsChanged += UpdateTotalCoinsUI;
        _gameStateController.OnGameStateChanged += HandleStateChange;
        UpdateTotalCoinsUI(_coinManager.GetTotalScore());
        UpdateRunCoinsUI(0);
        _runCoinCG.alpha = 0f;
        _menuCoinCG.alpha = 1f;

    }

    private void UpdateRunCoinsUI(int currentCoins)
    {
        _runCoinText.text = $"<sprite=0> : {currentCoins}";
        _runCoinText.transform.DORewind();
        _runCoinText.transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 0, 0);
    }
    private void UpdateTotalCoinsUI (int totalCoins)
    {
        _totalCoinsText.text = $"<sprite=0> : {totalCoins}";
    }
    private void HandleStateChange(GameState state)
    {
        _menuCoinCG.DOKill();
        _runCoinCG.DOKill();
        switch (state)
        {
            case GameState.Menu:
                UpdateTotalCoinsUI(_coinManager.GetTotalScore());
                _menuCoinCG.DOFade(1f, 0.5f);
                _runCoinCG.DOFade(0f, 0.5f);
                break;
            case GameState.Playing:
                _coinManager.ResetRunScore();
                _menuCoinCG.DOFade(0f, 0.5f);
                _runCoinCG.DOFade(1f, 0.5f);
                break;
            case GameState.Win:
            case GameState.Lose:
                _coinManager.SaveToTotalScore();
                _runCoinCG.DOFade(0f, 0.5f);
                break;
        }
    }


    private void OnDestroy()
    {
        if (_coinManager != null)
        {
            _coinManager.OnRunCoinsChanged -= UpdateRunCoinsUI;
            _coinManager.OnTotalCoinsChanged -= UpdateTotalCoinsUI;
        }
        if (_gameStateController != null)
        {
            _gameStateController.OnGameStateChanged -= HandleStateChange;
        }
    }
}

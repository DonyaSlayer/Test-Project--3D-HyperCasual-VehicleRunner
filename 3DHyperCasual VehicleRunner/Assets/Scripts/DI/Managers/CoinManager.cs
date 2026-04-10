using System;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public event Action<int> OnRunCoinsChanged;
    public event Action<int> OnTotalCoinsChanged;
    private int _currentRunCoins = 0;
    private int _totalCollectedCoins = 0;
    private const string TOTAL_COINS_KEY = "TotakCoins";

    private void Start()
    {
        _totalCollectedCoins = PlayerPrefs.GetInt(TOTAL_COINS_KEY, 0);
    }

    public void AddCoinsForRun(int amount)
    {
        _currentRunCoins += amount;
        OnRunCoinsChanged?.Invoke(_currentRunCoins);
    }

    public void SaveToTotalScore()
    {
        if (_currentRunCoins > 0)
        {
            _totalCollectedCoins += _currentRunCoins;
            PlayerPrefs.SetInt(TOTAL_COINS_KEY, _totalCollectedCoins);
            int maxCoinsForRun = PlayerPrefs.GetInt("MaxRunCoins", 0);
            if(_currentRunCoins > maxCoinsForRun)
            {
                PlayerPrefs.SetInt("MaxRunCoins", _currentRunCoins);
            }
            PlayerPrefs.Save();
            OnTotalCoinsChanged?.Invoke(_totalCollectedCoins);
        }
    }
    public void ResetRunScore()
    {
        _currentRunCoins = 0;
        OnRunCoinsChanged?.Invoke(_currentRunCoins);
    }

    public int GetTotalScore()=>_totalCollectedCoins;
    public int GetRunScore() =>_currentRunCoins;
}

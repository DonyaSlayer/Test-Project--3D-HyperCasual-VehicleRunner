using System;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public event Action<int> OnRunCoinsChanged;
    public event Action<int> OnTotalCoinsChanged;
    private int _currentRunCoins = 0;
    private int _totalCollectedCoins = 0;

    private void Start()
    {
        _totalCollectedCoins = PlayerPrefs.GetInt(GameConstants.Prefs.TotalCoins, 0);
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
            PlayerPrefs.SetInt(GameConstants.Prefs.TotalCoins, _totalCollectedCoins);
            int maxCoinsForRun = PlayerPrefs.GetInt(GameConstants.Prefs.TotalCoins, 0);
            if(_currentRunCoins > maxCoinsForRun)
            {
                PlayerPrefs.SetInt(GameConstants.Prefs.TotalCoins, _currentRunCoins);
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

using UnityEngine;
using Zenject;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int _maxHp;
    private int _currentHP;
    private GameStateController _gameStateController;

    [Inject]
    public void Construct(GameStateController gameStateController)
    {
        _gameStateController = gameStateController; 
    }

    private void Start()
    {
        _currentHP = _maxHp;
        _gameStateController.OnGameStateChanged += HandleStateChange;
    }

    private void HandleStateChange(GameState state)
    {
        if (state == GameState.Menu)
        {
            _currentHP = _maxHp;
        }
    }

    public void TakeDamage(int amount)
    {
        if (_gameStateController.CurrentState != GameState.Playing) return;
        _currentHP -= amount;
        //TEST
        Debug.Log($"Car got damaged by {amount}. {_currentHP} HP Left");
        if (_currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _gameStateController.ChangeState(GameState.Lose);
    }

    private void OnDestroy()
    {
        if (_gameStateController != null)
            _gameStateController.OnGameStateChanged -= HandleStateChange;
    }
}

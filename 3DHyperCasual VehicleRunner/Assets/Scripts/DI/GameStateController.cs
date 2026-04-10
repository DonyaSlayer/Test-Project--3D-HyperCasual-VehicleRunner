using UnityEngine;
using System;
public enum GameState { Menu, Playing, Win, Lose}

public class GameStateController
{
    public GameState CurrentState { get; private set; } = GameState.Menu;
    public event Action<GameState> OnGameStateChanged;

    public void ChangeState(GameState newState)
    {
        if(CurrentState == newState) return;
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);
    }
}

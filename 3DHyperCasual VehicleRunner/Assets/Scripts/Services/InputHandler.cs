using UnityEngine;
using System;
using UnityEngine.InputSystem;
using Zenject;

public class InputHandler : IInitializable, ITickable, IDisposable
{
    public event Action OnScreenTapped;
    public event Action<Vector2> OnPositionHandlerChanged;
    private InputMap _inputMap;
    private bool _isTouching;

    public InputHandler()
    {
        _inputMap = new InputMap();
    }

    public void Initialize()
    {
        _inputMap.Enable();
        _inputMap.Gameplay.Tap.performed += OnTapPerformed;
        _inputMap.Gameplay.Tap.canceled += OnTapCanceled;
    }

    private void OnTapPerformed(InputAction.CallbackContext context)
    {
        _isTouching = true;
        OnScreenTapped?.Invoke();
    }
    private void OnTapCanceled(InputAction.CallbackContext context)
    {
        _isTouching = false;
    }

    public void Tick()
    {
        if (_isTouching)
        {
            Vector2 controllerPosition = _inputMap.Gameplay.PositionHandler.ReadValue<Vector2>();
            OnPositionHandlerChanged?.Invoke(controllerPosition);
        }
    }

    public void Dispose()
    {
        _inputMap.Gameplay.Tap.performed -= OnTapPerformed;
        _inputMap.Gameplay.Tap.canceled -= OnTapCanceled;
        _inputMap.Disable();
    }
}

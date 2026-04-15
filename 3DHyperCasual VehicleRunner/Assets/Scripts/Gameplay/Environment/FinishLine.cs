using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private Collider _finishTrigger;
    private GameStateController _gameStateController;

    private void Awake()
    {
        if (_finishTrigger != null) _finishTrigger.enabled = false;
    }

    public void ActivateOnFinishChunk(GameStateController controller)
    {
        _gameStateController = controller;
        if(_finishTrigger != null) _finishTrigger.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Smth touched the finish collider: {other.gameObject.name}, with tag: {other.tag}");
        if (other.CompareTag("Player") && _gameStateController.CurrentState == GameState.Playing)
        {
            _gameStateController.ChangeState(GameState.Win);
            Debug.Log("Car is on finish line!");
        }
    }
}

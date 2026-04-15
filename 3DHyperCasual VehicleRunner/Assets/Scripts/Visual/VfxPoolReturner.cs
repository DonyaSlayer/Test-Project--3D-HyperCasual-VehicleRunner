using Cysharp.Threading.Tasks;
using UnityEngine;

public class VfxPoolReturner : MonoBehaviour
{
    private ObjectPoolManager _poolManager;
    private string _poolTag;

    public void Init(ObjectPoolManager poolManager, string poolTag, float lifeTime = 5f)
    {
        _poolManager = poolManager;
        _poolTag = poolTag;
        ReturnAfterDelayAsync(lifeTime).Forget();
    }

    private async UniTaskVoid ReturnAfterDelayAsync(float delay)
    {
        await UniTask.Delay(System.TimeSpan.FromSeconds(delay), cancellationToken: this.GetCancellationTokenOnDestroy());
        if(_poolManager != null )
        {
            _poolManager.ReturnToPool(_poolTag, gameObject);
        }
    }
}

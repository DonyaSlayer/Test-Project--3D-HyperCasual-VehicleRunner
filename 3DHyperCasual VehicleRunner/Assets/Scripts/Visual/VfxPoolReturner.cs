using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class VfxPoolReturner : MonoBehaviour
{
    private ObjectPoolManager _poolManager;
    private string _poolTag;
    private CancellationTokenSource _cts;

    private void OnEnable()
    {
        _cts = new CancellationTokenSource();
    }
    private void OnDisable()
    {
        if(_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }

    public void Init(ObjectPoolManager poolManager, string poolTag, float lifeTime = 5f)
    {
        _poolManager = poolManager;
        _poolTag = poolTag;
        if(_cts != null) ReturnAfterDelayAsync(lifeTime, _cts.Token).Forget();
    }

    private async UniTaskVoid ReturnAfterDelayAsync(float delay, CancellationToken token)
    {
        bool isCancelled = await UniTask.Delay(System.TimeSpan.FromSeconds(delay), cancellationToken: token).SuppressCancellationThrow();
        if(!isCancelled && _poolManager != null )
        {
            _poolManager.ReturnToPool(_poolTag, gameObject);
        }
    }
}

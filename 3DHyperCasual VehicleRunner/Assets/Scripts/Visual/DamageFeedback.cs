using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using System.Threading;
public class DamageFeedback : MonoBehaviour
{
    [Header("Flash Settings")]
    [SerializeField] private Renderer[] _renderers;
    [SerializeField] private Material _flashMaterial;
    [SerializeField] private int _flashDurationMs;

    [Header("Shake Settings (for enemies only)")]
    [SerializeField] private Transform _meshTransform;
    [SerializeField] private bool _useShake = false;
    [SerializeField] private float _shakeDuration;
    [SerializeField] private float _shakeStrength;
    private Material[][] _originalMaterials;
    private bool _isFlashing = false;
    private CancellationTokenSource _cts;

    private void Awake()
    {
        _originalMaterials = new Material[_renderers.Length][];
        for(int i = 0; i < _renderers.Length; i++)
        {
            _originalMaterials[i] = _renderers[i].sharedMaterials;
        }
    }
    private void OnEnable()
    {
        _cts = new CancellationTokenSource();
    }
    private void OnDisable()
    {
        if (_cts != null)
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
        _meshTransform?.DOKill();
        if(_isFlashing)
        {
            RestoreOriginalMaterials();
        }
    }
    public void PlayFeedback()
    {
        if (!_isFlashing && _cts != null)
        {
            FlashRoutineAsync(_cts.Token).Forget();
        }
        if (_useShake && _meshTransform != null)
        {
            _meshTransform.DOKill();
            _meshTransform.DOShakePosition(_shakeDuration, _shakeStrength, vibrato: 10, randomness: 90).SetRelative(true);
        }
    }
    private async UniTaskVoid FlashRoutineAsync(CancellationToken token)
    {
        _isFlashing = true;
        for(int i = 0; i < _renderers.Length;i++)
        {
            Material[] flashMats = new Material[_originalMaterials[i].Length];
            for (int j = 0; j < flashMats.Length; j++) flashMats[j] = _flashMaterial;
            _renderers[i].sharedMaterials = flashMats;
        }
        bool isCancelled = await UniTask.Delay(_flashDurationMs, cancellationToken: token).SuppressCancellationThrow();
        if(!isCancelled) RestoreOriginalMaterials();
    }
    private void RestoreOriginalMaterials()
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            if (_renderers[i] != null)
            {
                _renderers[i].sharedMaterials = _originalMaterials[i];
            }
        }
        _isFlashing = false;
    }
}

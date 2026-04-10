using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
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

    private void Awake()
    {
        _originalMaterials = new Material[_renderers.Length][];
        for(int i = 0; i < _renderers.Length; i++)
        {
            _originalMaterials[i] = _renderers[i].sharedMaterials;
        }
    }

    public void PlayFeedback()
    {
        if (!_isFlashing)
        {
            FlashRoutineAsync().Forget();
        }
        if (_useShake && _meshTransform != null)
        {
            _meshTransform.DOComplete();
            _meshTransform.DOShakePosition(_shakeDuration, _shakeStrength, vibrato: 10, randomness: 90).SetRelative(true);
        }
    }
    private async UniTaskVoid FlashRoutineAsync()
    {
        _isFlashing = true;
        for(int i = 0; i < _renderers.Length;i++)
        {
            Material[] flashMats = new Material[_originalMaterials[i].Length];
            for (int j = 0; j < flashMats.Length; j++) flashMats[j] = _flashMaterial;
            _renderers[i].sharedMaterials = flashMats;
        }
        await UniTask.Delay(_flashDurationMs);
        for(int i = 0; i< _renderers.Length;i++)
        {
            _renderers[i].sharedMaterials = _originalMaterials[i];   
        }
        _isFlashing = false;
    }
}

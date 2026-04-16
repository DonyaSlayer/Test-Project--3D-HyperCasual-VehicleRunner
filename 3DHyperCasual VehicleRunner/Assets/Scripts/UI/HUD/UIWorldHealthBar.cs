using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIWorldHealthBar : MonoBehaviour
{
    [SerializeField] private Image _fillImage;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _fillSpeed;
    private Camera _mainCamera;
    private int _maxHp;

    private void Start()
    {
        _mainCamera = Camera.main;
        if( _canvasGroup == null ) _canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Setup(int maxHp)
    {
        _maxHp = maxHp;
        _fillImage.fillAmount = 1f;
        _canvasGroup.DOKill();
        _canvasGroup.alpha = 0f;
    }

    public void UpdateHealth(int currentHP)
    {
        float targetFill = (float)currentHP / _maxHp;
        _fillImage.DOKill();
        _fillImage.DOFillAmount(targetFill,_fillSpeed).SetEase(Ease.OutCubic);
        if(currentHP <= 0)
        {
            _canvasGroup.DOFade(0f, 0.2f);
        }
    }

    public void DoVisible(bool isVisible)
    {
        _canvasGroup.DOKill();
        _canvasGroup.DOFade(isVisible ? 1f : 0f, 0.3f);
    }

    private void LateUpdate()
    {
        if (_mainCamera != null)
        {
            transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward, _mainCamera.transform.rotation * Vector3.up);
        }
    }

    private void OnDisable()
    {
        _fillImage?.DOKill();
        _canvasGroup?.DOKill();
    }
}

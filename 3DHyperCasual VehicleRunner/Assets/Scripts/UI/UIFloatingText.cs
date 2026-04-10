using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIFloatingText : MonoBehaviour
{
    [SerializeField] private TMP_Text _tMP;
    [SerializeField] private float _floatDuration;
    [SerializeField] private float _floatHeight;
    [SerializeField] private float _sideDrift;

    private ObjectPoolManager _poolManager;

    public void Init(ObjectPoolManager poolManager, int damageAmount, bool driftToSide)
    {
        _poolManager = poolManager;
        _tMP.text = $"-{damageAmount}";
        Color c = _tMP.color;
        c.a = 1f;
        _tMP.color = c;
        transform.DOComplete();
        _tMP.DOComplete();
        float sideDir = driftToSide? -_sideDrift: _sideDrift;
        Vector3 targetPos = transform.position + new Vector3(sideDir,_floatHeight, 0);
        transform.DOMove(targetPos, _floatDuration).SetEase(Ease.OutCirc);
        _tMP.DOFade(0f, _floatDuration).SetEase(Ease.InExpo).OnComplete(() =>
        {
            if (_poolManager != null)
            {
                _poolManager.ReturnToPool("DamageText", gameObject);
            }
        });
    }

    public void InitCoin(ObjectPoolManager poolManager, int coinAmount)
    {
        _poolManager = poolManager;
        Color c = Color.yellow;
        c.a = 1f;
        _tMP.color = c;
        _tMP.text = $"+{coinAmount}  <sprite=0>";
        transform.DOComplete();
        _tMP.DOComplete();
        Vector3 targetPos = transform.position + new Vector3(0f, _floatHeight, 0);
        transform.DOMove(targetPos, _floatDuration).SetEase(Ease.OutBack);
        _tMP.DOFade(0f, _floatDuration).SetEase(Ease.InExpo).OnComplete(() =>
        {
            if (_poolManager != null)
            {
                _poolManager.ReturnToPool("CoinText", gameObject);
            }
        });
    }
}

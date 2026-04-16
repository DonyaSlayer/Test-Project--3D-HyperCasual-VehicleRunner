using DG.Tweening;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Rendering;

public class Barrier : MonoBehaviour
{
    [Header("Barier Settings")]
    [SerializeField] private Transform _barierPivot;
    [SerializeField] private float _openAngle;
    [SerializeField] private float _openSpeed;
    [SerializeField] private Vector3 _rotationAxis = new Vector3(1, 0, 0);

    private Quaternion _closedRotation;
    private Quaternion _openRotation;

    private void Start()
    {
        _closedRotation = _barierPivot.localRotation;
        _openRotation = _closedRotation * Quaternion.Euler(_rotationAxis * _openAngle);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameConstants.Tags.Player))
        {
            _barierPivot.DOKill();
            _barierPivot.DOLocalRotateQuaternion(_openRotation, _openSpeed).SetEase(Ease.OutBack);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(GameConstants.Tags.Player))
        {
            _barierPivot.DOKill();
            _barierPivot.DOLocalRotateQuaternion(_closedRotation, _openSpeed).SetEase(Ease.InOutQuad);
        }
    }

    private void OnDisable()
    {
        if(_barierPivot != null)
        {
            _barierPivot.DOKill();
        }
    }
    private void OnDestroy()
    { 
        if (_barierPivot != null)
        {
            _barierPivot.DOKill();
        }
    }
}

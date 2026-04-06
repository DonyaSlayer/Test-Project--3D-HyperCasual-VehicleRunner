using UnityEngine;
using UnityEngine.Rendering;

public class Barier : MonoBehaviour
{
    [Header("BarierSettings")]
    [SerializeField] private Transform _barierPivot;
    [SerializeField] private float _openAngle;
    [SerializeField] private float _openSpeed;
    [SerializeField] private Vector3 _rotationAxis = new Vector3(1,0, 0);
    private Quaternion _closedRotation;
    private Quaternion _openRotation;
    private bool _isOpen = false;

    private void Start()
    {
        _closedRotation = _barierPivot.localRotation;
        _openRotation = _closedRotation * Quaternion.Euler(_rotationAxis * _openAngle);
    }

    private void Update()
    {
        Quaternion targetRotation = _isOpen? _openRotation : _closedRotation;
        _barierPivot.localRotation = Quaternion.Slerp(_barierPivot.localRotation, targetRotation, Time.deltaTime * _openSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isOpen = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isOpen = false;
        }
    }
}

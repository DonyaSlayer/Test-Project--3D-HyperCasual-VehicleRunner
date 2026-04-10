using UnityEngine;

public class VfxCarExplosion : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float _explosionForce;
    [SerializeField] private float _explosionRadius;
    [SerializeField] private float _upwardModifier;

    private void Start()
    {
        Rigidbody[] parts = GetComponentsInChildren<Rigidbody>();
        Vector3 explosionPos = transform.position + Vector3.down *0.5f;
        foreach(Rigidbody rb in parts)
        {
            rb.AddExplosionForce(_explosionForce, explosionPos, _explosionRadius, _upwardModifier, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
        }
    }
}

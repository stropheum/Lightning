using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LightningNode : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 1.0f;
    private Rigidbody _rigidbody;

    private void Awake()
    {
    }

    private void FixedUpdate()
    {
    }
}

using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    #region References
    
    private Rigidbody _rb;
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxSpeed;
    private Vector3 _input;
    
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 10f;
    
    #endregion
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) Destroy(this);
    }

    private void Update()
    {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        _rb.AddForce(_input.normalized * (moveSpeed * 10f), ForceMode.Force);
        _rb.linearVelocity = Vector3.ClampMagnitude(_rb.linearVelocity, maxSpeed);
    }

    private void HandleRotation()
    {
        if (_input != Vector3.zero)
            transform.forward = Vector3.Slerp(transform.forward, _input.normalized, Time.deltaTime * rotationSpeed);
    }
}

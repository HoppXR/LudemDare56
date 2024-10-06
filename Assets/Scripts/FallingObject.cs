using Unity.Netcode;
using UnityEngine;

public class FallingObject : NetworkBehaviour
{
    private Animator _animator;
    [SerializeField] private float fallCooldown = 3;
    private bool _isFalling;
    private float _nextFallTime;
    
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!IsServer) return;
        
        HandleFall();
    }

    private void HandleFall()
    {
        if (Time.time >= _nextFallTime && !_isFalling)
        {
            _animator.SetTrigger("Fall");
            _isFalling = true;
            _nextFallTime = Time.time + fallCooldown;
            
            Invoke(nameof(FallComplete), 3f);
        }
    }
    
    public void FallComplete()
    {
        _isFalling = false;
        _animator.SetTrigger("GetUp");
    }
}

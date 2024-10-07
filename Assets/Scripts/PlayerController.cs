using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    //private static readonly int Moving = Animator.StringToHash("Moving");
    //private static readonly int Death = Animator.StringToHash("Death");

    #region References
    
    private Rigidbody _rb;
    //private Animator _animator;
    
    //[Header("UI")]
    //[SerializeField] private GameObject uiAlive;
    //[SerializeField] private GameObject uiDead;
    //private Transform _canvasTransform;
    
    [Header("Player Index")]
    [SerializeField] private int playerIndex;
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxSpeed;
    private Vector3 _input;
    //private bool _moving;
    
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 10f;

    //private bool _isDead;
    
    #endregion
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        //_animator = GetComponent<Animator>();
    }

    /*
    private void Start()
    {
        _canvasTransform = FindFirstObjectByType<Canvas>().transform;
        
        Instantiate(uiAlive);
        Instantiate(uiDead);
        
        uiAlive.transform.SetParent(_canvasTransform);
        uiDead.transform.SetParent(_canvasTransform);
        
        uiAlive.SetActive(true);
        uiDead.SetActive(false);
    }*/

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) Destroy(this);
    }

    private void Update()
    {
        //if (_isDead) return;
        
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        //_moving = _input != Vector3.zero; 
        //_animator.SetBool(Moving, _moving);
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

    private void Die()
    {
        //_isDead = true;
        
        transform.localScale = new Vector3(transform.localScale.x, 0, transform.localScale.z);
        
        //uiAlive.SetActive(false);
        //uiDead.SetActive(true);
        
        GameManager.instance.PlayerDie(playerIndex);
        
        _rb.linearVelocity = Vector3.zero;
        
        Destroy(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("FallingObject"))
        {
            Die();
        }
    }
}

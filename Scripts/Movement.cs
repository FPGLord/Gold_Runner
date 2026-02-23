using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private float _gravityScale;
    [SerializeField] private GameObject[] _lives;
    [SerializeField] private GameObject _enemy;
    
    private InputActions _inputActions;
    private float _vertical;
    private bool _isLadder;
    private bool _isClimbing;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private int _currentLifeIndex;

    private void Awake()
    {
        _inputActions = new InputActions();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        
    }

    private void Start()
    {
        _currentLifeIndex = _lives.Length;
    }

    private void OnEnable()
    {
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

    private void Update()
    {
        Vector2 moveInput = _inputActions.Character.Move.ReadValue<Vector2>();
        float horizontal = moveInput.x;
        _vertical = moveInput.y;
       
        if (horizontal < 0)
        {
            _spriteRenderer.flipX = false;
        }
        else if (horizontal > 0)
        {
            _spriteRenderer.flipX = true;
        }

        if (_isLadder && Mathf.Abs(_vertical) > 0.1f)
        {
            _isClimbing = true;
        }
        
        bool isRunning = Mathf.Abs(horizontal) > 0.1f; // если движемся по X
        _animator.SetBool("Run", isRunning); // ← обновляем параметр
        
    }

    private void FixedUpdate()
    {
        Vector2 moveInput = _inputActions.Character.Move.ReadValue<Vector2>();
        float horizontal = moveInput.x;

        Vector2 move = new Vector2(horizontal * _speed, 0);

        if (_isClimbing)
        {
            _rb.gravityScale = 0;
            move.y = _vertical * _speed;
        }
        else
        {
            _rb.gravityScale = _gravityScale;
        }

        _rb.linearVelocity = move;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            _isLadder = true;
        }
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject == _enemy)
        {
            if (_currentLifeIndex > 0)
            {
                _lives[_currentLifeIndex - 1].SetActive(false);
                _currentLifeIndex--;
            }
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            _isLadder = false;
            _isClimbing = false;
        }
    }
}
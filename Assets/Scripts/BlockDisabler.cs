using UnityEngine;
using UnityEngine.InputSystem;

public class BlockDisabler : MonoBehaviour
{
    [SerializeField] private float _disableDuration = 5f;
    [SerializeField] private float _detectionRadius = 1.5f;
    [SerializeField] private LayerMask _blockLayer; 

    private InputActions _inputActions;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    
    private float _lastHorizontalDirection = 0f; // 

    private void Awake()
    {
        _inputActions = new InputActions();
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
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
        
        
        if (Mathf.Abs(moveInput.x) > 0.1f)
        {
            _lastHorizontalDirection = moveInput.x > 0 ? 1f : -1f;
        }

       
        if (_inputActions.Character.DestroyBlock.triggered)
        {
            DisableBlockInDirection(_lastHorizontalDirection);
        }
    }

    private void DisableBlockInDirection(float direction)
    {
       
        Vector2 detectionPosition = (Vector2)transform.position + new Vector2(direction * _detectionRadius, 0f);

       
        Collider2D[] hits = Physics2D.OverlapCircleAll(detectionPosition, 0.5f, _blockLayer);

        if (hits.Length == 0)
        {
            return;
        }

        
        Collider2D nearestBlock = null;
        float minDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            float distance = Vector2.Distance(hit.transform.position, detectionPosition);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestBlock = hit;
            }
        }

        if (nearestBlock != null)
        {
            StartCoroutine(ReenableBlock(nearestBlock.gameObject));
        }
    }

    private System.Collections.IEnumerator ReenableBlock(GameObject block)
    {
        Renderer renderer = block.GetComponent<Renderer>();
        Collider2D collider = block.GetComponent<Collider2D>();

        if (renderer != null)
        {
            renderer.enabled = false;
        }

        if (collider != null)
        {
            collider.enabled = false;
        }

        yield return new WaitForSeconds(_disableDuration);

        if (renderer != null)
        {
            renderer.enabled = true;
        }

        if (collider != null)
        {
            collider.enabled = true;
        }
    }
}
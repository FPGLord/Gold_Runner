using System;
using UnityEngine;

public class AIEnemy : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private GameObject _character;
    [SerializeField] private float _speedEnimyOnLadder = 4f;

    private SpriteRenderer _spriteRenderer;
    private bool _isOnLadder = false;
    private float _ladderBottomY;
    private float _ladderTopY;
    private float _ladderXPosition;
    private Rigidbody2D _rb;
    

    private void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        FollowCharacter();
        SetFlipDirection();
    }

    private void SetFlipDirection()
    {
        if (_character.transform.position.x > transform.position.x)
            _spriteRenderer.flipX = false;
        else if (_character.transform.position.x < transform.position.x)
            _spriteRenderer.flipX = true;
    }

    private void FollowCharacter()
    {
        // Если враг и персонаж на одном уровне по Y, игнорируем лестницу
        if (Mathf.Approximately(transform.position.y, _character.transform.position.y))
        {
            if (_isOnLadder)
            {
                _isOnLadder = false;
                Debug.Log("Враг вышел с лестницы, т.к. на уровне с персонажем.");
            } 
            Vector2 targetPos = new Vector2(_character.transform.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPos, _moveSpeed * Time.deltaTime);
            return;  
        }
        
        if (_isOnLadder)
        {
            ClimbLadder();
        }
        else
        {
            // Обычное движение по X к игроку
            Vector2 targetPos = new Vector2(_character.transform.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, targetPos, _moveSpeed * Time.deltaTime);
        }
    }

    private void ClimbLadder()
    {
            
            float targetY = _character.transform.position.y;
            float currentY = transform.position.y;

            float directionY = 0f;
            if (targetY > currentY) directionY = 1f;
            else if (targetY < currentY) directionY = -1f;
            else
            {
                _isOnLadder = false;
                return;
            }

            float newY = currentY;
            if (directionY != 0f)
            {
                newY = currentY + directionY * _speedEnimyOnLadder * Time.deltaTime;
                newY = Mathf.Clamp(newY, _ladderBottomY, _ladderTopY);
            }

            // ✅ Используем MovePosition для Kinematic или Dynamic тел
            _rb.MovePosition(new Vector2(_ladderXPosition, newY));

            bool atTop = Mathf.Approximately(newY, _ladderTopY);
            bool atBottom = Mathf.Approximately(newY, _ladderBottomY);
            bool playerAtSameLevel = Mathf.Approximately(transform.position.y, targetY);

            if (playerAtSameLevel || atTop || atBottom)
            {
                _isOnLadder = false;
                Debug.Log("Враг покидает лестницу");
            }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            float enemyY = transform.position.y;
            float characterY = _character.transform.position.y;

            bool needToClimb = Math.Abs(enemyY - characterY) > 0.5f;
            float ladderCenterX = other.bounds.center.x;
            float distanceX = Math.Abs(transform.position.x - ladderCenterX);
            bool isNearLadder = distanceX < 0.8f;

            if (needToClimb && isNearLadder)
            {
                // Только сохраняем данные, НЕ меняем позицию!
                _ladderXPosition = ladderCenterX;
                _ladderBottomY = other.bounds.min.y;
                _ladderTopY = other.bounds.max.y;
                _isOnLadder = true;
                Debug.Log($"Враг обнаружил лестницу. Готов к подъёму.");
            }
        }
        
    }

    private float GetBottomY(Transform t)
    {
        // Получаем размер спрайта в юнитах
        SpriteRenderer sr = t.GetComponentInChildren<SpriteRenderer>();
        if (sr == null) return t.position.y;

        Vector2 size = sr.sprite.bounds.size;
        float pixelsPerUnit = sr.sprite.pixelsPerUnit;
        float heightInUnits = size.y / pixelsPerUnit;

        
        return sr.bounds.min.y;
    }
}
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

    private void Start()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
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
            // Определяем, куда двигаться по Y в зависимости от позиции игрока
            float targetY = _character.transform.position.y;
            float currentY = transform.position.y;
          

            // Определяем направление: вверх или вниз
            float directionY = 0f;

            if (targetY > currentY)
            {
                // Игрок выше → поднимаемся
                directionY = 1f;
                // Debug.Log("Поднимаемся по лестнице");
            }
            else if (targetY < currentY)
            {
                // Игрок ниже → спускаемся
                directionY = -1f;
                // Debug.Log("Спускаемся по лестнице");
            }
            
            else
            {
                // Уже на уровне игрока - останавливаемся
                directionY = 0f;
                _isOnLadder = false;
                Debug.Log("Достигли уровня игрока на лестнице, останавливаемся");
            }
            
            
            // Двигаемся по Y, X остаётся фиксированным (на лестнице)
            float newY = currentY + directionY * _speedEnimyOnLadder * Time.deltaTime;

            // Ограничиваем движение в пределах лестницы
            newY = Mathf.Clamp(newY, _ladderBottomY, _ladderTopY);

            transform.position = new Vector2(_ladderXPosition, newY);

            // Условие выхода с лестницы:
            // 1. Враг достиг верха/низа лестницы
            // 2. Игрок больше не требует подъёма/спуска (т.е. Y врага ≈ Y игрока)
            // bool reachedEndOfLadder = Mathf.Approximately(newY, _ladderBottomY) || Mathf.Approximately(newY, _ladderTopY);
            bool playerAtSameLevel = Mathf.Approximately(transform.position.y, targetY);
            
            if (playerAtSameLevel || playerAtSameLevel)
            {
                _isOnLadder = false;
                Debug.Log("Враг покидает лестницу");
            }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
       
        if (other.CompareTag("Ladder"))
        {
            float enemyBottom = GetBottomY(transform);
            float characterBottom = GetBottomY(_character.transform);
            
            // Проверяем, не на одном ли уровне враг и игрок
             if (!Mathf.Approximately(transform.position.y, _character.transform.position.y))
             {
            
                _ladderXPosition = other.bounds.center.x;
                _ladderBottomY = other.bounds.min.y;
                _ladderTopY = other.bounds.max.y;
                
                transform.position = new Vector2(_ladderXPosition, transform.position.y);
                
                _isOnLadder = true;
                Debug.Log("Лестница: нижняя граница = " + _ladderBottomY + ", верхняя граница = " + _ladderTopY);
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

        // Pivot = Bottom → нижняя точка = position.y
        // Pivot = Center → нижняя точка = position.y - heightInUnits/2
        // Для надёжности: используем bounds.min.y
        return sr.bounds.min.y;
    }
}
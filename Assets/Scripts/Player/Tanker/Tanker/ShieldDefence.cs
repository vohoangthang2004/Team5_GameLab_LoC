using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldDefence : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float extendDuration = 0.15f;
    [SerializeField] private float retractDuration = 0.15f;
    [SerializeField] private float moveDistance = 2f;

    [Header("Damage/Push Settings")]
    [SerializeField] private float pushForce = 10f;
    [SerializeField] private LayerMask enemyLayer;

    private bool isMoving=false;
    private Vector3 startPosition;
    private float currentTime = 0f;
    private bool movingForward = true;

    void Start()
    {
        startPosition = transform.localPosition;
    }
    
    void Update()
    {
        if (isMoving)
        {
            MoveShield();
        }
    }

    public void StartShieldMove()
    {
        if (!isMoving)
        {
            isMoving = true;
            movingForward = true;
            currentTime = 0f;
        }
    }

    void MoveShield()
    {
        if (movingForward)
        {
            currentTime += Time.deltaTime;
            float progress = currentTime / extendDuration;

            if(progress >= 1f)
            {
                progress = 1f;
                movingForward = false;
                currentTime = 0f;
            }

            float distance = Mathf.Lerp(0f, moveDistance, progress);
            transform.localPosition = startPosition + Vector3.left * distance;
        }
        else
        {
            currentTime += Time.deltaTime;
            float progress = currentTime / retractDuration;

            if (progress >= 1f)
            {
                progress = 1f;
                isMoving = false;
            }
            float distance = Mathf.Lerp(moveDistance, 0f, progress);
            transform.localPosition = startPosition + Vector3.left * distance;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isEnemy(collision))
        {
            EnemyController enemy = collision.GetComponent<EnemyController>();
            if (enemy != null)
            {
                // Only push if it's a valid enemy with EnemyController
                Vector2 pushDir = (collision.transform.position - transform.position).normalized;
                enemy.GetPushed(pushDir, pushForce);
                Debug.Log($"Shield pushed enemy {collision.name} away!");
            }
            // Removed: The Rigidbody2D force application, as it's redundant and causes issues with non-enemies
        }
    }

    private bool isEnemy(Collider2D col)
    {
        return col.CompareTag("Enemy") || ((1 << col.gameObject.layer) & enemyLayer) != 0;
    }
}
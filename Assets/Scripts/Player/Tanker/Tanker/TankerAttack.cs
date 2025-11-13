using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankerAttack : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damage = 10;

    [Header("Push Settings")]
    public float pushForce = 5f;

    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();
    private bool canDamage = false;

    public void SetDamage(int damageAmount)
    {
        damage = damageAmount;
    }

    // Call this when attack starts
    public void StartAttack()
    {
        canDamage = true;
        hitEnemies.Clear();
    }

    // Call this when attack ends 
    public void EndAttack()
    {
        canDamage = false;
        hitEnemies.Clear();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Weapon collided with: {other.gameObject.name}, Tag: {other.tag}, canDamage: {canDamage}");
        if (!canDamage)
        {
            Debug.Log("Cannot damage - attack not active");
            return;
        }
        if (other.CompareTag("Enemy") && !hitEnemies.Contains(other.gameObject))
        {
            Debug.Log("Hit an enemy! Attempting damage...");
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                Vector2 pushDirection = (other.transform.position - transform.position).normalized;
                Debug.Log($"Calling TakeDamageAndPush with damage: {damage}, pushForce: {pushForce}, direction: {pushDirection}");
                enemy.TakeDamageAndPush(damage, pushDirection, pushForce);
                hitEnemies.Add(other.gameObject);
                Debug.Log("Damage and push applied!");
            }
            else
            {
                Debug.LogError($"No EnemyController on {other.gameObject.name}!");
            }
        }
    }

    void OnEnable()
    {
        hitEnemies.Clear();
    }

    void OnDisable()
    {
        hitEnemies.Clear();
        canDamage = false;
    }
}
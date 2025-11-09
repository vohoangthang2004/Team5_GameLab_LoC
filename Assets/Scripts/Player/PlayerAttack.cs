using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damage = 20;
    
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();
    private bool canDamage = false; 

    public void SetDamage (int damageAmount)
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
        Debug.Log("Sword collided with: " + other.gameObject.name); // See what you're hitting

        if (!canDamage)
        {
            Debug.Log("Cannot damage - attack not active");
            return;
        }

        Debug.Log("Can damage! Checking if enemy...");

        if (other.CompareTag("Enemy") && !hitEnemies.Contains(other.gameObject))
        {
            Debug.Log("Hit an enemy!");
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                hitEnemies.Add(other.gameObject);
                Debug.Log("Enemy took " + damage + " damage!");
            }
            else
            {
                Debug.Log("No EnemyController found on " + other.gameObject.name);
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

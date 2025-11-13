using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int maxHealth = 50;
    public int currentHealth;
    public EnemyHealthBar healthBar;

    [Header("Detection Settings")]
    public float detectionRange = 5f;
    public float attackRange = 2.5f;
    public LayerMask playerLayer;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float chaseSpeed = 3f;

    [Header("Attack")]
    public float attackCooldown = 1.5f;
    public int attackDamage = 10;

    [Header("Push")]
    public float pushStunDuration = 0.5f;

    [Header("Animator")]
    public Animator enemyAnimator;
    
    //Private variables
    private Transform player;
    private Rigidbody2D rb;
    private bool canAttack = true;
    private TankerController tankerController;
    private PlayerController playerController;
    private bool isPushed = false;
    private float pushEndTime = 0f;

    // Enemy states
    private enum EnemyState { Idle, Chase, Attack }
    private EnemyState currentState = EnemyState.Idle;
    
    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }

        rb = GetComponent<Rigidbody2D>();

        if(enemyAnimator == null)
        {
            enemyAnimator = GetComponent<Animator>();
        }

        GameObject tankerObj = GameObject.FindGameObjectWithTag("Tanker");
        if (tankerObj != null)
        {
            player = tankerObj.transform;
            tankerController = tankerObj.GetComponent<TankerController>();
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerController = playerObj.GetComponent<PlayerController>();
        }
    }

    void Update()
    {
        if(player == null || currentHealth <=0) return;

        if (isPushed && Time.time >= pushEndTime)
        {
            isPushed = false;
        }

        if (isPushed)
        {
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        //State machine Logic
        if (distanceToPlayer <= attackRange && canAttack)
        {
            currentState = EnemyState.Attack;
        }
        else if(distanceToPlayer <= detectionRange)
        {
            currentState = EnemyState.Chase;
        }
        else
        {
            currentState = EnemyState.Idle;
        }

        //Execute state beviour
        switch (currentState) 
        { 
            case EnemyState.Idle:
                HandleIdle();
                break;
            case EnemyState.Chase:
                HandleChase();
                break;
            case EnemyState.Attack:
                HandleAttack();
                break;
        }

        // Update animator - Speed set to 0 during attack for idle animation
        if (enemyAnimator != null)
        {
            // If attacking, force Speed to 0 to play idle animation
            if (currentState == EnemyState.Attack || isPushed)
            {
                enemyAnimator.SetFloat("Speed", 0f);
            }
            else
            {
                enemyAnimator.SetFloat("Speed", rb.velocity.magnitude);
            }
        }

    }

    void HandleIdle()
    {
        //Stop moving
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }

    void HandleChase()
    {
        //Chase player
        Vector2 direction = (player.position - transform.position).normalized;

        if(rb!= null)
        {
            rb.velocity = direction * chaseSpeed;
        }

        //Flip enemy to face player 
        if(direction.x != 0)
        {
            bool flipped = direction.x > 0;
            this.transform.rotation = Quaternion.Euler(new Vector3(0f, flipped ? 180f : 0f, 0f));
        }
    }

    void HandleAttack()
    {
        //stop moving while attacking 
        if(rb!= null)
        {
            rb.velocity = Vector2.zero;
        }

        //Facing to player
        Vector2 direction = (player.position - transform.position).normalized;
        if(direction.x != 0)
        {
            bool flipped = direction.x > 0;
            this.transform.rotation = Quaternion.Euler(new Vector3(0f, flipped ? 180f : 0f, 0f));
        }

        //continue to attack if player is in range
        if (canAttack)
        {
            StartCoroutine(PerformAttack());
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Current health: {currentHealth}");

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void GetPushed(Vector2 pushDirection, float pushForce)
    {
        if (rb != null && currentHealth > 0)
        {
            isPushed = true;

            pushEndTime = Time.time + pushStunDuration;

            rb.velocity = Vector2.zero;
            rb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
            Debug.Log($"{gameObject.name} was pushed!");
        }
    }

    public void TakeDamageAndPush(int damage, Vector2 pushDirection, float pushForce)
    {
        Debug.Log($"{gameObject.name} - TakeDamageAndPush called: damage={damage}, push={pushForce}");

        // Apply damage FIRST
        TakeDamage(damage);

        // Then push if still alive (currentHealth is already updated from TakeDamage)
        if (currentHealth > 0)
        {
            GetPushed(pushDirection, pushForce);
        }
        else
        {
            Debug.Log($"{gameObject.name} died from damage, no push applied");
        }
    }

    IEnumerator PerformAttack()
    {
        canAttack = false;

        //Deak damage to player
        if (playerController != null)
        {
            playerController.TakeDamage(attackDamage);
            Debug.Log("Enemy attacked player for " + attackDamage + " damage!");
        }

        //Cool down
        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }

    void Die()
    {
        //Disable when die
        this.enabled = false;
        if(rb != null) 
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;
        }

        //Destroy 
        Destroy(gameObject, 0.5f);
    }

    // Visualize detection ranges in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

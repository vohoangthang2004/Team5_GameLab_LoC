using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 5f;

    [Header("Player Animator (Movement)")]
    public Animator playerAnimator; // For player movement animations

    [Header("Attack Prefab")]
    public GameObject normalAttackPrefab;
    public GameObject abilityAttackPrefab;

    [Header("Attack Spawn Settings")]
    public Transform attackSpawnPoint; 
    public bool attachToPlayer = true;

    [Header("Attack Durations")]
    public float normalAttackDuration = 0.5f;
    public float abilityAttackDuration = 0.5f;

    [Header("Fighter's Health")]
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBarScript healthBar;
    // Private variables
    private Rigidbody2D rb;
    private Vector2 movementDirection;
    private bool isAttacking = false;
    private GameObject currentAttackInstance;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
        rb = GetComponent<Rigidbody2D>();
        // Get animator player
        if (playerAnimator == null)
        {
            playerAnimator = GetComponent<Animator>();
        }

        if (attackSpawnPoint == null)
        {
            attackSpawnPoint = this.transform;
        }
    }

    void Update()
    {
        // Get input for both horizontal and vertical movement
        movementDirection = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );

        if (playerAnimator != null)
        {
            playerAnimator.SetFloat("Speed", movementDirection.magnitude * movementSpeed);
        }

        if (movementDirection.x != 0)
        {
            bool flipped = movementDirection.x > 0;
            this.transform.rotation = Quaternion.Euler(new Vector3(0f, flipped ? 180f : 0f, 0f));
        }
        if (Input.GetKeyDown(KeyCode.E) && !isAttacking)
        {
            StartCoroutine(PerformAbilityAttack());
        }
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(PerformNormalAttack());
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(20);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }
    void FixedUpdate()
    {
        if (rb != null)
        {
            // Use Rigidbody2D for physics-based movement
            rb.velocity = movementDirection * movementSpeed;
        }
    }

    private IEnumerator PerformNormalAttack()
    {
        isAttacking = true;

        // Spawn normal attack prefab
        SpawnAttackPrefab(normalAttackPrefab);

        yield return new WaitForSeconds(normalAttackDuration);

        // Destroy attack instance after duration
        if (currentAttackInstance != null)
        {
            Destroy(currentAttackInstance);
        }

        isAttacking = false;
    }

    private IEnumerator PerformAbilityAttack()
    {
        isAttacking = true;

        // Spawn ability attack prefab
        SpawnAttackPrefab(abilityAttackPrefab);

        yield return new WaitForSeconds(abilityAttackDuration);

        // Destroy attack instance after duration
        if (currentAttackInstance != null)
        {
            Destroy(currentAttackInstance);
        }

        isAttacking = false;
    }

    private void SpawnAttackPrefab(GameObject attackPrefab)
    {
        if (attackPrefab == null)
        {
            Debug.LogWarning("Attack prefab is not assigned!");
            return;
        }

        // Destroy previous attack if exists
        if (currentAttackInstance != null)
        {
            Destroy(currentAttackInstance);
        }

        // Spawn at spawn point position and rotation
        if (attachToPlayer)
        {
            // Spawn as child of player (follows player)
            currentAttackInstance = Instantiate(
                attackPrefab,
                attackSpawnPoint.position,
                attackSpawnPoint.rotation,
                this.transform // Parent to player
            );
        }
        else
        {
            // Spawn independently (stays in place)
            currentAttackInstance = Instantiate(
                attackPrefab,
                attackSpawnPoint.position,
                attackSpawnPoint.rotation
            );
        }
        Animator attackAnimator = currentAttackInstance.GetComponent<Animator>();
        if (attackAnimator != null)
        {
            // The animator on the prefab should auto-play or you can trigger here
            // attackAnimator.SetTrigger("Attack");
        }
    }

    public void OnAttackAnimationEnd()
    {
        isAttacking = false;

        if (currentAttackInstance != null)
        {
            Destroy(currentAttackInstance);
        }
    }

    public void TriggerNormalAttack()
    {
        if (!isAttacking)
        {
            StartCoroutine(PerformNormalAttack());
        }
    }

    public void TriggerAbilityAttack()
    {
        if (isAttacking)
        {
            StartCoroutine(PerformAbilityAttack());
        }
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

    public Vector2 GetMovementDirection()
    {
        return movementDirection;
    }
}
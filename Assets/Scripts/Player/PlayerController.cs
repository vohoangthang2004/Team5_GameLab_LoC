using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movementDirection;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Get input for both horizontal and vertical movement
        movementDirection = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );

        // Update animator with movement speed
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(movementDirection.magnitude * movementSpeed));
        }

        // Handle sprite flipping based on horizontal movement
        if (movementDirection.x != 0)
        {
            bool flipped = movementDirection.x > 0;
            this.transform.rotation = Quaternion.Euler(new Vector3(0f, flipped ? 180f : 0f, 0f));
        }
    }

    void FixedUpdate()
    {
        // Use Rigidbody2D for physics-based movement
        rb.velocity = movementDirection * movementSpeed;
    }
}
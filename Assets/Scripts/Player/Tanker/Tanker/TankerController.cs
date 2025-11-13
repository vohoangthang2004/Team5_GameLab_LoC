using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 5f;

    [Header("Tanker Animator (Movement)")]
    public Animator tankerAnimator;

    [Header("Shield")]
    [SerializeField] private ShieldDefence shieldDefence;
    [SerializeField] private KeyCode shieldKey = KeyCode.E;

    private Rigidbody2D rb;
    private Vector2 movementDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (tankerAnimator == null)
        {
            tankerAnimator = GetComponent<Animator>();
        }

        if(shieldDefence == null)
        {
            shieldDefence = GetComponentInChildren<ShieldDefence>();
            if (shieldDefence == null)
            {
                Debug.LogError("ShieldDefence component not found! Please assign it in the Inspector.");
            }
        }
    }

    void Update()
    {

        movementDirection = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );

        if (tankerAnimator != null)
        {
            tankerAnimator.SetFloat("Speed", movementDirection.magnitude * movementSpeed);
        }

        // Trigger shield movement when E is pressed
        if (Input.GetKeyDown(shieldKey) && shieldDefence != null)
        {
            shieldDefence.StartShieldMove();
        }

        if (movementDirection.x != 0)
        {
            bool flipped = movementDirection.x > 0;
            this.transform.rotation = Quaternion.Euler(new Vector3(0f, flipped ? 180f : 0f, 0f));
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            // Use Rigidbody2D for physics-based movement
            rb.velocity = movementDirection * movementSpeed;
        }
    }

    public Vector2 GetMovementDirection()
    {
        return movementDirection;
    }
}

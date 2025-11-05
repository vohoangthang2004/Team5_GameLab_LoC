using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    [Header("Animation Settings")]
    public Animator animator; // Drag your Player's Animator here
    public string attackTriggerName = "Attack"; // Name of the trigger parameter
    public string normalAttackStateName = "NormalAttack"; // Name of the attack animation state

    [Header("Optional: Script-based Rotation")]
    public bool useScriptRotation = false; // Set to true if you want script rotation instead of animation
    public Transform sword;
    public float rotationSpeed = 720f;
    public float rotationAmount = 180f;

    private bool isAttacking = false;
    private float startAngle;
    private float endAngle;
    private float currentRotation;
    private bool swingForward = true;

    void Start()
    {
        // Get the Animator component if not assigned
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Setup script rotation if enabled
        if (useScriptRotation && sword != null)
        {
            startAngle = sword.localEulerAngles.z;
            endAngle = startAngle + rotationAmount;
            currentRotation = startAngle;
        }
    }

    void Update()
    {
        // Check if player presses E
        if (Input.GetKeyDown(KeyCode.E) && !isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
        // Check for normal attack input (left mouse button)
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            StartCoroutine(PerformNormalAttack());
        }

        // Perform script-based rotation if enabled
        if (useScriptRotation && isAttacking)
        {
            RotateSword();
        }
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        // Trigger the animation
        if (animator != null)
        {
            animator.ResetTrigger(attackTriggerName);
            animator.SetTrigger(attackTriggerName);
        }

        // Start script rotation if enabled
        if (useScriptRotation)
        {
            swingForward = true;
            currentRotation = startAngle;
        }
        float length = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    private IEnumerator PerformNormalAttack()
    {
        isAttacking = true;
        // Trigger the animation
        if (animator != null)
        {
            animator.ResetTrigger(normalAttackStateName);
            animator.SetTrigger(normalAttackStateName);
        }
        if (useScriptRotation)
        {
            swingForward = true;
            currentRotation = startAngle;
        }
        float length = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    void RotateSword()
    {
        if (sword == null) return;

        if (swingForward)
        {
            currentRotation += rotationSpeed * Time.deltaTime;

            if (currentRotation >= endAngle)
            {
                currentRotation = endAngle;
                swingForward = false;
            }
        }
        else
        {
            currentRotation -= rotationSpeed * Time.deltaTime;

            if (currentRotation <= startAngle)
            {
                currentRotation = startAngle;
                isAttacking = false;
            }
        }

        sword.localRotation = Quaternion.Euler(0, 0, currentRotation);
    }

    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
    }
}

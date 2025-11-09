using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    [Header("Script-based Rotation")]
    public bool useScriptRotation = false;
    public Transform sword;
    public float rotationSpeed = 720f;
    public float rotationAmount = 180f;

    [Header("Damage Settings")]
    public int normalAttackDamage = 10;
    public int abilityAttackDamage = 20;
    [Header("Attack Script Reference")]
    public PlayerAttack playerAttack;

    private bool isAttacking = false;
    private float startAngle;
    private float endAngle;
    private float currentRotation;
    private bool swingForward = true;

    void Start()
    {
        if (useScriptRotation && sword != null)
        {
            startAngle = sword.localEulerAngles.z;
            endAngle = startAngle + rotationAmount;
            currentRotation = startAngle;

            // Try to find PlayerAttack if not assigned
            if (playerAttack == null)
            {
                playerAttack = sword.GetComponentInChildren<PlayerAttack>();

                if (playerAttack == null)
                {
                    Debug.LogError("PlayerAttack component not found! Please assign it in the Inspector.");
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && useScriptRotation && !isAttacking)
        {
            isAttacking = true;
            swingForward = true;

            if(playerAttack != null)
            {
                playerAttack.SetDamage(normalAttackDamage);
                playerAttack.StartAttack();
            }
        }
        if (Input.GetKeyDown(KeyCode.E) && useScriptRotation && !isAttacking)
        {
            isAttacking = true;
            swingForward = true;

            if (playerAttack != null)
            {
                playerAttack.SetDamage(abilityAttackDamage);
                playerAttack.StartAttack();
            }
        }

        // Continue rotating every frame while attacking
        if (isAttacking && useScriptRotation)
        {
            RotateSword();
        }

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

                // Disable damage when attack ends
                if (playerAttack != null)
                {
                    playerAttack.EndAttack();
                }
            }
        }

        sword.localRotation = Quaternion.Euler(0, 0, currentRotation);
    }
}
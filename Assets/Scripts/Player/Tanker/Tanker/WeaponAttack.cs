using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    [Header("Script-based Rotation")]
    public bool useScriptRotation = false;
    public Transform weapon;
    public float rotationSpeed = 720f;
    public float rotationAmount = 180f;

    [Header("Damage Settings")]
    public int normalAttackDamage = 10;
    [Header("Attack Script Reference")]
    public TankerAttack tankerAttack;

    private bool isAttacking = false;
    private float startAngle;
    private float endAngle;
    private float currentRotation;
    private bool swingForward = true;

    void Start()
    {
        if (useScriptRotation && weapon != null)
        {
            startAngle = weapon.localEulerAngles.z;
            endAngle = startAngle + rotationAmount;
            currentRotation = startAngle;

            // Try to find PlayerAttack if not assigned
            if (tankerAttack == null)
            {
                tankerAttack = weapon.GetComponentInChildren<TankerAttack>();

                if (tankerAttack == null)
                {
                    Debug.LogError("TankerAttack component not found! Please assign it in the Inspector.");
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

            if (tankerAttack != null)
            {
                tankerAttack.SetDamage(normalAttackDamage);
                tankerAttack.StartAttack();
            }
        }

        // Continue rotating every frame while attacking
        if (isAttacking && useScriptRotation)
        {
            RotateWeapon();
        }

    }

    void RotateWeapon()
    {
        if (weapon == null) return;

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
                if (tankerAttack != null)
                {
                    tankerAttack.EndAttack();
                }
            }
        }

        weapon.localRotation = Quaternion.Euler(0, 0, currentRotation);
    }
}

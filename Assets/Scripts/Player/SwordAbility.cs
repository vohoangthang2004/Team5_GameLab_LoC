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
        }
    }

    void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.E)) && useScriptRotation && !isAttacking)
        {
            isAttacking = true;
            swingForward = true;
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
            }
        }

        sword.localRotation = Quaternion.Euler(0, 0, currentRotation);
    }
}
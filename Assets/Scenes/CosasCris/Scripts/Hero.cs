using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class Hero : MonoBehaviour
{
    private List<IAttacker> attacks;
    private IAttacker attacker;

    public Vector3 desiredDirection;
    public int desiredRotation;
    private float elapsedtime;
    public static readonly float GCD = 1f;

    //Movement Stats
    #region Movement
    public float movementSpeed = 750;
    public float turnSpeed = 1;
    private bool canMove;


    private Rigidbody rb;

    #endregion
    public bool isAttacking;


    // Start is called before the first frame update
    void Start()
    {
        isAttacking = false;
        canMove = true;
        rb = GetComponent<Rigidbody>();

        attacks = new List<IAttacker>();
        attacks.Add(new Exploder());
        attacker = attacks[0];
    }

    // Update is called once per frame
    void Update()
    {
        DecideAttack();

        if (!isAttacking && (Mouse.current.leftButton.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame))
            Attack();

        DecideDirection();
    }

    private void DecideAttack()
    {
        if (Keyboard.current.digit1Key.isPressed)
            attacker = attacks[0];
    }

    private void FixedUpdate()
    {
        rb.AddForce(desiredDirection.normalized * movementSpeed * Time.fixedDeltaTime);
    }

    private void DecideDirection()
    {
        desiredDirection = Vector3.zero;
        if (Keyboard.current.wKey.isPressed)
            desiredDirection += transform.forward;
        else if (Keyboard.current.sKey.isPressed)
            desiredDirection += -transform.forward;
        if (Keyboard.current.aKey.isPressed)
            desiredDirection += -transform.right;
        else if (Keyboard.current.dKey.isPressed)
            desiredDirection += transform.right;
    }

    private void Attack()
    {
        StartCoroutine(PerformAttack());
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        elapsedtime = 0;

        attacker.Attack(transform.position, transform.forward);

        while (elapsedtime < GCD)
        {
            Debug.Log("Elapsedtime: " + elapsedtime);
            elapsedtime += Time.deltaTime;
            yield return 0;
        }

        isAttacking = false;
    }
}

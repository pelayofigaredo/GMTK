using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class Hero : MonoBehaviour
{
    Camera cam;
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


    private CharacterController cc;

    #endregion
    public bool isAttacking;
    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        cam = Camera.main;
    }

    // Start is called before the first frame update
    void Start()
    {
        isAttacking = false;
        canMove = true;

        attacks = new List<IAttacker>();
        attacks.Add(new Exploder());
        attacks.Add(new MissileLauncher());
        attacker = attacks[0];
    }

    // Update is called once per frame
    void Update()
    {
        DecideAttack();

        if (!isAttacking && (Mouse.current.leftButton.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame))
            Attack();

        DecideDirection();

        HandleAim();
    }

    private void DecideAttack()
    {
        if (Keyboard.current.digit1Key.isPressed)
            attacker = attacks[0];
        if (Keyboard.current.digit2Key.isPressed)
            attacker = attacks[1];
    }

    private void FixedUpdate()
    {
        cc.Move(desiredDirection.normalized * movementSpeed * Time.fixedDeltaTime);
    }

    private void DecideDirection()
    {
        desiredDirection = Vector3.zero;
        if (Keyboard.current.wKey.isPressed)
            desiredDirection += Vector3.forward;
        else if (Keyboard.current.sKey.isPressed)
            desiredDirection += -Vector3.forward;
        if (Keyboard.current.aKey.isPressed)
            desiredDirection += -Vector3.right;
        else if (Keyboard.current.dKey.isPressed)
            desiredDirection += Vector3.right;
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

    private void HandleAim()
    {
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (!groundPlane.Raycast(ray, out float enter)) return;

        Vector3 hitPoint = ray.GetPoint(enter);
        Vector3 lookDir = hitPoint - transform.position;
        lookDir.y = 0f;

        if (lookDir.sqrMagnitude < 0.0001f) return;

        Quaternion target = Quaternion.LookRotation(lookDir);

        transform.rotation = turnSpeed <= 0f
            ? target
            : Quaternion.RotateTowards(transform.rotation, target, turnSpeed * Time.deltaTime);
    }
}

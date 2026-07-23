using UnityEngine;

[RequireComponent (typeof(CharacterController))]
public class ControllerTest : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float turnSpeed = 0f;
    Camera cam;
    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main;
    }

    private void Update()
    {
        HandleMovement();
        HandleAim();
    }

    private void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");   
        float v = Input.GetAxisRaw("Vertical");    

        Vector3 input = new Vector3(h, 0f, v);
        if (input.sqrMagnitude > 1f) input.Normalize();

        Vector3 velocity = input * moveSpeed;


        velocity.y = 0;

        controller.Move(velocity * Time.deltaTime);
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

        Debug.DrawLine(transform.position, hitPoint);

        if (lookDir.sqrMagnitude < 0.0001f) return; 

        Quaternion target = Quaternion.LookRotation(lookDir);

        transform.rotation = turnSpeed <= 0f
            ? target
            : Quaternion.RotateTowards(transform.rotation, target, turnSpeed * Time.deltaTime);
    }
}

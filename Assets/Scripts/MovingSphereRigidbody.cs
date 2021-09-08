using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovingSphereRigidbody : MonoBehaviour
{
    // Speed
    [Range(0, 20)]
    public float maxSpeed = 2f;
    [Range(0f, 100f)]
    public float maxAcceleration = 10f;
    [Range(0f, 100f)]
    public float maxAirAcceleration = 10f;

    // Jump
    [Range(0f, 10f)]
    public float jumpHeight = 2f;
    [Range(0f, 4)]
    public int maxNumberOfJumps = 2;
    public int timesJumped = 0;


    Vector2 playerInput;
    Vector3 velocity;

    [SerializeField]
    private float jumpTimeBuffer = 0.15f;
    [SerializeField]
    private float desiredJump = 0f;
    bool grounded = true;

    Rigidbody rb;

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    public void UpdateJump(InputAction.CallbackContext context) {
        if (context.performed)
        {
            desiredJump = 0.2f;
        }
    }

    public void UpdateMovementInput(InputAction.CallbackContext context) {
        playerInput = context.ReadValue<Vector2>();
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);
    }

    private void Update() {
        if (desiredJump > 0f)
        {
            desiredJump = Mathf.Max(0, desiredJump - Time.deltaTime);
        }
    }
    private void FixedUpdate() {
        if (grounded) timesJumped = 0;
        Movement();
        CheckJump();
    }

    private void Movement() {

        Vector3 desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;

        float acceleration = grounded ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.fixedDeltaTime;

        velocity = rb.velocity;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
        rb.velocity = velocity;
    }

    private void CheckJump() {
        if (desiredJump > 0f && timesJumped < maxNumberOfJumps)
        {
            desiredJump = 0f;
            grounded = false;
            Jump();
        }
    }

    private void Jump() {
        float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
        rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z);
        timesJumped++;
        Debug.Log("Jump input");
    }

    private void OnCollisionEnter(Collision other) {
        grounded |= IsCollisionUnder(other);
    }
    private void OnCollisionExit(Collision other) {
        grounded |= IsCollisionUnder(other);
    }

    private bool IsCollisionUnder(Collision collision) {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            if (normal.y >= 0.9f) return true;
        }
        return false;
    }
}

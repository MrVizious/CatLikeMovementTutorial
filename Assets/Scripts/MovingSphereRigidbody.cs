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

    // Jump
    [Range(0f, 10f)]
    public float jumpHeight = 2f;
    [Range(0f, 4)]
    public int maxNumberOfJumps = 2;
    public int timesJumped = 0;


    Vector2 playerInput;
    Vector3 velocity;

    bool desiredJump = false;
    bool grounded = false;

    Rigidbody rb;

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    public void UpdateJump(InputAction.CallbackContext context) {
        desiredJump = context.action.triggered;
    }

    public void UpdateMovementInput(InputAction.CallbackContext context) {
        playerInput = context.ReadValue<Vector2>();
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);
    }

    private void FixedUpdate() {
        if (grounded) timesJumped = 0;
        Movement();
        CheckJump();
        grounded = false;
    }

    private void Movement() {
        Vector3 desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
        float maxSpeedChange = maxAcceleration * Time.fixedDeltaTime;
        velocity = rb.velocity;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
        rb.velocity = velocity;
    }

    private void CheckJump() {
        if (desiredJump && timesJumped < maxNumberOfJumps)
        {
            desiredJump = false;
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
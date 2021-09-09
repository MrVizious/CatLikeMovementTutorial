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

    // Jump Variables
    [Range(0f, 10f)]
    public float jumpHeight = 2f;
    [Range(0f, 4f)]
    public int maxNumberOfAirJumps = 1;
    public int timesJumpedOnAir = 0;
    private float timeSinceLastJump = float.MaxValue;
    public float jumpTimeBufferBeforeGrounded = 0.15f;
    public float jumpTimeBufferCoyoteTime = 0.05f;
    public float maxExtendedJumpTime = 0.5f;

    // Jump button variables
    private bool jumpButtonJustPressed = false;
    private bool jumpButtonPressed = false;
    private float timeSinceJumpPressed = float.MaxValue;
    private float onAirTime = 0f;
    [SerializeField]
    bool grounded = true;

    Vector2 playerInput;
    Vector3 velocity;



    Rigidbody rb;

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    public void InputJump(InputAction.CallbackContext context) {
        if (context.performed)
        {
            jumpButtonJustPressed = true;
            jumpButtonPressed = true;
            timeSinceJumpPressed = 0f;
        }
        else if (context.canceled)
        {
            jumpButtonPressed = false;
        }
    }

    public void UpdateMovementInput(InputAction.CallbackContext context) {
        playerInput = context.ReadValue<Vector2>();
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);
    }

    private void FixedUpdate() {
        Movement();
        UpdateJump();
    }

    private void Movement() {

        Vector3 desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;

        float acceleration;
        if (grounded)
        {
            acceleration = maxAcceleration;
        }
        else
        {
            acceleration = maxAirAcceleration;
        }
        float maxSpeedChange = acceleration * Time.fixedDeltaTime;


        velocity = rb.velocity;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
        rb.velocity = velocity;
    }

    private void UpdateJump() {

        // Reset number of jumps
        if (grounded)
        {
            timesJumpedOnAir = 0;
            onAirTime = 0f;

            // Normal grounded jump
            if (jumpButtonJustPressed)
            {
                Jump();
                Debug.Log("Normal jump");
            }
            // Buffered grounded jump
            else if (jumpButtonPressed && timeSinceJumpPressed < jumpTimeBufferBeforeGrounded)
            {
                Jump();
                Debug.Log("Buffered jump");
            }
        }
        else
        {
            // Coyote jump
            if (jumpButtonJustPressed && onAirTime < jumpTimeBufferCoyoteTime && onAirTime <= timeSinceLastJump)
            {
                Jump();
                Debug.Log("Coyote jump");
            }
            // Normal air jump
            else if (jumpButtonJustPressed && timesJumpedOnAir < maxNumberOfAirJumps)
            {
                Jump();
                onAirTime = 0f;
                timesJumpedOnAir++;
                Debug.Log("Air jump");
            }
            onAirTime += Time.deltaTime;
        }

        if (timeSinceLastJump < maxExtendedJumpTime && timeSinceJumpPressed == timeSinceLastJump && jumpButtonPressed)
        {
            AddJumpSpeed();
        }

        // Reset pressed button variable
        jumpButtonJustPressed = false;
        timeSinceJumpPressed += Time.deltaTime;
        timeSinceLastJump += Time.deltaTime;
    }

    private void Jump() {
        grounded = false;
        AddJumpSpeed();
        timeSinceLastJump = 0f;
    }

    private void AddJumpSpeed() {
        float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
        rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z);
    }

    private void OnCollisionEnter(Collision other) {
        grounded |= IsCollisionUnder(other);
    }
    private void OnCollisionExit(Collision other) {
        grounded = IsCollisionUnder(other);
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
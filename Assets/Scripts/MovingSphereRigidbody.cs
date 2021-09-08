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
    public float jumpTimeBuffer = 0.15f;
    [SerializeField]
    public float maxOnAirTime = 0.5f;

    private bool justPressedJump = false;
    private bool pressedButton = false;
    private float timeSinceJumpPressed = float.MaxValue;
    private float onAirTime = 0f;
    bool grounded = true;

    Rigidbody rb;

    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    public void InputJump(InputAction.CallbackContext context) {
        if (context.performed)
        {
            justPressedJump = true;
            pressedButton = true;
            timeSinceJumpPressed = 0f;
        }
        else if (context.canceled)
        {
            onAirTime = 0f;
            pressedButton = false;
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

        float acceleration = grounded ? maxAcceleration : maxAirAcceleration;
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
            timesJumped = 0;
            onAirTime = 0f;
        }
        else
        {
            onAirTime += Time.deltaTime;
        }


        // Jump conditions
        // Normal jump
        if (justPressedJump && timesJumped < maxNumberOfJumps)
        {
            Jump();
        }
        // Buffered jump
        else if (timeSinceJumpPressed < jumpTimeBuffer && timesJumped < maxNumberOfJumps && grounded)
        {
            Jump();
        }


        // Reset pressed button variable
        justPressedJump = false;
        timeSinceJumpPressed += Time.deltaTime;
    }

    private void Jump() {
        grounded = false;
        AddJumpSpeed();
        timesJumped++;
        Debug.Log("Jump input");
    }

    private void AddJumpSpeed() {
        float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
        rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z);
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

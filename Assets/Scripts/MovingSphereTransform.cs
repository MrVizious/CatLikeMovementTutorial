using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovingSphereTransform : MonoBehaviour
{
    [Range(0, 10)]
    public float maxSpeed = 2f;

    [Range(0f, 100f)]
    public float maxAcceleration = 10f;

    Vector2 playerInput;
    Vector3 velocity;

    public void UpdateInput(InputAction.CallbackContext context) {
        playerInput = context.ReadValue<Vector2>();
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);
    }


    private void Update() {
        // Basic Input
        // transform.localPosition = new Vector3(playerInput.x, 0.5f, playerInput.y);

        // Basic movement
        // Vector3 input = new Vector3(playerInput.x, 0f, playerInput.y);
        // Vector3 displacement = input * horizontal_speed * Time.deltaTime;
        // transform.localPosition += displacement;

        // Basic Acceleration
        // Vector3 acceleration = new Vector3(playerInput.x, 0f, playerInput.y) * max_speed;
        // velocity += acceleration * Time.deltaTime;
        // Vector3 displacement = velocity * Time.deltaTime;
        // transform.localPosition += displacement;

        // Complex acceleration
        Vector3 desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
        transform.position += velocity * Time.deltaTime;


    }
}

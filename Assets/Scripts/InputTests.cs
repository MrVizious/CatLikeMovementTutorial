using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputTests : MonoBehaviour
{
    public bool debug;
    public void Move(InputAction.CallbackContext context) {
        Vector2 info = context.ReadValue<Vector2>();
        if (debug) Debug.Log((Vector3)info);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput playerInput;

    public static Vector2 movement;
    public static bool jumpIsPressed;
    public static bool jumpIsHeld;
    public static bool jumpIsReleased;
    public static bool runIsHeld;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _runAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        _moveAction = playerInput.actions["Move"];
        _jumpAction = playerInput.actions["Jump"];
        _runAction = playerInput.actions["Run"];
    }

    private void Update()
    {
        movement = _moveAction.ReadValue<Vector2>();

        jumpIsPressed = _jumpAction.WasPressedThisFrame();
        jumpIsHeld = _jumpAction.IsPressed();
        jumpIsReleased = _jumpAction.WasReleasedThisFrame();

        runIsHeld = _runAction.IsPressed();
    }
}

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class MarioInputManager : MonoBehaviour, MarioActions.IGameplayActions
{
    [Header("Input Actions")]
    private MarioActions marioActions;

    [Header("Unity Events")]
    public UnityEvent<Vector2> onMoveInput;
    public UnityEvent onJumpInput;
    public UnityEvent onJumpRelease;
    public UnityEvent onJumpHoldInput;
    public UnityEvent onDashInput;

    // Properties for direct access (alternative to events)
    public Vector2 MoveInput { get; private set; }
    public bool IsJumpPressed { get; private set; }
    public bool IsJumpHoldPressed { get; private set; }
    public bool IsDashPressed { get; private set; }

    // Expose read-only accessors if listeners will be added by code
    public UnityEvent<Vector2> OnMoveInput => onMoveInput;
    public UnityEvent OnJumpInput => onJumpInput;
    public UnityEvent OnJumpRelease => onJumpRelease;
    public UnityEvent OnJumpHoldInput => onJumpHoldInput;
    public UnityEvent OnDashInput => onDashInput;

    // State tracking
    private bool wasJumpPressed = false;
    private bool wasJumpHoldPressed = false;

    void Awake()
    {
        // Initialize MarioActions
        marioActions = new MarioActions();

        // Register this script as callback handler
        marioActions.gameplay.AddCallbacks(this);
    }

    void OnEnable() => marioActions.gameplay.Enable();
    void OnDisable() => marioActions.gameplay.Disable();

    void OnDestroy()
    {
        // Remove callbacks and dispose
        if (marioActions != null)
        {
            marioActions.gameplay.RemoveCallbacks(this);
            marioActions.Dispose();
        }

        // Do not set UnityEvents to null; remove listeners instead
        onMoveInput.RemoveAllListeners();
        onJumpInput.RemoveAllListeners();
        onJumpRelease.RemoveAllListeners();
        onJumpHoldInput.RemoveAllListeners();
        onDashInput.RemoveAllListeners();
    }

    #region IGameplayActions Implementation

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
        OnMoveInput?.Invoke(MoveInput);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsJumpPressed = true;
            wasJumpPressed = true;
            OnJumpInput?.Invoke();
            Debug.Log("Jump input started");
        }
        else if (context.canceled)
        {
            IsJumpPressed = false;
            if (wasJumpPressed)
            {
                OnJumpRelease?.Invoke();
                wasJumpPressed = false;
                Debug.Log("Jump input released");
            }
        }
    }

    public void OnJumpHold(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsJumpHoldPressed = true;
            wasJumpHoldPressed = true;
            OnJumpHoldInput?.Invoke();
            Debug.Log("Jump hold started (variable height jump)");
        }
        else if (context.canceled)
        {
            IsJumpHoldPressed = false;
            wasJumpHoldPressed = false;
            Debug.Log("Jump hold ended");
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsDashPressed = true;
            OnDashInput?.Invoke();
            Debug.Log("Dash input detected");

            // Reset dash state after frame (dash is one-shot)
            IsDashPressed = false;
        }
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.started)
            Debug.Log("mouse click started");
        else if (context.performed)
        {
            Debug.Log("mouse click performed");
        }
        else if (context.canceled)
            Debug.Log("mouse click cancelled");
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 point = context.ReadValue<Vector2>();
            Debug.Log($"Point detected: {point}");
        }
    }

    #endregion

    // Public methods for querying input state
    public bool GetJumpInputDown()
    {
        return marioActions.gameplay.Jump.WasPressedThisFrame();
    }

    public bool GetJumpInputUp()
    {
        return marioActions.gameplay.Jump.WasReleasedThisFrame();
    }

    public bool GetDashInputDown()
    {
        return marioActions.gameplay.Dash.WasPressedThisFrame();
    }

    public bool GetJumpHoldInputDown()
    {
        return marioActions.gameplay.JumpHold.WasPressedThisFrame();
    }

    public bool IsJumpHeld()
    {
        return marioActions.gameplay.JumpHold.IsPressed();
    }

    // Get current move input as Vector2 (for compatibility with your existing code)
    public Vector2 GetMoveInputVector2()
    {
        return MoveInput;
    }

    // Enable/Disable input (useful for pausing or cutscenes)
    public void EnableInput()
    {
        marioActions.gameplay.Enable();
    }

    public void DisableInput()
    {
        marioActions.gameplay.Disable();
    }

    // Debug visualization
    void Update()
    {
        if (Application.isEditor)
        {
            // Optional: Display current input state in inspector during development
            // You could add [SerializeField] fields to show current input values
        }
    }
}